
# Set Working Directory
Split-Path $MyInvocation.MyCommand.Path | Push-Location
[Environment]::CurrentDirectory = $PWD

./Publish.ps1 -ProjectPath "CriFsHook.ReloadedII/CriFsHook.ReloadedII.csproj" `
              -PackageName "criware.filesystem.hook" `

Pop-Location