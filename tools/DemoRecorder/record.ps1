# Records the whole product demo in one take.
#
#   powershell -File tools/DemoRecorder/record.ps1 [-OutDir docs/demo]
#
# Starts the API (as the teacher), Tutor.Web and the Account Holder Portal, runs the recorder,
# and when the recorder asks for it (switch-user.request) restarts the API as the parent so the
# statement can be shown. Everything it started is stopped at the end. Run from the repo root
# after `dotnet build Tutor.slnx` and `dotnet build tools/DemoRecorder`.
param(
    [string]$OutDir = 'docs/demo',
    [string]$ParentEmail = 'thandi.mokoena@example.com'
)
$ErrorActionPreference = 'Stop'
$root = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$OutDir = Join-Path $root $OutDir
New-Item -ItemType Directory -Force $OutDir | Out-Null
$logDir = Join-Path $env:TEMP 'DemoRecorder'
New-Item -ItemType Directory -Force $logDir | Out-Null

# Each app's console output goes to %TEMP%\DemoRecorder\<project>.log so the recorder's own output stays readable.
function Start-App([string]$project, [hashtable]$env) {
    $log = Join-Path $logDir "$project.log"
    $psi = New-Object System.Diagnostics.ProcessStartInfo
    $psi.FileName = 'cmd.exe'
    $psi.Arguments = "/c dotnet run --project $project --no-build > `"$log`" 2>&1"
    $psi.WorkingDirectory = $root
    $psi.UseShellExecute = $false
    $psi.CreateNoWindow = $true
    foreach ($k in $env.Keys) { $psi.Environment[$k] = $env[$k] }
    [System.Diagnostics.Process]::Start($psi)
}

function Stop-Tree([System.Diagnostics.Process]$p) {
    if ($p -and -not $p.HasExited) { & taskkill /PID $p.Id /T /F 2>&1 | Out-Null }
}

function Wait-Url([string]$url, [int]$seconds = 90) {
    $until = (Get-Date).AddSeconds($seconds)
    do {
        $code = & curl.exe -k -s -o NUL -w '%{http_code}' $url
        if ($code -and $code -ne '000') { return }
        Start-Sleep -Milliseconds 500
    } while ((Get-Date) -lt $until)
    throw "Timed out waiting for $url"
}

$api = $null; $web = $null; $portal = $null
try {
    Write-Host 'Starting the apps...'
    $api    = Start-App 'Tutor.Api' @{}
    $web    = Start-App 'Tutor.Web' @{}
    $portal = Start-App 'Tutor.AccountHolderPortal' @{}
    Wait-Url 'https://localhost:64100/Teacher'
    Wait-Url 'http://localhost:64315/'
    Wait-Url 'http://localhost:57350/'

    $request = Join-Path $OutDir 'switch-user.request'
    $done    = Join-Path $OutDir 'switch-user.done'
    Remove-Item $request, $done -ErrorAction SilentlyContinue

    Write-Host 'Recording...'
    $recorder = Start-Process dotnet -ArgumentList "run --project tools/DemoRecorder --no-build -- all `"$OutDir`"" `
        -WorkingDirectory $root -NoNewWindow -PassThru

    # Serve the recorder's request to switch the API user, then wait for it to finish.
    while (-not $recorder.HasExited) {
        if (Test-Path $request) {
            Write-Host "Restarting the API as $ParentEmail..."
            Stop-Tree $api
            $api = Start-App 'Tutor.Api' @{ 'Auth__BypassEmail' = $ParentEmail }
            Wait-Url 'https://localhost:64100/Teacher'
            Remove-Item $request
            New-Item -ItemType File $done | Out-Null
        }
        Start-Sleep -Milliseconds 500
    }
    if ($recorder.ExitCode -ne 0) { throw "Recorder exited with code $($recorder.ExitCode)" }
    Write-Host "Done: $(Join-Path $OutDir 'tutor-demo.webm')"
}
finally {
    Stop-Tree $api; Stop-Tree $web; Stop-Tree $portal
    $global:LASTEXITCODE = 0   # taskkill's exit code must not become the script's
}
