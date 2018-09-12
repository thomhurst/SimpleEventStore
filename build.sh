docker build -t simpleeventstore-build . \
&& docker run --rm --name simpleeventstore-build simpleeventstore-build sh /build/build.sh