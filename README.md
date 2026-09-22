# WhatsApp Photo Manager

A professional WhatsApp bot and desktop application for managing and organizing photos by phone numbers and categories. Built with [`@open-wa/wa-automate`](https://github.com/open-wa/wa-automate-nodejs).

## Features

- **📱 WhatsApp Integration** — Connects to WhatsApp Web for automated media management
- **🖥️ Windows System Tray App** — Run as a quiet background service with tray menu, live log viewer, and settings GUI
- **🔐 Authorization System** — Only pre-approved phone numbers can use the service
- **📁 Automatic Organization** — Creates folders by phone number within each category
- **📂 Category System** — Organize media into customizable categories (4x6, A4, Polaroid, Banner)
- **📸 Multi-Media Support** — Handles images, videos, and documents
- **🔄 Image Conversion** — HEIC/HEIF auto-converted to JPG; all images standardized via Sharp
- **📊 Photo Counting** — Tracks and reports total photos per folder
- **💬 Configurable Messages** — Fully customizable bot responses
- **🔁 Session Persistence** — QR code scanned once, session persists across restarts
- **📦 Zero-Dependency Installer** — Standalone Windows installer bundling portable Node.js and Google Chrome

## Getting Started

### Option A: Windows Installer (Recommended for End Users)

1. Run the installer located in `installer/WhatsAppPhotoManager-Installer-1.0.0-x64.exe` (or build it yourself).
2. Launch **WhatsApp Photo Manager** from your desktop or start menu.
3. The app starts minimized in the system tray:
   - **QR Code Scan Dialog**: Automatically pops up when a WhatsApp QR code needs to be paired.
   - **Configure Settings**: Right-click the tray icon to edit authorized numbers, photo categories, or download directory without editing code.
   - **View Console Logs**: Real-time log monitoring with ANSI-color filtering.
   - **Open Downloads Storage**: Direct shortcut to your saved files.

### Option B: Developer Setup (Node.js)

#### Prerequisites
- **Node.js 18+**
- **WhatsApp account** (for linking via QR code)

#### Installation

1. Clone the repository:
```bash
git clone <your-repo-url>
cd wa-photo-manager
```

2. Install dependencies:
```bash
npm install
```
> The `postinstall` script automatically patches the WhatsApp Web user agent for compatibility. See [Technical Notes](#technical-notes) for details.

3. Configure the bot — edit `config.js`:
   - Add your authorized phone numbers to `ALLOWED_NUMBERS`
   - Customize photo categories, messages, and folder settings

4. Start the bot:
```bash
npm start
```
On first launch, a QR code will be saved as `qr_code_photo-manager-session.png` in the project root. Scan it with your WhatsApp mobile app to link the session. **Keep the session alive for at least 5 minutes** after scanning before restarting.

## Configuration

All configuration lives in `config.js` (or can be configured via GUI settings into `config.json`):

### Authorized Numbers

```javascript
ALLOWED_NUMBERS: [
    '1234567890',     // US
    '9779800000000',  // Nepal
],
```

### Photo Categories

```javascript
PHOTO_CATEGORIES: [
    '4x6 Size Photo',
    'A4 Photo Frame',
    'Polaroid Photo',
    '18x24 Banner'
],
```

### Bot Settings

| Setting | Default | Description |
|---------|---------|-------------|
| `sessionId` | `photo-manager-session` | Unique session identifier |
| `authTimeout` | `120` | QR code scan timeout (seconds) |
| `qrTimeout` | `120` | QR code generation timeout (seconds) |
| `headless` | `true` | Run browser in background (`false` to debug or re-scan QR) |
| `multiDevice` | `true` | WhatsApp multi-device support |
| `deleteSessionDataOnLogout` | `true` | Auto-cleanup session data on logout |
| `waitForRipeSession` | `true` | Wait for session to fully initialize before injection |

### Messages

Edit the `MESSAGES` object in `config.js` to customize all bot responses. Available variables: `{phone}`, `{category}`, `{folder}`, `{filename}`, `{count}`, `{size}`.

## Usage

1. **Send Phone Number** — Send a 7-15 digit phone number to the bot (e.g., `1234567890`)
2. **Select Category** — Reply with a number (1-4) to choose a category
3. **Upload Media** — Send images, videos, or documents
4. **Auto-Organized** — Files are saved to `downloads/[Category]/[PhoneNumber]/`

### Folder Structure

```
downloads/
├── 4x6 Size Photo/
│   └── 1234567890/
│       ├── 1715234567890.jpg
│       └── 1715234567891.jpg
├── A4 Photo Frame/
│   └── 1234567890/
├── Polaroid Photo/
└── 18x24 Banner/
```

### Supported Formats

| Type | Formats | Notes |
|------|---------|-------|
| **Images** | JPG, PNG, HEIC, HEIF, GIF, WEBP | All converted to JPG |
| **Videos** | MP4, AVI, MOV | Saved as-is |
| **Documents** | PDF, DOC, DOCX, TXT | Saved as-is |

## Building the Windows Desktop Package

To assemble the standalone distribution and compile the C# launcher:

```bash
npm run package
```

This runs `scripts/build-package.ps1` which:
1. Compiles TypeScript source to `dist/`
2. Downloads portable `node.exe` (LTS v20)
3. Bundles standalone Chrome via Puppeteer CLI
4. Compiles `scripts/Launcher.cs` with the custom app logo
5. Copies all runtime dependencies into `out-build/whatsapp-photo-manager`

To compile the single-file setup installer, open `scripts/installer.iss` in Inno Setup and build.

## Troubleshooting

### QR Code Not Generating

- Set `headless: false` in `config.js` or via the Tray Settings menu to inspect the browser window.
- Increase `authTimeout` and `qrTimeout` values.

### Session Expired / Login Issues

1. Delete the `_IGNORE_photo-manager-session/` folder.
2. Delete any `*.data.json` files in the project directory.
3. Restart with `headless: false` and re-scan the QR code.

### "Browser Not Supported" Error

This is handled automatically by the `postinstall` script. If it recurs:
1. Run `npm install` (triggers the postinstall patch).
2. If still failing, manually run `node scripts/postinstall.js`.

## Technical Notes

### User Agent Patch

`@open-wa/wa-automate@4.76.0` hardcodes a `WhatsApp/x.x.x Chrome/104.0.0.0` user agent string. WhatsApp Web now rejects this, showing a "browser not supported" page. The `scripts/postinstall.js` script patches this to a clean modern Chrome user agent on every `npm install`.

### Puppeteer Override

The bundled Puppeteer in `wa-automate` (v23) ships with an older Chromium. This project overrides it to Puppeteer v24 (Chromium 148+) via npm `overrides` in `package.json` to ensure compatibility with current WhatsApp Web.

## Scripts

| Command | Description |
|---------|-------------|
| `npm start` | Start the bot in development mode |
| `npm run build` | Compile TypeScript to JavaScript |
| `npm run package` | Build standalone Windows package with bundled Node & Chrome |
| `npm run clean` | Remove build artifacts |

## License

MIT License — see LICENSE file for details.