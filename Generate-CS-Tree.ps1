$root = 'E:/BDC/Projects/SASZombieAssaultTD'
$output = 'E:/BDC/Projects/CS-Tree.md'

$files = Get-ChildItem -Path $root -Recurse -Filter *.cs

'```' | Out-File $output -Encoding UTF8

foreach ($file in $files) {

    # Normalize path separators
    $full = $file.FullName -replace '\\', '/'
    $rootNorm = $root -replace '\\', '/'

    # Remove root prefix
    $relative = $full.Substring($rootNorm.Length).TrimStart('/')

    # Split on forward slash
    $parts = $relative.Split('/')

    # Output with indentation
    $indent = ''
    foreach ($part in $parts) {
        "$indent- $part" | Out-File $output -Append -Encoding UTF8
        $indent += '  '
    }
}

'```' | Out-File $output -Append -Encoding UTF8