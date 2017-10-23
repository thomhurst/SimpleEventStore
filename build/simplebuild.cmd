REM TODO: Simplify the script by running in a container with full .NET Framework & .NET Core SDKs
dotnet restore "../src/SimpleEventStore/SimpleEventStore.sln"
dotnet build "../src/SimpleEventStore/SimpleEventStore" -c "Release" -f "netstandard1.6" --no-incremental
dotnet build "../src/SimpleEventStore/SimpleEventStore.AzureDocumentDb" -c "Release" -f "netstandard1.6" --no-incremental
dotnet build "../src/SimpleEventStore/SimpleEventStore.Tests" -c "Release" -f "netcoreapp2.0" --no-incremental
dotnet build "../src/SimpleEventStore/SimpleEventStore.AzureDocumentDb.Tests" -c "Release" -f "netcoreapp2.0" --no-incremental