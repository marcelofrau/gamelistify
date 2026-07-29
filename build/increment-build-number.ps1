$root = Split-Path -Parent $PSScriptRoot
$propsPath = Join-Path $root "Directory.Build.props"

if (-not (Test-Path -LiteralPath $propsPath)) {
    throw "Version props file not found: $propsPath"
}

[xml]$xml = Get-Content -LiteralPath $propsPath
$node = $xml.Project.PropertyGroup.BuildNumber

if ($null -eq $node) {
    throw "BuildNumber node not found in Directory.Build.props"
}

$current = [int]$node.InnerText
$next = $current + 1
$node.InnerText = $next.ToString()
$xml.Save($propsPath)

"BuildNumber incremented: $current -> $next"
