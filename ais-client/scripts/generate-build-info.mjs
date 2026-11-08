// Writes the current build timestamp to src/environments/build-info.ts.
// Runs via the npm "prebuild" hook, but only in CI (or with BUILD_INFO=1), so local
// builds keep the committed placeholder and do not leave a modified file behind.
import { writeFileSync } from 'node:fs';

if (!process.env.CI && !process.env.BUILD_INFO) {
  process.exit(0);
}

const target = new URL('../src/environments/build-info.ts', import.meta.url);
const content = `export const buildInfo = {\n  buildDate: '${new Date().toISOString()}',\n};\n`;

writeFileSync(target, content);
