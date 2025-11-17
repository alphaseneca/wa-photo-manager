@echo off
echo ========================================
echo     WHATSAPP PHOTO MANAGER
echo ========================================
echo.
echo Starting WhatsApp Photo Manager...
echo.

REM Check if Node.js is installed
node --version >nul 2>&1
if errorlevel 1 (
    echo ERROR: Node.js is not installed!
    echo Please install Node.js from https://nodejs.org/
    pause
    exit /b 1
)

REM Check if dependencies are installed
if not exist "node_modules" (
    echo Installing dependencies...
    npm install
    echo.
)

REM Start the bot
echo Starting the bot...
echo Press Ctrl+C to stop the bot
echo.
npx ts-node src/index.ts
pause