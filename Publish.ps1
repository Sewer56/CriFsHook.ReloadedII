# Project Output Paths
$CrifsHookOutputPath = "CriFsHook.ReloadedII/bin"
$publishDirectory = "Publish"

if ([System.IO.Directory]::Exists($publishDirectory)) {
	Get-ChildItem $publishDirectory -Include * -Recurse | Remove-Item -Force -Recurse
}

# Build
dotnet clean CriFsHook.ReloadedII.sln
dotnet build -c Release CriFsHook.ReloadedII.sln

# Cleanup
Get-ChildItem $CrifsHookOutputPath -Include *.pdb -Recurse | Remove-Item -Force -Recurse
Get-ChildItem $CrifsHookOutputPath -Include *.xml -Recurse | Remove-Item -Force -Recurse

# Make compressed directory
if (![System.IO.Directory]::Exists($publishDirectory)) {
    New-Item $publishDirectory -ItemType Directory
}

# Compress
Add-Type -A System.IO.Compression.FileSystem
[IO.Compression.ZipFile]::CreateFromDirectory( $CrifsHookOutputPath + '/Release', 'Publish/CriFsHook.zip')