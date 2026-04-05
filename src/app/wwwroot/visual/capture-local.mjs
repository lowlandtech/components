import fs from "node:fs/promises";
import path from "node:path";
import { chromium } from "playwright";
import { getArtifactsRoot, getComponentRegistry } from "./component-registry.mjs";

async function ensureDir(dir) {
  await fs.mkdir(dir, { recursive: true });
}

async function run() {
  const baseUrl = process.env.LOCAL_COMPONENTS_BASE_URL || "http://localhost:5000";
  const registry = await getComponentRegistry();
  const artifactsRoot = getArtifactsRoot();
  const outputDir = path.join(artifactsRoot, "local");
  const metadataPath = path.join(artifactsRoot, "local-manifest.json");

  await ensureDir(outputDir);

  const browser = await chromium.launch({ headless: true });
  const page = await browser.newPage({ viewport: { width: 1440, height: 1200 } });

  const manifest = [];

  for (const component of registry) {
    const screenshotPath = path.join(outputDir, `${component.name}.png`);
    const targetUrl = `${baseUrl}${component.localPath}`;

    try {
      const response = await page.goto(targetUrl, { waitUntil: "networkidle", timeout: 30000 });
      if (!response || !response.ok()) {
        manifest.push({
          ...component,
          status: "failed",
          reason: `http-${response ? response.status() : "no-response"}`,
          targetUrl
        });
        continue;
      }

      // Optional marker: if a page sets this, screenshots are taken only when component is ready.
      await page.waitForSelector("[data-visual-ready='true']", { timeout: 1500 }).catch(() => null);

      const target = page.locator("main").first();
      if (await target.count()) {
        await target.screenshot({ path: screenshotPath });
      } else {
        await page.screenshot({ path: screenshotPath, fullPage: true });
      }

      manifest.push({ ...component, status: "captured", screenshotPath, targetUrl });
      console.log(`[captured] ${component.name}`);
    } catch (error) {
      manifest.push({
        ...component,
        status: "failed",
        reason: error instanceof Error ? error.message : "unknown-error",
        targetUrl
      });
      console.log(`[failed] ${component.name}`);
    }
  }

  await browser.close();
  await fs.writeFile(metadataPath, JSON.stringify(manifest, null, 2), "utf8");

  const capturedCount = manifest.filter((item) => item.status === "captured").length;
  const failedCount = manifest.filter((item) => item.status === "failed").length;

  console.log(`done: captured=${capturedCount} failed=${failedCount} baseUrl=${baseUrl}`);
}

run().catch((error) => {
  console.error(error);
  process.exit(1);
});
