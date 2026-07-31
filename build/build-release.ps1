param(
    [Parameter(Mandatory = $true)]
    [string]$Version,
    [string]$Rid = "win-x64",
    [string]$Project = "Gamelistify/Gamelistify.csproj",
    [string]$OutputDir = "build/dist"
)

$Version = $Version -replace '^v', ''

$root = Split-Path -Parent $PSScriptRoot
$projectPath = Join-Path $root $Project
$dist = Join-Path $root $OutputDir
$publishDir = Join-Path $dist "publish-$Rid"
$zipName = "Gamelistify-v$Version-$Rid.zip"
$zipPath = Join-Path $dist $zipName

if (-not (Test-Path -LiteralPath $projectPath)) {
    throw "Project not found: $projectPath"
}

if (-not (Test-Path -LiteralPath $dist)) {
    New-Item -ItemType Directory -Path $dist | Out-Null
}

$dotnet = (Get-Command "dotnet" -ErrorAction SilentlyContinue).Source
if ([string]::IsNullOrWhiteSpace($dotnet)) {
    $dotnet = "C:\Program Files\dotnet\dotnet.exe"
}

& $dotnet publish $projectPath `
    -c Release `
    -r $Rid `
    --self-contained true `
    -p:Version=$Version `
    -p:InformationalVersion=$Version `
    -o $publishDir

if ($LASTEXITCODE -ne 0) {
    throw "Publish failed"
}

if (Test-Path -LiteralPath $zipPath) {
    Remove-Item -LiteralPath $zipPath -Force
}

Add-Type -AssemblyName System.IO.Compression.FileSystem
[System.IO.Compression.ZipFile]::CreateFromDirectory($publishDir, $zipPath)

"Release created: $zipPath"
