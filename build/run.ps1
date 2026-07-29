param(
    [string]$Project = "Gamelistify/Gamelistify.csproj"
)

$root = Split-Path -Parent $PSScriptRoot
$projectPath = Join-Path $root $Project

if (-not (Test-Path -LiteralPath $projectPath)) {
    throw "Project not found: $projectPath"
}

& "C:\Program Files\dotnet\dotnet.exe" run --project $projectPath
