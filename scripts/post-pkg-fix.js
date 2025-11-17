const fs = require("fs");
const path = require("path");

function copyRecursive(src, dest) {
	if (!fs.existsSync(src)) return;

	if (!fs.existsSync(dest)) {
		fs.mkdirSync(dest, { recursive: true });
	}

	for (const item of fs.readdirSync(src)) {
		const srcPath = path.join(src, item);
		const destPath = path.join(dest, item);

		if (fs.lstatSync(srcPath).isDirectory()) {
			copyRecursive(srcPath, destPath);
		} else {
			fs.copyFileSync(srcPath, destPath);
		}
	}
}

console.log("[*] Fixing pkg native module assets...");

const outDir = process.cwd();

// SHARP
copyRecursive(
	"node_modules/sharp/build/Release",
	path.join(outDir, "sharp/build/Release")
);

copyRecursive(
	"node_modules/sharp/vendor/lib",
	path.join(outDir, "sharp/vendor/lib")
);

// OPTIONAL: Puppeteer chromium (if present)
copyRecursive(
	"node_modules/puppeteer/.local-chromium",
	path.join(outDir, "puppeteer/.local-chromium")
);

console.log("[✓] Native modules fixed successfully.");
