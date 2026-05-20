# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base

RUN apt-get update && apt-get install -y ca-certificates
RUN update-ca-certificates

WORKDIR /app

USER $APP_UID

EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PMS.API/PMS.API.csproj", "PMS.API/"]
COPY ["PMS.Infrastructure/PMS.Infrastructure.csproj", "PMS.Infrastructure/"]
COPY ["PMS.Domain/PMS.Domain.csproj", "PMS.Domain/"]
COPY ["PMS.Application/PMS.Application.csproj", "PMS.Application/"]
RUN dotnet restore "./PMS.API/PMS.API.csproj"
COPY . .
WORKDIR "/src/PMS.API"
RUN dotnet build "./PMS.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PMS.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PMS.API.dll"]