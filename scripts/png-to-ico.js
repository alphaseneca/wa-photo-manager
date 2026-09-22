const fs = require('fs');
const path = require('path');

const pngPath = path.join(__dirname, '../assets/app-logo.png');
const icoPath = path.join(__dirname, '../assets/app-logo.ico');

if (!fs.existsSync(pngPath)) {
    console.error('PNG file not found at:', pngPath);
    process.exit(1);
}

const pngBuffer = fs.readFileSync(pngPath);
const size = pngBuffer.length;

// Create ICO header (6 bytes)
const header = Buffer.alloc(6);
header.writeUInt16LE(0, 0); // Reserved
header.writeUInt16LE(1, 2); // Type (1 for icon)
header.writeUInt16LE(1, 4); // Count (1 image)

// Create Directory entry (16 bytes)
const entry = Buffer.alloc(16);
entry.writeUInt8(0, 0); // Width (0 means 256px)
entry.writeUInt8(0, 1); // Height (0 means 256px)
entry.writeUInt8(0, 2); // Color count (0)
entry.writeUInt8(0, 3); // Reserved (0)
entry.writeUInt16LE(1, 4); // Color planes (1)
entry.writeUInt16LE(32, 6); // Bits per pixel (32)
entry.writeUInt32LE(size, 8); // Size of PNG data
entry.writeUInt32LE(22, 12); // Offset where PNG data starts (6 + 16 = 22)

// Combine header, entry, and png bytes
const icoBuffer = Buffer.concat([header, entry, pngBuffer]);
fs.writeFileSync(icoPath, icoBuffer);

console.log('✓ ICO file generated successfully at:', icoPath);
