/**
 * Post-install patch for @open-wa/wa-automate
 * 
 * Problem: The library prepends "WhatsApp/x.x.x" to the user agent string
 * AND uses Chrome/104. WhatsApp Web rejects this as an unsupported browser.
 * 
 * Fix: Replace the UA with a clean modern Chrome user agent string.
 * This script runs automatically after npm install.
 */
const fs = require('fs');
const path = require('path');

const configPath = path.join(
    __dirname, '..', 'node_modules', '@open-wa', 'wa-automate', 'dist', 'config', 'puppeteer.config.js'
);

if (!fs.existsSync(configPath)) {
    console.log('[postinstall] wa-automate not found, skipping patch.');
    process.exit(0);
}

let content = fs.readFileSync(configPath, 'utf8');

// Replace the createUserAgent function to remove WhatsApp/ prefix and use modern Chrome
const oldPattern = /const createUserAgent = \(waVersion\) => `WhatsApp\/\$\{waVersion\} Mozilla.*?`;/;
const newUA = "const createUserAgent = (waVersion) => `Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/136.0.0.0 Safari/537.36`;";

if (oldPattern.test(content)) {
    content = content.replace(oldPattern, newUA);
    fs.writeFileSync(configPath, content, 'utf8');
    console.log('[postinstall] ✓ Patched wa-automate user agent (removed WhatsApp/ prefix, updated Chrome version)');
} else if (content.includes('Chrome/136.0.0.0') && !content.includes('WhatsApp/')) {
    console.log('[postinstall] User agent already patched, skipping.');
} else {
    console.log('[postinstall] ⚠ Could not find expected UA pattern. Manual patch may be needed.');
}
