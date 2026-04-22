param(
    [string]$Configuration = "Debug"
)

$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
$testProject = Join-Path $root "Test\Test.csproj"
$artifactsRoot = Join-Path $root "artifacts\test"
$binPath = Join-Path $artifactsRoot "bin\"
$objPath = Join-Path $artifactsRoot "obj\"

# Stop Avalonia designer hosts that commonly lock app output during test restore/build.
Get-CimInstance Win32_Process |
    Where-Object { $_.Name -eq "dotnet.exe" -and $_.CommandLine -match "Avalonia.Designer.HostApp.dll" } |
    ForEach-Object { Stop-Process -Id $_.ProcessId -Force -ErrorAction SilentlyContinue }

dotnet build-server shutdown | Out-Null

dotnet test $testProject -c $Configuration `
    -p:BaseOutputPath=$binPath `
    -p:BaseIntermediateOutputPath=$objPath

