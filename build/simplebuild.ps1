#TODO: Simplify the script by running in a container with full .NET Framework & .NET Core SDKs

#Variables
$docDbConfig = "../src/SimpleEventStore/SimpleEventStore.AzureDocumentDb.Tests/appsettings.json"
$buildVersion = $env:buildVersion
$uri = $env:uri
$authKey = $env:authKey
$consistencyLevel = $env:consistencyLevel

#Transform config
Write-Host "Transforming config"
$config = Get-Content $docDbConfig -raw | ConvertFrom-Json
$config.Uri = $uri
$config.AuthKey = $authKey
$config.ConsistencyLevel = $consistencyLevel
$config | ConvertTo-Json  | set-content $docDbConfig

#Restore
Write-Host "Restoring packages"
dotnet restore "../src/SimpleEventStore/SimpleEventStore.sln"

#Build
Write-Host "Building"
dotnet build "../src/SimpleEventStore/SimpleEventStore" -c "Release" -f "netstandard1.6"
dotnet build "../src/SimpleEventStore/SimpleEventStore.AzureDocumentDb" -c "Release" -f "netstandard1.6"

#Test
Write-Host "Running unit tests"
dotnet test "../src/SimpleEventStore/SimpleEventStore.Tests" -c "Release" -f "netcoreapp2.0"
dotnet test "../src/SimpleEventStore/SimpleEventStore.AzureDocumentDb.Tests" -c "Release" -f "netcoreapp2.0"

#Package - will work once TODO point is covered
#Write-Host "Packaging"
#mkdir "./nuget" -Force
#dotnet pack "../src/SimpleEventStore/SimpleEventStore" -c "Release" -o "./nuget" /p:BuildVersion="$buildVersion" --no-build
#dotnet pack "../src/SimpleEventStore/SimpleEventStore.AzureDocumentDb" -c "Release" -o "./nuget" /p:BuildVersion="$buildVersion" --no-build