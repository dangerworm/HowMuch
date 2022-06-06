import fs from 'fs';

const outputPath = './availableFilenames.mjs';

export function getFilenames (directoryPath) {
  var filenames = fs.readdirSync(directoryPath)
    .map(fn => `'${fn}'`)
    .join(',\r\n');

  fs.writeFileSync(outputPath, `export const filenames = [\r\n${filenames}\r\n];`);
}