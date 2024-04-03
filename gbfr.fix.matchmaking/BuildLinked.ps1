# Set Working Directory
Split-Path $MyInvocation.MyCommand.Path | Push-Location
[Environment]::CurrentDirectory = $PWD

Remove-Item "$env:RELOADEDIIMODS/gbfr.fix.matchmaking/*" -Force -Recurse
dotnet publish "./gbfr.fix.matchmaking.csproj" -c Release -o "$env:RELOADEDIIMODS/gbfr.fix.matchmaking" /p:OutputPath="./bin/Release" /p:ReloadedILLink="true"

# Restore Working Directory
Pop-Location