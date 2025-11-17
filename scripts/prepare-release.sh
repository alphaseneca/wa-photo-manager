#!/bin/bash
# ========================================
# Prepare Release Package - macOS/Linux
# ========================================

set -e

VERSION=${1:-"dev"}
OUTPUT_DIR="release-output"

echo "========================================="
echo "  Photo Manager - Release Preparation"
echo "========================================="
echo ""
echo "[*] Version: $VERSION"
echo "[*] Output: $OUTPUT_DIR"
echo ""

# Clean and create output directory
rm -rf "$OUTPUT_DIR"
mkdir -p "$OUTPUT_DIR/photo-manager-macos"
mkdir -p "$OUTPUT_DIR/photo-manager-linux"

# Build
echo "[*] Building..."
npm run build

# Build macOS
echo "[*] Building macOS executable..."
npm run pkg:macos

# Build Linux
echo "[*] Building Linux executable..."
npm run pkg:linux

# Prepare macOS package
echo "[*] Preparing macOS package..."
cp photo-manager-macos "$OUTPUT_DIR/photo-manager-macos/"
cp config.js "$OUTPUT_DIR/photo-manager-macos/"
cp README.md "$OUTPUT_DIR/photo-manager-macos/"
cp PACKAGING.md "$OUTPUT_DIR/photo-manager-macos/"
cp run-app.sh "$OUTPUT_DIR/photo-manager-macos/" 2>/dev/null || true
mkdir -p "$OUTPUT_DIR/photo-manager-macos/downloads"
touch "$OUTPUT_DIR/photo-manager-macos/downloads/.gitkeep"
chmod +x "$OUTPUT_DIR/photo-manager-macos/photo-manager-macos"

# Prepare Linux package
echo "[*] Preparing Linux package..."
cp photo-manager-linux "$OUTPUT_DIR/photo-manager-linux/"
cp config.js "$OUTPUT_DIR/photo-manager-linux/"
cp README.md "$OUTPUT_DIR/photo-manager-linux/"
cp PACKAGING.md "$OUTPUT_DIR/photo-manager-linux/"
cp run-app.sh "$OUTPUT_DIR/photo-manager-linux/" 2>/dev/null || true
mkdir -p "$OUTPUT_DIR/photo-manager-linux/downloads"
touch "$OUTPUT_DIR/photo-manager-linux/downloads/.gitkeep"
chmod +x "$OUTPUT_DIR/photo-manager-linux/photo-manager-linux"

# Create zips
echo "[*] Creating distribution packages..."
cd "$OUTPUT_DIR"
zip -r "photo-manager-macos-$VERSION.zip" photo-manager-macos/
zip -r "photo-manager-linux-$VERSION.zip" photo-manager-linux/
cd ..

echo ""
echo "========================================="
echo "  ✅ Release packages ready!"
echo "========================================="
echo ""
echo "[+] Packages created:"
echo "    • $OUTPUT_DIR/photo-manager-macos-$VERSION.zip"
echo "    • $OUTPUT_DIR/photo-manager-linux-$VERSION.zip"
echo ""
echo "[+] Recommended next steps:"
echo "    1. Test the packages on target systems"
echo "    2. Tag release: git tag -a v$VERSION -m \"Release $VERSION\""
echo "    3. Push tags: git push origin v$VERSION"
echo "    4. GitHub Actions will create releases automatically"
echo ""
