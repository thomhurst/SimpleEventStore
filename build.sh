COSMOS_URI=$1
COSMOS_AUTHKEY=$2
COSMOS_CONSISTENCYLEVEL=$3
NUGET_SOURCE=$4
NUGET_KEY=$5
BUILD_VERSION=$6

docker build -t simpleeventstore-build . && \
docker run --rm --name simpleeventstore-build \
-e COSMOS_URI="$COSMOS_URI" \
-e COSMOS_AUTHKEY="$COSMOS_AUTHKEY" \
-e COSMOS_CONSISTENCYLEVEL="$COSMOS_CONSISTENCYLEVEL" \
-e NUGET_SOURCE="$NUGET_SOURCE" \
-e NUGET_KEY="$NUGET_KEY" \
-e BUILD_VERSION="$BUILD_VERSION" \
simpleeventstore-build sh /build/build.sh