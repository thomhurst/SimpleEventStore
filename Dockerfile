FROM microsoft/dotnet:2.1-sdk-alpine AS build

WORKDIR /build
COPY ./build/build.sh ./build.sh
RUN ["chmod", "-x", "build.sh"]

WORKDIR /src
COPY ./src/SimpleEventStore/SimpleEventStore.sln ./
COPY ./src/SimpleEventStore/SimpleEventStore/SimpleEventStore.csproj ./SimpleEventStore/
COPY ./src/SimpleEventStore/SimpleEventStore.Tests/SimpleEventStore.Tests.csproj ./SimpleEventStore.Tests/
COPY ./src/SimpleEventStore/SimpleEventStore.AzureDocumentDb/SimpleEventStore.AzureDocumentDb.csproj ./SimpleEventStore.AzureDocumentDb/
COPY ./src/SimpleEventStore/SimpleEventStore.AzureDocumentDb.Tests/SimpleEventStore.AzureDocumentDb.Tests.csproj ./SimpleEventStore.AzureDocumentDb.Tests/
RUN dotnet restore
COPY ./src/SimpleEventStore ./