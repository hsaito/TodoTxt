# https://hub.docker.com/_/microsoft-dotnet-core
FROM mcr.microsoft.com/dotnet/core/sdk:latest AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY . .

WORKDIR /source
RUN dotnet build
RUN dotnet test
