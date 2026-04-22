param(
    [string]$Configuration = "Debug"
)

$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
$appProject = Join-Path $root "FlightTrackerSystem\FlightTrackerSystem.csproj"

# Stop Avalonia designer hosts that commonly lock bin output during rebuild.
Get-CimInstance Win32_Process |
    Where-Object { $_.Name -eq "dotnet.exe" -and $_.CommandLine -match "Avalonia.Designer.HostApp.dll" } |
    ForEach-Object { Stop-Process -Id $_.ProcessId -Force -ErrorAction SilentlyContinue }

dotnet build-server shutdown | Out-Null

dotnet build $appProject -c $Configuration

