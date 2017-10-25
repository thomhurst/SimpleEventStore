#TODO: Simplify the script by running in a container with full .NET Framework & .NET Core SDKs

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
dotnet build "../src/SimpleEventStore/SimpleEventStore" -c "Release" -f "netstandard1.6"
if($LastExitCode -ne 0) { exit 1 }
dotnet build "../src/SimpleEventStore/SimpleEventStore.AzureDocumentDb" -c "Release" -f "netstandard1.6"
if($LastExitCode -ne 0) { exit 1 }

#Test
Write-Host "Running unit tests"
dotnet test "../src/SimpleEventStore/SimpleEventStore.Tests" -c "Release" -f "netcoreapp2.0"
if($LastExitCode -ne 0) { exit 1 }
dotnet test "../src/SimpleEventStore/SimpleEventStore.AzureDocumentDb.Tests" -c "Release" -f "netcoreapp2.0"
if($LastExitCode -ne 0) { exit 1 }

#Package - will work once TODO point is covered
#Write-Host "Packaging"
#mkdir "./nuget" -Force
#dotnet pack "../src/SimpleEventStore/SimpleEventStore" -c "Release" -o "./nuget" /p:BuildVersion="$buildVersion" --no-build
#dotnet pack "../src/SimpleEventStore/SimpleEventStore.AzureDocumentDb" -c "Release" -o "./nuget" /p:BuildVersion="$buildVersion" --no-build