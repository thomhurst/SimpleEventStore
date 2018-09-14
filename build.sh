BUILD_VERSION=${1:-"1.0.0"}
COSMOS_URI=${2:-"https://localhost:8081/"}
COSMOS_AUTHKEY=${3:-"C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="}
COSMOS_CONSISTENCYLEVEL=${4:-"BoundedStaleness"}
NUGET_SOURCE=$5
NUGET_KEY=$6

docker build -t simpleeventstore-build . && \
docker run --rm --name simpleeventstore-build \
-e COSMOS_URI="$COSMOS_URI" \
-e COSMOS_AUTHKEY="$COSMOS_AUTHKEY" \
-e COSMOS_CONSISTENCYLEVEL="$COSMOS_CONSISTENCYLEVEL" \
-e NUGET_SOURCE="$NUGET_SOURCE" \
-e NUGET_KEY="$NUGET_KEY" \
-e BUILD_VERSION="$BUILD_VERSION" \
simpleeventstore-build sh /build/build.sh