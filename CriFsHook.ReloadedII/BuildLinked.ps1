# Set Working Directory
Split-Path $MyInvocation.MyCommand.Path | Push-Location
[Environment]::CurrentDirectory = $PWD

Remove-Item "$env:RELOADEDIIMODS/CriFsHook.ReloadedII/*" -Force -Recurse
dotnet publish "./CriFsHook.ReloadedII.csproj" -c Release -o "$env:RELOADEDIIMODS/CriFsHook.ReloadedII" /p:OutputPath="./bin/Release" /p:ReloadedILLink="true"

# Restore Working Directory
Pop-Location