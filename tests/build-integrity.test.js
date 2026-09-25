const test = require('node:test');
const assert = require('node:assert/strict');
const path = require('node:path');
const fs = require('node:fs');

test('Build & Packaging Integrity Suite', async (t) => {
    const rootDir = path.join(__dirname, '..');

    await t.test('App branding assets exist with valid signatures', () => {
        const pngPath = path.join(rootDir, 'assets', 'app-logo.png');
        const icoPath = path.join(rootDir, 'assets', 'app-logo.ico');

        assert.ok(fs.existsSync(pngPath), 'assets/app-logo.png must exist');
        assert.ok(fs.existsSync(icoPath), 'assets/app-logo.ico must exist');

        const pngBuf = fs.readFileSync(pngPath);
        // PNG magic bytes: 89 50 4E 47 0D 0A 1A 0A
        assert.equal(pngBuf[0], 0x89, 'PNG byte 0 match');
        assert.equal(pngBuf[1], 0x50, 'PNG byte 1 match');
        assert.equal(pngBuf[2], 0x4E, 'PNG byte 2 match');
        assert.equal(pngBuf[3], 0x47, 'PNG byte 3 match');

        const icoBuf = fs.readFileSync(icoPath);
        // ICO magic bytes: 00 00 01 00
        assert.equal(icoBuf.readUInt16LE(0), 0, 'ICO reserved must be 0');
        assert.equal(icoBuf.readUInt16LE(2), 1, 'ICO type must be 1');
        const iconCount = icoBuf.readUInt16LE(4);
        assert.ok(iconCount >= 1, 'ICO must contain at least 1 image frame');
    });

    await t.test('Installer configuration file scripts/installer.iss is properly parameterized', () => {
        const issPath = path.join(rootDir, 'scripts', 'installer.iss');
        assert.ok(fs.existsSync(issPath), 'installer.iss must exist');

        const content = fs.readFileSync(issPath, 'utf8');
        assert.match(content, /AppVerName=WhatsApp Photo Manager v\{#AppVer\}/, 'Must contain AppVerName header formatting');
        assert.match(content, /AppVersion=\s*\{#AppVer\}/, 'Must specify AppVersion');
        assert.match(content, /SetupIconFile=\.\.\\assets\\app-logo\.ico/, 'Must reference assets\\app-logo.ico');
    });

    await t.test('TypeScript entry point compiles and exports properly', () => {
        const tsconfigPath = path.join(rootDir, 'tsconfig.json');
        assert.ok(fs.existsSync(tsconfigPath), 'tsconfig.json must exist');

        const distEntry = path.join(rootDir, 'dist', 'index.js');
        assert.ok(fs.existsSync(distEntry), 'dist/index.js must exist after build');
    });

    await t.test('Windows C# launcher script exists and defines core lifecycle methods', () => {
        const csPath = path.join(rootDir, 'scripts', 'Launcher.cs');
        assert.ok(fs.existsSync(csPath), 'scripts/Launcher.cs must exist');

        const content = fs.readFileSync(csPath, 'utf8');
        assert.ok(content.includes('class TrayApplicationContext'), 'Must define TrayApplicationContext');
        assert.ok(content.includes('class QrCodeForm'), 'Must define QrCodeForm');
        assert.ok(content.includes('class LogViewerForm'), 'Must define LogViewerForm');
        assert.ok(content.includes('class SettingsForm'), 'Must define SettingsForm');
        assert.ok(content.includes('class RoundedButton'), 'Must define RoundedButton with corner radius support');
        assert.ok(content.includes('class AlreadyRunningDialog'), 'Must define AlreadyRunningDialog');
        assert.ok(content.includes('class ModernMessageBox'), 'Must define ModernMessageBox');
        assert.ok(content.includes('class ModernToastNotification'), 'Must define ModernToastNotification');
        assert.ok(!content.includes('ShowBalloonTip'), 'Must not contain any legacy unstyled ShowBalloonTip calls');
        assert.ok(content.includes('WhatsAppPhotoManagerWakeEvent'), 'Must define IPC wake event');
        assert.ok(content.includes('WhatsAppPhotoManagerPingEvent'), 'Must define IPC ping event');
        assert.ok(!/(?<!Modern)MessageBox\.Show/.test(content), 'Must not contain any legacy unstyled MessageBox.Show calls');
        assert.ok(content.includes('RecolorQrToBlack'), 'Must define high-contrast RecolorQrToBlack method');
    });

    await t.test('QR code recoloring pipeline converts colored pixels to crisp black', async () => {
        const sharp = require('sharp');
        // Create a 2x1 test image: Left pixel red (255, 0, 0), right pixel white (255, 255, 255)
        const redAndWhitePng = await sharp({
            create: {
                width: 2,
                height: 1,
                channels: 3,
                background: { r: 255, g: 0, b: 0 }
            }
        })
        .composite([{
            input: Buffer.from([255, 255, 255]),
            raw: { width: 1, height: 1, channels: 3 },
            top: 0,
            left: 1
        }])
        .png()
        .toBuffer();

        // Process through exact same pipeline as src/index.ts
        const processed = await sharp(redAndWhitePng)
            .flatten({ background: '#ffffff' })
            .threshold(200)
            .toColourspace('srgb')
            .raw()
            .toBuffer();

        // Red pixel (pixel 0, bytes 0-2) must be thresholded to black (0, 0, 0)
        assert.equal(processed[0], 0, 'Colored/red pixel R must be converted to pure black (0)');
        assert.equal(processed[1], 0, 'Colored/red pixel G must be converted to pure black (0)');
        assert.equal(processed[2], 0, 'Colored/red pixel B must be converted to pure black (0)');

        // White pixel (pixel 1, bytes 3-5) must remain pure white (255, 255, 255)
        assert.equal(processed[3], 255, 'White pixel R must remain pure white (255)');
        assert.equal(processed[4], 255, 'White pixel G must remain pure white (255)');
        assert.equal(processed[5], 255, 'White pixel B must remain pure white (255)');
    });
});
