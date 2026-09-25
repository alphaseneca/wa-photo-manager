# Changelog

All notable changes to this project will be documented in this file.

## [1.1.0] - 2026-09-25

### Improvements & Fixes
- **Modern UI & Toast Notifications**: Replaced default Windows balloon tooltips with styled modern dark toasts, custom buttons, and smooth animations.
- **Enhanced QR Code Recognition**: Upgraded Sharp threshold and grayscale pipeline to render high-contrast black QR codes for faster scanning.
- **Single-Instance IPC Wakeup**: Restoring or activating an already-running tray instance now smoothly brings the primary window to the foreground via named events.
- **Robust Upgrade & Uninstall Routines**:
  - Installer purge routine terminates running instances and replaces stale code while preserving user session, configuration, and downloads.
  - Uninstaller includes interactive prompt to choose whether to retain or purge media downloads.
- **Windows 11 Compatibility**: Included standalone `wmic.exe` shim to maintain process inspection compatibility on modern Windows 11 builds.
- **Cross-Platform Test Runner**: Replaced shell-dependent globbing with `scripts/test-runner.js` supporting Node 18, 20, and 22 across Linux and Windows.

## [1.0.0] - 2026-09-22

### Initial Release

- **WhatsApp Web Integration**: Connects to WhatsApp Web using modern Puppeteer (v24+) and native user-agent configuration.
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
