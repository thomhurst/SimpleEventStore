dotnet build ../src/SimpleEventStore/SimpleEventStore.sln -c Release /p:Version=$BUILD_VERSION && \

dotnet test ../src/SimpleEventStore/SimpleEventStore.Tests/SimpleEventStore.Tests.csproj -c Release --no-build && \
dotnet test ../src/SimpleEventStore/SimpleEventStore.AzureDocumentDb.Tests/SimpleEventStore.AzureDocumentDb.Tests.csproj && \

dotnet pack ../src/SimpleEventStore/SimpleEventStore/SimpleEventStore.csproj -c Release /p:Version=$BUILD_VERSION -o /packages --no-build && \
dotnet pack ../src/SimpleEventStore/SimpleEventStore.AzureDocumentDb/SimpleEventStore.AzureDocumentDb.csproj -c Release /p:Version=$BUILD_VERSION -o /packages --no-build && \

for f in /packages/*.nupkg; do
    dotnet nuget push $f -s $NUGET_SOURCE -k $NUGET_KEY
  echo "File -> $f"
done