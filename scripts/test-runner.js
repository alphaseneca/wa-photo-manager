const fs = require('fs');
const path = require('path');
const { spawnSync } = require('child_process');

const rootDir = path.join(__dirname, '..');
const testsDir = path.join(rootDir, 'tests');

// Discover all test files in tests/ directory cross-platform (compatible with Node 18, 20, 22+)
const testFiles = fs.readdirSync(testsDir)
    .filter(file => file.endsWith('.test.js'))
    .map(file => path.join('tests', file));

if (testFiles.length === 0) {
    console.error('No test files found in tests/');
    process.exit(1);
}

const result = spawnSync(process.execPath, ['--test', ...testFiles], {
    stdio: 'inherit',
    cwd: rootDir
});

process.exit(result.status !== null ? result.status : 1);
