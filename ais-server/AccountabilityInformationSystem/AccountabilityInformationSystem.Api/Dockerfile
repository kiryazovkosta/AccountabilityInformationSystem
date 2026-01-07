FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["AccountabilityInformationSystem.Api/AccountabilityInformationSystem.Api.csproj", "AccountabilityInformationSystem.Api/"]
RUN dotnet restore "./AccountabilityInformationSystem.Api/AccountabilityInformationSystem.Api.csproj"
COPY . .
WORKDIR "/src/AccountabilityInformationSystem.Api"
RUN dotnet build "./AccountabilityInformationSystem.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AccountabilityInformationSystem.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AccountabilityInformationSystem.Api.dll"]