# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8078
EXPOSE 8079


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Gateway/DigitalLearningPlatform.ApiGateway/DigitalLearningPlatform.ApiGateway.csproj", "src/Gateway/DigitalLearningPlatform.ApiGateway/"]
RUN dotnet restore "./src/Gateway/DigitalLearningPlatform.ApiGateway/DigitalLearningPlatform.ApiGateway.csproj"
COPY . .
WORKDIR "/src/src/Gateway/DigitalLearningPlatform.ApiGateway"
RUN dotnet build "./DigitalLearningPlatform.ApiGateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DigitalLearningPlatform.ApiGateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app

USER root
COPY DevCertificates/rootCA.pem /usr/local/share/ca-certificates/rootCA.crt
RUN update-ca-certificates
USER $APP_UID

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DigitalLearningPlatform.ApiGateway.dll"]