#!/bin/bash

echo "========================================"
echo "    WHATSAPP PHOTO MANAGER"
echo "========================================"
echo ""
echo "Starting WhatsApp Photo Manager..."
echo ""

# Check if Node.js is installed
if ! command -v node &> /dev/null; then
    echo "ERROR: Node.js is not installed!"
    echo "Please install Node.js from https://nodejs.org/"
    exit 1
fi

# Check if dependencies are installed
if [ ! -d "node_modules" ]; then
    echo "Installing dependencies..."
    npm install
    echo ""
fi

# Start the bot
echo "Starting the bot..."
echo "Press Ctrl+C to stop the bot"
echo ""
npx ts-node src/index.ts 