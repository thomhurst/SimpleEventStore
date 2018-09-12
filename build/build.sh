dotnet build ../src/SimpleEventStore/SimpleEventStore.sln -c Release

# TODO: Need to determine the best way to populate the environment so we can run the Cosmos tests
dotnet test ../src/SimpleEventStore/SimpleEventStore.Tests/SimpleEventStore.Tests.csproj -c Release --no-build

dotnet pack ../src/SimpleEventStore/SimpleEventStore/SimpleEventStore.csproj -c Release --no-build