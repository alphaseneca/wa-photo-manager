# Windows Packaging Script for WhatsApp Photo Manager
# Run this script to compile and assemble the standalone package

$ErrorActionPreference = "Stop"

$workspaceDir = (Get-Item .).FullName
$buildDir = Join-Path $workspaceDir "out-build"
$appDestDir = Join-Path $buildDir "whatsapp-photo-manager"
$nodeModulesDest = Join-Path $appDestDir "app\node_modules"

Write-Host "==============================================" -ForegroundColor Cyan
Write-Host "   BUILDING STANDALONE WINDOWS BOT PACKAGE" -ForegroundColor Cyan
Write-Host "==============================================" -ForegroundColor Cyan

# 1. Create build folders if they do not exist
if (-not (Test-Path $appDestDir)) {
    New-Item -ItemType Directory -Force -Path $appDestDir | Out-Null
}
if (-not (Test-Path (Join-Path $appDestDir "app"))) {
    New-Item -ItemType Directory -Force -Path (Join-Path $appDestDir "app") | Out-Null
}

# 2. Compile TypeScript
Write-Host "[2/7] Compiling TypeScript source files..." -ForegroundColor Yellow
npm run build

# 3. Download Portable Node.js Executable (if not already downloaded)
$nodeDest = Join-Path $appDestDir "node.exe"
if (-not (Test-Path $nodeDest)) {
    Write-Host "[3/7] Downloading portable node.exe (v24.21.0)..." -ForegroundColor Yellow
    $nodeUrl = "https://nodejs.org/dist/v24.21.0/win-x64/node.exe"
    Invoke-WebRequest -Uri $nodeUrl -OutFile $nodeDest
    Write-Host "[+] node.exe downloaded successfully." -ForegroundColor Green
} else {
    Write-Host "[3/7] Portable node.exe already exists, skipping download." -ForegroundColor Green
}

# 4. Download Google Chrome via Puppeteer CLI (if not already bundled)
$chromeDest = Join-Path $appDestDir "chrome"
if (-not (Test-Path $chromeDest)) {
    Write-Host "[4/7] Downloading Chrome browser dependency (this may take a minute)..." -ForegroundColor Yellow
    npx --yes puppeteer browsers install chrome --path $chromeDest
    Write-Host "[+] Chrome downloaded and bundled successfully." -ForegroundColor Green
} else {
    Write-Host "[4/7] Chrome browser already bundled, skipping download." -ForegroundColor Green
}

# 5. Compile C# GUI Launcher
Write-Host "[5/7] Compiling C# GUI Launcher..." -ForegroundColor Yellow
$csc = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
if (-not (Test-Path $csc)) {
    throw "C# compiler csc.exe not found. Ensure .NET Framework is installed."
}
$launcherSource = Join-Path $workspaceDir "scripts\Launcher.cs"
$launcherDest = Join-Path $appDestDir "whatsapp-photo-manager.exe"
& $csc /out:$launcherDest /target:winexe "/win32icon:$(Join-Path $workspaceDir 'assets\app-logo.ico')" $launcherSource
Write-Host "[+] whatsapp-photo-manager.exe compiled successfully." -ForegroundColor Green

# 5b. Compile Windows 11 wmic.exe compatibility shim
$wmicSource = Join-Path $workspaceDir "scripts\wmic.cs"
$wmicDest = Join-Path $appDestDir "wmic.exe"
& $csc /r:System.Management.dll /out:$wmicDest /target:exe $wmicSource
Write-Host "[+] wmic.exe compatibility shim compiled successfully." -ForegroundColor Green

# 6. Copy Application Files (overwrite existing)
if (Test-Path (Join-Path $appDestDir "app\dist")) {
    Remove-Item -Path (Join-Path $appDestDir "app\dist") -Recurse -Force
}
Copy-Item -Path (Join-Path $workspaceDir "dist") -Destination (Join-Path $appDestDir "app") -Recurse -Force
Copy-Item -Path (Join-Path $workspaceDir "package.json") -Destination (Join-Path $appDestDir "app\package.json") -Force
Copy-Item -Path (Join-Path $workspaceDir "config.js") -Destination (Join-Path $appDestDir "config.js") -Force
Copy-Item -Path (Join-Path $workspaceDir "README.md") -Destination (Join-Path $appDestDir "README.md") -Force
Copy-Item -Path (Join-Path $workspaceDir "assets\app-logo.ico") -Destination (Join-Path $appDestDir "app-logo.ico") -Force
Copy-Item -Path (Join-Path $workspaceDir "assets\app-logo.png") -Destination (Join-Path $appDestDir "app-logo.png") -Force
# Create empty downloads folder if missing
if (-not (Test-Path (Join-Path $appDestDir "downloads"))) {
    New-Item -ItemType Directory -Force -Path (Join-Path $appDestDir "downloads") | Out-Null
}
# Clean up any residual test configs, logs, or sessions from the package
Remove-Item -Path (Join-Path $appDestDir "config.json") -ErrorAction SilentlyContinue
Remove-Item -Path (Join-Path $appDestDir "*.log") -ErrorAction SilentlyContinue
Remove-Item -Path (Join-Path $appDestDir "*.data.json") -ErrorAction SilentlyContinue
Remove-Item -Path (Join-Path $appDestDir "qr_code*.png") -ErrorAction SilentlyContinue
Remove-Item -Path (Join-Path $appDestDir "_IGNORE_*") -Recurse -Force -ErrorAction SilentlyContinue

Write-Host "[+] App files copied." -ForegroundColor Green

# 7. Copy Node Modules incrementally using robocopy
Write-Host "[7/7] Copying/updating node_modules..." -ForegroundColor Yellow
$robocopySource = Join-Path $workspaceDir "node_modules"
# Use robocopy to sync only differences, ignoring locked files if any (R:0, W:0)
robocopy $robocopySource $nodeModulesDest /E /xd .git /xd .github /NDL /NFL /NJH /NJS /R:0 /W:0 | Out-Null
if ($LASTEXITCODE -ge 8) {
    Write-Host "[-] Note: Robocopy completed with warnings (exit code $LASTEXITCODE), but proceeding." -ForegroundColor Yellow
} else {
    Write-Host "[+] node_modules synced successfully." -ForegroundColor Green
}

Write-Host ""
Write-Host "==============================================" -ForegroundColor Green
Write-Host " STANDALONE BOT PACKAGE READY FOR PACKAGING" -ForegroundColor Green
Write-Host " Location: $appDestDir" -ForegroundColor Green
Write-Host "==============================================" -ForegroundColor Green
