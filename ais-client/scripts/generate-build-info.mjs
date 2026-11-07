// Writes the current build timestamp to src/environments/build-info.ts.
// Runs automatically via the npm "pre*" script hooks.
import { writeFileSync } from 'node:fs';

const target = new URL('../src/environments/build-info.ts', import.meta.url);
const content = `export const buildInfo = {\n  buildDate: '${new Date().toISOString()}',\n};\n`;

writeFileSync(target, content);
