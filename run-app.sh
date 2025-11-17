#!/bin/bash
# ========================================
# WhatsApp Photo Manager - App Launcher
# ========================================

set -e

# Get the directory where this script is located
APP_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Check if config.js exists
if [ ! -f "$APP_DIR/config.js" ]; then
    echo ""
    echo "[ERROR] config.js not found in $APP_DIR"
    echo ""
    echo "Please make sure config.js is in the same directory as this script."
    echo ""
    exit 1
fi

# Check if the executable exists
EXE_NAME=$(basename "$0" .sh)
EXE_PATH="$APP_DIR/$EXE_NAME"

if [ ! -f "$EXE_PATH" ]; then
    echo ""
    echo "[ERROR] Executable '$EXE_NAME' not found in $APP_DIR"
    echo ""
    echo "Please ensure the executable is in the same directory as this script."
    echo ""
    exit 1
fi

# Make executable if not already
if [ ! -x "$EXE_PATH" ]; then
    chmod +x "$EXE_PATH"
fi

# Run the application
echo ""
echo "======================================="
echo "   WhatsApp Photo Manager"
echo "======================================="
echo ""
echo "[*] Starting Photo Manager..."
echo "[*] Press Ctrl+C to stop the application"
echo ""

cd "$APP_DIR"
"$EXE_PATH"

EXIT_CODE=$?
if [ $EXIT_CODE -ne 0 ]; then
    echo ""
    echo "[ERROR] Application exited with code: $EXIT_CODE"
    echo ""
    exit $EXIT_CODE
fi
