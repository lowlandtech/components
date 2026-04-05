import fs from "node:fs/promises";
import path from "node:path";
import pixelmatch from "pixelmatch";
import { PNG } from "pngjs";
import { getArtifactsRoot } from "./component-registry.mjs";

async function ensureDir(dir) {
  await fs.mkdir(dir, { recursive: true });
}

async function readPng(filePath) {
  const buffer = await fs.readFile(filePath);
  return PNG.sync.read(buffer);
}

function cropToSize(image, width, height) {
  const cropped = new PNG({ width, height });

  for (let y = 0; y < height; y += 1) {
    for (let x = 0; x < width; x += 1) {
      const srcIdx = (y * image.width + x) << 2;
      const dstIdx = (y * width + x) << 2;

      cropped.data[dstIdx] = image.data[srcIdx];
      cropped.data[dstIdx + 1] = image.data[srcIdx + 1];
      cropped.data[dstIdx + 2] = image.data[srcIdx + 2];
      cropped.data[dstIdx + 3] = image.data[srcIdx + 3];
    }
  }

  return cropped;
}

async function run() {
  const artifactsRoot = getArtifactsRoot();
  const chadcnDir = path.join(artifactsRoot, "chadcn");
  const localDir = path.join(artifactsRoot, "local");
  const diffDir = path.join(artifactsRoot, "diff");
  const reportJsonPath = path.join(artifactsRoot, "report.json");
  const reportMdPath = path.join(artifactsRoot, "report.md");

  const maxDiffRatio = Number(process.env.MAX_DIFF_RATIO || 0.04);
  const pixelThreshold = Number(process.env.PIXELMATCH_THRESHOLD || 0.12);

  await ensureDir(diffDir);

  const chadcnFiles = await fs.readdir(chadcnDir).catch(() => []);
  const localFiles = new Set(await fs.readdir(localDir).catch(() => []));

  const pngFiles = chadcnFiles.filter((name) => name.toLowerCase().endsWith(".png"));
  const results = [];

  for (const fileName of pngFiles) {
    const localExists = localFiles.has(fileName);
    if (!localExists) {
      results.push({
        component: path.parse(fileName).name,
        status: "missing-local",
        fileName
      });
      continue;
    }

    const chadcnPath = path.join(chadcnDir, fileName);
    const localPath = path.join(localDir, fileName);

    const chadcnImage = await readPng(chadcnPath);
    const localImage = await readPng(localPath);

    const hasSizeMismatch = chadcnImage.width !== localImage.width || chadcnImage.height !== localImage.height;

    let chadcnComparable = chadcnImage;
    let localComparable = localImage;
    let comparedSize = `${chadcnImage.width}x${chadcnImage.height}`;

    if (hasSizeMismatch) {
      const width = Math.min(chadcnImage.width, localImage.width);
      const height = Math.min(chadcnImage.height, localImage.height);

      chadcnComparable = cropToSize(chadcnImage, width, height);
      localComparable = cropToSize(localImage, width, height);
      comparedSize = `${width}x${height}`;
    }

    const diff = new PNG({ width: chadcnComparable.width, height: chadcnComparable.height });
    const differentPixels = pixelmatch(
      chadcnComparable.data,
      localComparable.data,
      diff.data,
      chadcnComparable.width,
      chadcnComparable.height,
      { threshold: pixelThreshold }
    );

    const totalPixels = chadcnComparable.width * chadcnComparable.height;
    const diffRatio = totalPixels ? differentPixels / totalPixels : 1;
    const pass = diffRatio <= maxDiffRatio;

    const diffPath = path.join(diffDir, fileName);
    await fs.writeFile(diffPath, PNG.sync.write(diff));

    results.push({
      component: path.parse(fileName).name,
      status: pass ? "pass" : "fail",
      fileName,
      differentPixels,
      totalPixels,
      diffRatio,
      diffPath,
      comparedSize,
      hasSizeMismatch,
      chadcnSize: `${chadcnImage.width}x${chadcnImage.height}`,
      localSize: `${localImage.width}x${localImage.height}`
    });
  }

  const summary = {
    generatedAtUtc: new Date().toISOString(),
    maxDiffRatio,
    pixelThreshold,
    total: results.length,
    pass: results.filter((x) => x.status === "pass").length,
    fail: results.filter((x) => x.status === "fail").length,
    missingLocal: results.filter((x) => x.status === "missing-local").length,
    sizeMismatch: results.filter((x) => x.hasSizeMismatch).length,
    results
  };

  await fs.writeFile(reportJsonPath, JSON.stringify(summary, null, 2), "utf8");

  const lines = [
    "# Visual Comparison Report",
    "",
    `Generated: ${summary.generatedAtUtc}`,
    `Thresholds: maxDiffRatio=${maxDiffRatio}, pixelThreshold=${pixelThreshold}`,
    `Totals: total=${summary.total}, pass=${summary.pass}, fail=${summary.fail}, missingLocal=${summary.missingLocal}, sizeMismatch=${summary.sizeMismatch}`,
    "",
    "| Component | Status | Diff Ratio | Notes |",
    "|---|---|---:|---|"
  ];

  for (const result of results) {
    if (result.status === "pass" || result.status === "fail") {
      const notes = result.hasSizeMismatch
        ? `${result.differentPixels}/${result.totalPixels} px (normalized ${result.comparedSize} from ${result.chadcnSize} vs ${result.localSize})`
        : `${result.differentPixels}/${result.totalPixels} px`;
      lines.push(`| ${result.component} | ${result.status} | ${(result.diffRatio * 100).toFixed(2)}% | ${notes} |`);
    } else {
      lines.push(`| ${result.component} | missing-local | n/a | no local screenshot |`);
    }
  }

  await fs.writeFile(reportMdPath, `${lines.join("\n")}\n`, "utf8");

  console.log(`report: ${reportMdPath}`);

  if (summary.fail > 0 || summary.missingLocal > 0 || summary.sizeMismatch > 0) {
    process.exit(1);
  }
}

run().catch((error) => {
  console.error(error);
  process.exit(1);
});
