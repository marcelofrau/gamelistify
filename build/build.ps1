param(
    [string]$Configuration = "Debug"
)

$root = Split-Path -Parent $PSScriptRoot
$solution = Join-Path $root "Gamelistify.sln"

if (-not (Test-Path -LiteralPath $solution)) {
    throw "Solution not found: $solution"
}

& "C:\Program Files\dotnet\dotnet.exe" build $solution -c $Configuration
