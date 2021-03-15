# Sample contents of Dockerfile
# Stage 1
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source

# caches restore result by copying csproj file separately
COPY *.csproj .
RUN dotnet restore

# copies the rest of your code
COPY . .
RUN dotnet publish --output /app/ --configuration Release

# Stage 2
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
ARG BUILD_VERSION=0.0.0
ENV APPLICATION_VERSION=$BUILD_VERSION

WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "EventHorizon.Identity.AuthServer.dll"]
