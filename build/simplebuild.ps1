#Variables
$buildVersion = $env:buildVersion
$uri = $env:uri
$authKey = $env:authKey
$consistencyLevel = $env:consistencyLevel

#Restore
Write-Host "Restoring packages"
dotnet restore "../src/SimpleEventStore/SimpleEventStore.sln"
if($LastExitCode -ne 0) { exit 1 }

#Build
Write-Host "Building"
dotnet build "../src/SimpleEventStore/SimpleEventStore.sln" -c "Release"
if($LastExitCode -ne 0) { exit 1 }

#Test
Write-Host "Running unit tests"
dotnet test "../src/SimpleEventStore/SimpleEventStore.Tests" -c "Release" -f "netcoreapp2.0"
if($LastExitCode -ne 0) { exit 1 }
dotnet test "../src/SimpleEventStore/SimpleEventStore.AzureDocumentDb.Tests" -c "Release" -f "netcoreapp2.0"
if($LastExitCode -ne 0) { exit 1 }

#Package
Write-Host "Packaging"
mkdir "./nuget" -Force
dotnet pack "../src/SimpleEventStore/SimpleEventStore" -c "Release" -o "./nuget" /p:BuildVersion="$buildVersion" --no-build
dotnet pack "../src/SimpleEventStore/SimpleEventStore.AzureDocumentDb" -c "Release" -o "./nuget" /p:BuildVersion="$buildVersion" --no-build