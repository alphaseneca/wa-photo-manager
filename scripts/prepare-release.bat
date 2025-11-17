@echo off
REM ========================================
REM Prepare Release Package - Windows
REM ========================================

setlocal enabledelayedexpansion

set VERSION=%1
if "%VERSION%"=="" set VERSION=dev

set OUTPUT_DIR=release-output

echo.
echo =========================================
echo   Photo Manager - Release Preparation
echo =========================================
echo.
echo [*] Version: %VERSION%
echo [*] Output: %OUTPUT_DIR%
echo.

REM Clean and create output directory
if exist "%OUTPUT_DIR%" rmdir /s /q "%OUTPUT_DIR%"
mkdir "%OUTPUT_DIR%\photo-manager-windows"

REM Build
echo [*] Building...
call npm run build
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Build failed
    exit /b 1
)

REM Build Windows
echo [*] Building Windows executable...
call npm run pkg
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Windows build failed
    exit /b 1
)

REM Prepare Windows package
echo [*] Preparing Windows package...
copy photo-manager.exe "%OUTPUT_DIR%\photo-manager-windows\"
copy config.js "%OUTPUT_DIR%\photo-manager-windows\"
copy README.md "%OUTPUT_DIR%\photo-manager-windows\"
copy PACKAGING.md "%OUTPUT_DIR%\photo-manager-windows\"
copy run-app.bat "%OUTPUT_DIR%\photo-manager-windows\"
mkdir "%OUTPUT_DIR%\photo-manager-windows\downloads"
type nul > "%OUTPUT_DIR%\photo-manager-windows\downloads\.gitkeep"

REM Create zip using PowerShell
echo [*] Creating distribution package...
powershell -Command "Compress-Archive -Path '%OUTPUT_DIR%\photo-manager-windows' -DestinationPath '%OUTPUT_DIR%\photo-manager-windows-%VERSION%.zip' -Force"

echo.
echo =========================================
echo   [OK] Release package ready!
echo =========================================
echo.
echo [+] Package created:
echo     %OUTPUT_DIR%\photo-manager-windows-%VERSION%.zip
echo.
echo [+] Recommended next steps:
echo     1. Test the package
echo     2. Tag release: git tag -a v%VERSION% -m "Release %VERSION%"
echo     3. Push tags: git push origin v%VERSION%
echo     4. GitHub Actions will create releases automatically
echo.
pause
