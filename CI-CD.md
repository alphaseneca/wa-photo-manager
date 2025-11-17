# CI/CD Pipeline & Release Management

## Overview

This project includes a complete GitHub Actions CI/CD pipeline that automatically builds, packages, and releases the Photo Manager application across Windows, macOS, and Linux platforms.

## 🔄 CI/CD Workflow

### Trigger Points

The pipeline is triggered in two ways:

1. **Tag-based Release (Automatic)**

   ```bash
   git tag -a v1.0.0 -m "Release v1.0.0"
   git push origin v1.0.0
   ```

   When you push a tag matching `v*.*.*`, the full build and release pipeline runs.

2. **Manual Trigger**
   - Go to GitHub Actions → Build and Release → Run workflow
   - Useful for testing or rebuilding specific versions

### Pipeline Stages

```
┌─────────────────────────────────────┐
│  1. Build (Matrix - 3 OS)          │
│  ├─ Windows (photo-manager.exe)    │
│  ├─ macOS (photo-manager-macos)    │
│  └─ Linux (photo-manager-linux)    │
└────────────┬────────────────────────┘
             │
             ▼
┌─────────────────────────────────────┐
│  2. Package (Create distribution)   │
│  ├─ Add config.js template         │
│  ├─ Add run-app.bat / run-app.sh   │
│  ├─ Add documentation              │
│  └─ Create downloads/ folder       │
└────────────┬────────────────────────┘
             │
             ▼
┌─────────────────────────────────────┐
│  3. Create Artifacts                │
│  ├─ photo-manager-windows.zip       │
│  ├─ photo-manager-macos.zip         │
│  └─ photo-manager-linux.zip         │
└────────────┬────────────────────────┘
             │
             ▼
┌─────────────────────────────────────┐
│  4. GitHub Release                  │
│  ├─ Upload .zip files               │
│  ├─ Generate release notes          │
│  └─ Create downloadable releases    │
└─────────────────────────────────────┘
```

## 📦 Release Package Structure

Each release includes three platform-specific packages:

### Windows Package

```
photo-manager-windows/
├── photo-manager.exe          (Executable)
├── config.js                   (Configuration template)
├── run-app.bat                 (Launcher script)
├── README.md                   (Features & usage)
├── PACKAGING.md                (Setup & troubleshooting)
└── downloads/                  (Auto-created folder)
```

### macOS Package

```
photo-manager-macos/
├── photo-manager-macos         (Executable)
├── config.js                   (Configuration template)
├── run-app.sh                  (Launcher script)
├── README.md                   (Features & usage)
├── PACKAGING.md                (Setup & troubleshooting)
└── downloads/                  (Auto-created folder)
```

### Linux Package

```
photo-manager-linux/
├── photo-manager-linux         (Executable)
├── config.js                   (Configuration template)
├── run-app.sh                  (Launcher script)
├── README.md                   (Features & usage)
├── PACKAGING.md                (Setup & troubleshooting)
└── downloads/                  (Auto-created folder)
```

## 🚀 How to Release

### Step 1: Update Version (Optional)

```bash
npm version patch  # or minor, major
```

### Step 2: Create Release Tag

```bash
git tag -a v1.0.1 -m "Release v1.0.1"
git push origin v1.0.1
```

Or with automatic versioning:

```bash
npm version patch && git push --follow-tags
```

### Step 3: Monitor Build

- Go to GitHub → Actions → Build and Release
- Watch the workflow run on all three platforms
- Once complete, a GitHub Release is automatically created

### Step 4: Verify Release

- Go to GitHub → Releases
- Download and test each platform's package
- Verify all files are included

## 📋 File Structure

```
photo-manager/
├── .github/workflows/
│   └── build-and-release.yml    (CI/CD configuration)
├── scripts/
│   ├── prepare-release.bat      (Windows release prep)
│   └── prepare-release.sh       (macOS/Linux release prep)
├── src/
│   └── index.ts
├── config.js
├── package.json
├── PACKAGING.md
└── README.md
```

## 🛠️ Local Release Testing

### Windows

```bash
.\scripts\prepare-release.bat 1.0.1
# Creates: release-output/photo-manager-windows-1.0.1.zip
```

### macOS/Linux

```bash
chmod +x scripts/prepare-release.sh
./scripts/prepare-release.sh 1.0.1
# Creates: release-output/photo-manager-*.zip
```

## 📝 Workflow Configuration

The workflow file is at: `.github/workflows/build-and-release.yml`

### Key Configuration Points

**Node.js Version:**

```yaml
node-version: [18.x]
```

**Operating Systems:**

```yaml
os: [ubuntu-latest, windows-latest, macos-latest]
```

**Trigger Patterns:**

```yaml
tags:
  - "v*.*.*" # Semantic versioning
```

## 🔐 Secrets & Permissions

The workflow uses:

- `GITHUB_TOKEN` - Automatically provided by GitHub for creating releases
- No additional secrets required

## 📊 Build Matrix

The workflow builds on three platforms in parallel:

| OS      | Architecture | Executable          | Status |
| ------- | ------------ | ------------------- | ------ |
| Windows | x64          | photo-manager.exe   | ✅     |
| macOS   | x64 + ARM64  | photo-manager-macos | ✅     |
| Linux   | x64          | photo-manager-linux | ✅     |

### Parallel Execution Times

- Build time: ~5-15 minutes per platform
- Total pipeline: ~10-20 minutes (parallel runs)

## 🐛 Troubleshooting

### Build Fails on Specific Platform

**Check logs:**

1. Go to GitHub → Actions
2. Click failed workflow
3. Expand job logs
4. Check for dependency or compilation errors

**Common Issues:**

- Missing dependencies: Run `npm install` locally
- TypeScript errors: Run `npm run build` locally
- Pkg build errors: Verify Node.js version compatibility

### Release Not Created

**Verify:**

1. Tag format matches `v*.*.*` (e.g., `v1.0.0`)
2. All build jobs completed successfully
3. GITHUB_TOKEN has proper permissions (default is fine)

### Executables Not in Release

**Check:**

1. All platform builds completed
2. Artifact upload steps completed
3. No permission issues on artifact files

## 🎯 Advanced Usage

### Manual Build Trigger

1. Go to GitHub → Actions → Build and Release
2. Click "Run workflow"
3. Enter optional version number
4. Monitor progress

### Build Specific Platform Only

Edit `.github/workflows/build-and-release.yml`:

```yaml
strategy:
  matrix:
    os: [windows-latest] # Only Windows
```

### Custom Release Notes

Edit the "Create Release Notes" step in the workflow:

```yaml
- name: Create Release Notes
  run: |
    cat > RELEASE_NOTES.md << 'EOF'
    # Your custom notes here
    EOF
```

### Deploy to Additional Services

Add steps to the release job to deploy to:

- AWS S3
- Google Cloud Storage
- Custom server
- Package managers (chocolatey, brew, apt)

## 📈 Performance Optimization

### Caching Dependencies

Already implemented via `cache: 'npm'` in Node.js setup.

### Build Time Breakdown

- Setup (each job): ~2 min
- Install dependencies: ~1-3 min
- TypeScript compilation: ~1 min
- Build executable: ~2-5 min
- Package creation: ~1 min

### Optimize Further

```yaml
# Reuse compiled files across jobs
- name: Cache build output
  uses: actions/cache@v3
  with:
    path: dist/
    key: build-${{ github.sha }}
```

## 📚 References

- [GitHub Actions Documentation](https://docs.github.com/actions)
- [softprops/action-gh-release](https://github.com/softprops/action-gh-release)
- [Semantic Versioning](https://semver.org/)
- [Pkg Documentation](https://github.com/vercel/pkg)

## ✅ Release Checklist

Before releasing:

- [ ] Update version in package.json (optional)
- [ ] Test locally: `npm run dev`
- [ ] Build locally: `npm run pkg:all`
- [ ] Test executables on each platform
- [ ] Update CHANGELOG.md
- [ ] Create git tag: `git tag -a v1.0.0 -m "Release v1.0.0"`
- [ ] Push tag: `git push origin v1.0.0`
- [ ] Monitor GitHub Actions workflow
- [ ] Verify release was created on GitHub
- [ ] Download and test packages
- [ ] Share release link with users

## 🔗 Quick Links

- [Build and Release Workflow](.github/workflows/build-and-release.yml)
- [Release Scripts](scripts/)
- [GitHub Releases](../../releases)
- [GitHub Actions](../../actions)

---

**Last Updated:** 2025  
**Maintainer:** Photo Manager Team
