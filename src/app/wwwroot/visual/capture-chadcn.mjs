import fs from "node:fs/promises";
import path from "node:path";
import { chromium } from "playwright";
import { getArtifactsRoot, getComponentRegistry } from "./component-registry.mjs";

async function ensureDir(dir) {
  await fs.mkdir(dir, { recursive: true });
}

async function run() {
  const registry = await getComponentRegistry();
  const artifactsRoot = getArtifactsRoot();
  const outputDir = path.join(artifactsRoot, "chadcn");
  const metadataPath = path.join(artifactsRoot, "chadcn-manifest.json");

  await ensureDir(outputDir);

  const browser = await chromium.launch({ headless: true });
  const page = await browser.newPage({ viewport: { width: 1440, height: 1200 } });

  const manifest = [];

  for (const component of registry) {
    const screenshotPath = path.join(outputDir, `${component.name}.png`);

    if (!component.chadcnUrl) {
      manifest.push({
        ...component,
        status: "skipped",
        reason: "no-chadcn-page"
      });
      continue;
    }

    try {
      const response = await page.goto(component.chadcnUrl, { waitUntil: "networkidle", timeout: 30000 });
      if (!response || !response.ok()) {
        manifest.push({
          ...component,
          status: "failed",
          reason: `http-${response ? response.status() : "no-response"}`
        });
        continue;
      }

      await page.waitForTimeout(500);

      const target = page.locator("main").first();
      if (await target.count()) {
        await target.screenshot({ path: screenshotPath });
      } else {
        await page.screenshot({ path: screenshotPath, fullPage: true });
      }

      manifest.push({ ...component, status: "captured", screenshotPath });
      console.log(`[captured] ${component.name}`);
    } catch (error) {
      manifest.push({
        ...component,
        status: "failed",
        reason: error instanceof Error ? error.message : "unknown-error"
      });
      console.log(`[failed] ${component.name}`);
    }
  }

  await browser.close();
  await fs.writeFile(metadataPath, JSON.stringify(manifest, null, 2), "utf8");

  const capturedCount = manifest.filter((item) => item.status === "captured").length;
  const skippedCount = manifest.filter((item) => item.status === "skipped").length;
  const failedCount = manifest.filter((item) => item.status === "failed").length;

  console.log(`done: captured=${capturedCount} skipped=${skippedCount} failed=${failedCount}`);
}

run().catch((error) => {
  console.error(error);
  process.exit(1);
});
