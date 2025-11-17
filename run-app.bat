@echo off
REM ========================================
REM WhatsApp Photo Manager - App Launcher
REM ========================================

setlocal enabledelayedexpansion

REM Get the directory where this script is located
set APP_DIR=%~dp0

REM Check if config.js exists in the current directory
if not exist "%APP_DIR%config.js" (
    echo.
    echo [ERROR] config.js not found in %APP_DIR%
    echo.
    echo Please make sure config.js is in the same directory as this script.
    echo.
    pause
    exit /b 1
)

REM Check if the executable exists
if not exist "%APP_DIR%photo-manager.exe" (
    echo.
    echo [ERROR] photo-manager.exe not found in %APP_DIR%
    echo.
    echo Please run 'build-executable.bat' first to create the executable.
    echo.
    pause
    exit /b 1
)

REM Run the application
echo.
echo =======================================
echo   WhatsApp Photo Manager
echo =======================================
echo.
echo [*] Starting Photo Manager...
echo [*] Press Ctrl+C to stop the application
echo.

REM Change to app directory and run
cd /d "%APP_DIR%"
"%APP_DIR%photo-manager.exe"

REM Check if it exited with an error
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERROR] Application exited with error code: %ERRORLEVEL%
    echo.
    pause
)
