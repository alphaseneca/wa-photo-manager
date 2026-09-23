const fs = require('fs');
const path = require('path');
const sharp = require('sharp');

const pngPath = path.join(__dirname, '../assets/app-logo.png');
const icoPath = path.join(__dirname, '../assets/app-logo.ico');

if (!fs.existsSync(pngPath)) {
    console.error('PNG file not found at:', pngPath);
    process.exit(1);
}

async function generateIco() {
    const inputBuf = fs.readFileSync(pngPath);

    // Generate valid PNG buffers for standard Windows icon sizes with transparency preserved
    const png256 = await sharp(inputBuf).resize(256, 256).png().toBuffer();
    const png48 = await sharp(inputBuf).resize(48, 48).png().toBuffer();
    const png32 = await sharp(inputBuf).resize(32, 32).png().toBuffer();
    const png16 = await sharp(inputBuf).resize(16, 16).png().toBuffer();

    const images = [
        { size: 16, buf: png16 },
        { size: 32, buf: png32 },
        { size: 48, buf: png48 },
        { size: 256, buf: png256 }
    ];

    const count = images.length;
    const header = Buffer.alloc(6);
    header.writeUInt16LE(0, 0); // reserved
    header.writeUInt16LE(1, 2); // icon type
    header.writeUInt16LE(count, 4); // count of images

    let offset = 6 + count * 16;
    const entries = [];
    for (const img of images) {
        const entry = Buffer.alloc(16);
        entry.writeUInt8(img.size === 256 ? 0 : img.size, 0); // width (0 = 256)
        entry.writeUInt8(img.size === 256 ? 0 : img.size, 1); // height (0 = 256)
        entry.writeUInt8(0, 2); // color count
        entry.writeUInt8(0, 3); // reserved
        entry.writeUInt16LE(1, 4); // color planes
        entry.writeUInt16LE(32, 6); // bits per pixel (32 = 8-bit RGBA)
        entry.writeUInt32LE(img.buf.length, 8); // size of image data
        entry.writeUInt32LE(offset, 12); // offset in file
        entries.push(entry);
        offset += img.buf.length;
    }

    const ico = Buffer.concat([header, ...entries, ...images.map(i => i.buf)]);
    fs.writeFileSync(icoPath, ico);
    console.log('✓ Valid 32-bit transparent Windows ICO generated at:', icoPath);
}

generateIco().catch(err => {
    console.error('Failed to generate ICO:', err);
    process.exit(1);
});
