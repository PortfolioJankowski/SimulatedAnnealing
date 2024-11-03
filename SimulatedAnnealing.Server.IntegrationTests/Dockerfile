#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SimulatedAnnealing.Server.IntegrationTests/SimulatedAnnealing.Server.IntegrationTests.csproj", "SimulatedAnnealing.Server.IntegrationTests/"]
COPY ["SimulatedAnnealing.Server/SimulatedAnnealing.Server.csproj", "SimulatedAnnealing.Server/"]
RUN dotnet restore "./SimulatedAnnealing.Server.IntegrationTests/SimulatedAnnealing.Server.IntegrationTests.csproj"
COPY . .
WORKDIR "/src/SimulatedAnnealing.Server.IntegrationTests"
RUN dotnet build "./SimulatedAnnealing.Server.IntegrationTests.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SimulatedAnnealing.Server.IntegrationTests.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimulatedAnnealing.Server.IntegrationTests.dll"]