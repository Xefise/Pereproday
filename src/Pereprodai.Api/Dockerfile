FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["src/Pereprodai.Api/Pereprodai.Api.csproj", "src/Pereprodai.Api/"]
COPY ["src/Pereprodai.Shared/Pereprodai.Shared.csproj", "src/Pereprodai.Shared/"]
COPY ["src/Modules/Pereprodai.Catalog/Pereprodai.Catalog.csproj", "src/Modules/Pereprodai.Catalog/"]
RUN dotnet restore "src/Pereprodai.Api/Pereprodai.Api.csproj"
COPY . .
WORKDIR "/src/src/Pereprodai.Api"
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pereprodai.Api.dll"]
