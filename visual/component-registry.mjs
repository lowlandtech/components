import fs from "node:fs/promises";
import path from "node:path";
import { fileURLToPath } from "node:url";

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

const DOCS_COMPONENTS_DIR = path.resolve(__dirname, "../../../../../docs/components");

const EXCLUDED_DOC_FILES = new Set(["readme", "_index", "next-components", "visual-regression"]);

// Some docs do not map 1:1 to a public shadcn component page.
const CHADCN_SLUG_OVERRIDES = {
  ButtonGroup: null,
  CodeEditor: null,
  DataGrid: null,
  Direction: null,
  Empty: null,
  Field: null,
  Icon: null,
  InputGroup: null,
  InputOTP: "input-otp",
  Item: null,
  Kbd: "kbd",
  NativeSelect: null,
  SelectInput: null,
  Sonner: "sonner"
};

function toKebabCase(value) {
  return value
    .replace(/([a-z0-9])([A-Z])/g, "$1-$2")
    .replace(/[._\s]+/g, "-")
    .toLowerCase();
}

function mapChadcnSlug(componentName) {
  if (Object.prototype.hasOwnProperty.call(CHADCN_SLUG_OVERRIDES, componentName)) {
    return CHADCN_SLUG_OVERRIDES[componentName];
  }

  return toKebabCase(componentName);
}

export async function getComponentRegistry() {
  const entries = await fs.readdir(DOCS_COMPONENTS_DIR, { withFileTypes: true });

  const names = entries
    .filter((entry) => entry.isFile())
    .map((entry) => path.parse(entry.name).name)
    .filter((name) => !EXCLUDED_DOC_FILES.has(name.toLowerCase()))
    .filter((name) => !name.startsWith("."))
    .sort((a, b) => a.localeCompare(b));

  return names.map((name) => {
    const slug = mapChadcnSlug(name);

    return {
      name,
      slug,
      chadcnUrl: slug ? `https://ui.shadcn.com/docs/components/${slug}` : null,
      localPath: `/visual/${toKebabCase(name)}`
    };
  });
}

export function getArtifactsRoot() {
  // Keep artifacts outside wwwroot so they are not served over HTTP
  return path.resolve(__dirname, "../../visual-artifacts");
}
