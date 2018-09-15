Param(
    [string]$BUILD_VERSION = "1.0.0",
    [string]$COSMOS_URI = "https://localhost:8081/",
    [string]$COSMOS_AUTHKEY = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
    [string]$COSMOS_CONSISTENCYLEVEL = "BoundedStaleness",    
    [string]$NUGET_SOURCE,
    [string]$NUGET_KEY
)

docker build -t simpleeventstore-build . 

if($?)
{
    docker run --rm --name simpleeventstore-build `
    -e COSMOS_URI="$COSMOS_URI" `
    -e COSMOS_AUTHKEY="$COSMOS_AUTHKEY" `
    -e COSMOS_CONSISTENCYLEVEL="$COSMOS_CONSISTENCYLEVEL" `
    -e NUGET_SOURCE="$NUGET_SOURCE" `
    -e NUGET_KEY="$NUGET_KEY" `
    -e BUILD_VERSION="$BUILD_VERSION" `
    simpleeventstore-build sh /build/build.sh
}