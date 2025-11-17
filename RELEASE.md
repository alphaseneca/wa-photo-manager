# Quick Release Guide

## 🎯 One-Command Release

### Windows

```powershell
$version = "1.0.1"
.\scripts\prepare-release.bat $version
git tag -a "v$version" -m "Release v$version"
git push origin "v$version"
```

### macOS/Linux

```bash
VERSION="1.0.1"
chmod +x scripts/prepare-release.sh
./scripts/prepare-release.sh $VERSION
git tag -a "v$VERSION" -m "Release v$VERSION"
git push origin "v$VERSION"
```

## 📋 Step-by-Step

### 1. Prepare

```bash
# Make sure everything is committed
git status

# Update version (optional)
npm version patch
```

### 2. Create Release Package (Optional - GitHub Actions does this)

```bash
# Windows
.\scripts\prepare-release.bat 1.0.1

# macOS/Linux
./scripts/prepare-release.sh 1.0.1
```

### 3. Tag and Push

```bash
git tag -a v1.0.1 -m "Release v1.0.1"
git push origin v1.0.1
```

### 4. Monitor

- Go to: https://github.com/alphaseneca/wa_photodownloader-bot/actions
- Watch the workflow complete

### 5. Download

- Go to: https://github.com/alphaseneca/wa_photodownloader-bot/releases
- Download your platform's package

## ✨ What You Get

Each release includes **three downloadable packages**:

- 📦 **photo-manager-windows.zip** - Windows executable + config template
- 📦 **photo-manager-macos.zip** - macOS executable + config template
- 📦 **photo-manager-linux.zip** - Linux executable + config template

All pre-configured and ready to use!

## 🚀 User Experience

End users simply:

1. Download their OS's package
2. Extract the zip
3. Edit `config.js`
4. Run the app
5. Done! ✅

---

**Next Release Command Ready?**

- Windows: `.\scripts\prepare-release.bat 1.0.2`
- macOS/Linux: `./scripts/prepare-release.sh 1.0.2`
