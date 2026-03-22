param(
    [string]$OutputFile = "codebase.md"
)

$root = Get-Location
$outputPath = Join-Path $root $OutputFile

Write-Host "Starting flatten..."

if (Test-Path $outputPath) {
    Remove-Item $outputPath
}

# Header
Add-Content -Path $outputPath -Value '# Flattened Codebase'
Add-Content -Path $outputPath -Value ''
Add-Content -Path $outputPath -Value ('Generated: ' + (Get-Date))
Add-Content -Path $outputPath -Value ''

# Collect files
$files = Get-ChildItem -Recurse -File -Include *.cs,*.razor,*.json,*.csproj |
Where-Object {
    $_.FullName -notlike "*\bin\*" -and
    $_.FullName -notlike "*\obj\*" -and
    $_.Name -notlike "*.Designer.cs" -and
    $_.Name -notlike "*.g.cs"
}

$total = $files.Count
Write-Host "Files to process: $total"

foreach ($file in $files) {

    $relativePath = $file.FullName.Replace($root.Path + "\", "")

    Add-Content -Path $outputPath -Value ''
    Add-Content -Path $outputPath -Value ('## File: ' + $relativePath)
    Add-Content -Path $outputPath -Value ''

    # Determine language for Markdown
    $ext = $file.Extension.TrimStart('.').ToLower()
    switch ($ext) {
        'cs' { $lang = 'csharp' }
        'razor' { $lang = 'razor' }
        'json' { $lang = 'json' }
        'csproj' { $lang = 'xml' }
        'xml' { $lang = 'xml' }
        default { $lang = '' }
    }

    # Start code block
    Add-Content -Path $outputPath -Value ('```' + $lang)

    # Read file content
    try {
        $content = Get-Content -Raw -LiteralPath $file.FullName
        Add-Content -Path $outputPath -Value $content
    }
    catch {
        Add-Content -Path $outputPath -Value '// ERROR READING FILE'
    }

    # End code block
    Add-Content -Path $outputPath -Value '```'
}

Write-Host "Done. Output written to $outputPath"