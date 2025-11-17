@echo off
REM ========================================
REM WhatsApp Photo Manager - Build Executable
REM ========================================

echo.
echo =======================================
echo   WhatsApp Photo Manager - Build
echo =======================================
echo.

REM Check if Node.js is installed
where node >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Node.js is not installed or not in PATH
    echo Please install Node.js from https://nodejs.org/
    echo.
    pause
    exit /b 1
)

echo [+] Node.js found: 
node --version

REM Install dependencies
echo.
echo [*] Installing dependencies...
call npm install
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Failed to install dependencies
    pause
    exit /b 1
)

REM Compile TypeScript
echo.
echo [*] Compiling TypeScript...
call npm run build
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] TypeScript compilation failed
    pause
    exit /b 1
)

REM Build executable
echo.
echo [*] Building standalone executable...
echo [!] This may take a few minutes...
call npm run pkg
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Failed to build executable
    pause
    exit /b 1
)

REM Success
echo.
echo =======================================
echo   Build Completed Successfully!
echo =======================================
echo.
echo [+] Executable created: photo-manager.exe
echo [+] Location: %cd%\photo-manager.exe
echo.
echo [*] Next steps:
echo    1. Copy photo-manager.exe to desired location
echo    2. Copy config.js to the same directory as photo-manager.exe
echo    3. Run photo-manager.exe
echo.
pause
