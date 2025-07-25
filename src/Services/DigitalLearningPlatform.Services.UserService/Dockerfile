# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/DigitalLearningPlatform.Services.UserService/DigitalLearningPlatform.Services.UserService.csproj", "src/Services/DigitalLearningPlatform.Services.UserService/"]
RUN dotnet restore "./src/Services/DigitalLearningPlatform.Services.UserService/DigitalLearningPlatform.Services.UserService.csproj"
COPY . .
WORKDIR "/src/src/Services/DigitalLearningPlatform.Services.UserService"
RUN dotnet build "./DigitalLearningPlatform.Services.UserService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DigitalLearningPlatform.Services.UserService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app

#Certificateion update shoud be based on environment. Enhancement/Prod/E3
#Root certifcate copied to docker container for validating the certificate the local generated certificate
USER root
COPY DevCertificates/rootCA.pem /usr/local/share/ca-certificates/rootCA.crt
RUN update-ca-certificates
USER $APP_UID

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DigitalLearningPlatform.Services.UserService.dll"]