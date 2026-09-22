# Changelog

All notable changes to this project will be documented in this file.

## [1.0.0] - 2026-09-22

### Initial Release

#### Features
- **WhatsApp Web Integration**: Connects to WhatsApp Web using modern Puppeteer (v24+) and an automated user-agent compatibility patch (`scripts/postinstall.js`).
- **Windows System Tray GUI Launcher**:
  - Background execution (`scripts/Launcher.cs` compiled into `whatsapp-photo-manager.exe`) without command window popups.
  - Interactive system tray icon with start/stop, settings, log viewer, and folder shortcut.
  - Live console logs viewer with real-time ANSI-color code stripping.
  - Automatic QR code pairing popup dialog that displays when login is required and closes when paired.
  - Settings dialog to edit authorized numbers, photo categories, and storage path without editing code.
- **Zero-Dependency Windows Installer**:
  - `scripts/build-package.ps1` bundles portable Node.js LTS v20 and standalone Chrome.
  - `scripts/installer.iss` compiles a single setup installer executable (`WhatsAppPhotoManager-Installer-1.0.0-x64.exe`).
- **Automated Media Organization**:
  - Organizes incoming images, videos, and documents into `downloads/<Category>/<PhoneNumber>/`.
  - Photo counting per folder with real-time feedback to users.
- **Image Processing**:
  - Automatic HEIC/HEIF conversion to JPEG via `heic-convert`.
  - Image optimization and standardization using Sharp.
- **Security & Authorization**:
  - Whitelist authorization system for allowed sender phone numbers.
  - Non-destructive configuration overrides supported via `config.json`.
