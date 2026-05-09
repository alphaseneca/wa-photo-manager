# WhatsApp Photo Manager

A professional WhatsApp bot for managing and organizing photos by phone numbers and categories. Built with [`@open-wa/wa-automate`](https://github.com/open-wa/wa-automate-nodejs).

## Features

- **📱 WhatsApp Integration** — Connects to WhatsApp Web for automated media management
- **🔐 Authorization System** — Only pre-approved phone numbers can use the service
- **📁 Automatic Organization** — Creates folders by phone number within each category
- **📂 Category System** — Organize media into customizable categories (4x6, A4, Polaroid, Banner)
- **📸 Multi-Media Support** — Handles images, videos, and documents
- **🔄 Image Conversion** — HEIC/HEIF auto-converted to JPG; all images standardized via Sharp
- **📊 Photo Counting** — Tracks and reports total photos per folder
- **💬 Configurable Messages** — Fully customizable bot responses
- **🔁 Session Persistence** — QR code scanned once, session persists across restarts

## Prerequisites

- **Node.js 18+**
- **WhatsApp account** (for linking via QR code)

## Installation

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

4. First run (QR code scan required):
```bash
npm start
```
On first launch, a QR code will be saved as `qr_code_photo-manager-session.png` in the project root. Scan it with your WhatsApp mobile app to link the session. **Keep the session alive for at least 5 minutes** after scanning before restarting.

## Configuration

All configuration lives in `config.js`:

### Authorized Numbers

```javascript
ALLOWED_NUMBERS: [
    '9779867936480',  // Nepal
    '1234567890',     // US
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

1. **Send Phone Number** — Send a 7-15 digit phone number to the bot
2. **Select Category** — Reply with a number (1-4) to choose a category
3. **Upload Media** — Send images, videos, or documents
4. **Auto-Organized** — Files are saved to `downloads/[Category]/[PhoneNumber]/`

### Folder Structure

```
downloads/
├── 4x6 Size Photo/
│   └── 9779867936480/
│       ├── 1715234567890.jpg
│       └── 1715234567891.jpg
├── A4 Photo Frame/
│   └── 9779867936480/
├── Polaroid Photo/
└── 18x24 Banner/
```

### Supported Formats

| Type | Formats | Notes |
|------|---------|-------|
| **Images** | JPG, PNG, HEIC, HEIF, GIF, WEBP | All converted to JPG |
| **Videos** | MP4, AVI, MOV | Saved as-is |
| **Documents** | PDF, DOC, DOCX, TXT | Saved as-is |

## Troubleshooting

### QR Code Not Generating

- Set `headless: false` in `config.js` to see the browser window
- Increase `authTimeout` and `qrTimeout` values

### Session Expired / Login Issues

1. Delete the `_IGNORE_photo-manager-session/` folder
2. Delete any `*.data.json` files in the project root
3. Restart with `headless: false` and re-scan the QR code

### "Browser Not Supported" Error

This is handled automatically by the `postinstall` script. If it recurs:
1. Run `npm install` (triggers the postinstall patch)
2. If still failing, manually run `node scripts/postinstall.js`

## Technical Notes

### User Agent Patch

`@open-wa/wa-automate@4.76.0` hardcodes a `WhatsApp/x.x.x Chrome/104.0.0.0` user agent string. WhatsApp Web now rejects this, showing a "browser not supported" page. The `scripts/postinstall.js` script patches this to a clean modern Chrome user agent on every `npm install`.

### Puppeteer Override

The bundled Puppeteer in `wa-automate` (v23) ships with an older Chromium. This project overrides it to Puppeteer v24 (Chromium 148+) via npm `overrides` in `package.json` to ensure compatibility with current WhatsApp Web.

## Scripts

| Command | Description |
|---------|-------------|
| `npm start` | Start the bot |
| `npm run dev` | Start the bot (same as start) |
| `npm run build` | Compile TypeScript to JavaScript |
| `npm run clean` | Remove build artifacts |

## License

MIT License — see LICENSE file for details.

## Support

For issues or questions, please open an issue or contact the administrator.