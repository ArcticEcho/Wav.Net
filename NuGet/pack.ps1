$root = (split-path -parent $MyInvocation.MyCommand.Definition) + '\..'
$version = [System.Reflection.Assembly]::LoadFile("$root\Wav.Net\bin\Release\Wav.Net.dll").GetName().Version
$versionStr = "{0}.{1}.{2}-beta" -f ($version.Major, $version.Minor, $version.Revision)

Write-Host "Setting .nuspec version tag to $versionStr"

$content = (Get-Content $root\nuget\Wav.Net.nuspec) 
$content = $content -replace '\$version\$',$versionStr

$content | Out-File $root\nuget\Wav.Net.compiled.nuspec

& $root\nuget\NuGet.exe pack $root\nuget\Wav.Net.compiled.nuspec