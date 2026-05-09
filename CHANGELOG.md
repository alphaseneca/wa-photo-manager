# Changelog

All notable changes to this project will be documented in this file.

## [1.1.0] - 2026-05-09

### Fixed
- **WhatsApp Web Compatibility**: Fixed critical startup failure caused by WhatsApp Web rejecting the browser as "unsupported"
  - **Root Cause**: `@open-wa/wa-automate` hardcodes a user agent string with a `WhatsApp/` prefix and `Chrome/104.0.0.0`. WhatsApp Web now rejects this combination and serves a "browser not supported" error page instead of loading the app. Since the app never loads, `window.Debug` (which the library waits for) never becomes available, causing a `TimeoutError: Waiting failed: 30000ms exceeded` crash.
  - **Fix**: Added a `postinstall` script (`scripts/postinstall.js`) that patches the user agent in `@open-wa/wa-automate` to use a clean, modern Chrome UA string without the `WhatsApp/` prefix. This patch auto-applies on every `npm install`.

### Changed
- **Upgraded Puppeteer**: Upgraded from `puppeteer@23.11.1` (Chromium 131) to `puppeteer@24.x` (Chromium 148+) via npm `overrides` to ensure a modern browser is always used
- **Updated dev dependencies**:
  - `@types/node`: `^18.7.6` → `^22.0.0`
  - `typescript`: `^4.9.3` → `^5.7.0`
  - `ts-node`: `^10.9.1` → `^10.9.2`
  - `rimraf`: `^3.0.2` → `^6.0.0`
  - Removed `pkg` (deprecated, no longer maintained)
- **Node.js requirement**: Bumped minimum from Node.js 14 to Node.js 18

### Added
- `scripts/postinstall.js` — Automatic UA patch that survives `npm install`
- `CHANGELOG.md` — This file
- npm `overrides` in `package.json` to force `puppeteer` and `puppeteer-core` to v24

## [1.0.0] - Initial Release

### Features
- WhatsApp Web integration via `@open-wa/wa-automate`
- Phone number-based folder organization
- Category system (4x6 Photo, A4 Frame, Polaroid, Banner)
- Multi-media support (images, videos, documents)
- HEIC/HEIF to JPG auto-conversion
- Automatic photo counting per folder
- Authorization system for allowed phone numbers
- Configurable bot messages and responses
