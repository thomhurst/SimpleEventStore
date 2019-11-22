Param(
    [string]$Uri = "https://localhost:8081/",
    [string]$AuthKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
    [string]$ConsistencyLevel = "BoundedStaleness",
    [string]$BuildVersion = "1.0.0",
    [string]$Configuration = "Release"
)

function Write-Stage([string]$name)
{
    Write-Host $('=' * 30) -ForegroundColor Green
    Write-Host $name -ForegroundColor Green
    Write-Host $('=' * 30) -ForegroundColor Green
}

$outputDir = "../../output";
Push-Location src\SimpleEventStore

Write-Stage "Building solution"
dotnet build -c $Configuration -p:BuildVersion=$BuildVersion

Write-Stage "Running tests"
$env:Uri = $Uri
$env:AuthKey = $AuthKey
$env:ConsistencyLevel = $ConsistencyLevel

dotnet test SimpleEventStore.Tests --no-build
dotnet test SimpleEventStore.AzureDocumentDb.Tests --no-build

Write-Stage "Creating nuget packages"
rm "../../output/*.nupkg"
dotnet pack SimpleEventStore -c $Configuration -o $outputDir -p:BuildVersion=$BuildVersion --no-build
dotnet pack SimpleEventStore.AzureDocumentDb.Tests -c $Configuration -o $outputDir  -p:BuildVersion=$BuildVersion --no-build

Pop-Location