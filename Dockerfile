# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Restore from project metadata first so dependency layers remain cacheable.
COPY Directory.Build.props ./
COPY BLL/BLL.csproj BLL/
COPY DAL/DAL.csproj DAL/
COPY Mystic-Journey-API/Mystic-Journey-API.csproj Mystic-Journey-API/
RUN dotnet restore Mystic-Journey-API/Mystic-Journey-API.csproj

COPY BLL/ BLL/
COPY DAL/ DAL/
COPY Mystic-Journey-API/ Mystic-Journey-API/
RUN dotnet build Mystic-Journey-API/Mystic-Journey-API.csproj \
    --configuration $BUILD_CONFIGURATION \
    --no-restore

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish Mystic-Journey-API/Mystic-Journey-API.csproj \
    --configuration $BUILD_CONFIGURATION \
    --output /app/publish \
    --no-build \
    /p:UseAppHost=false

# This target is used only by the one-shot Compose migration service.
FROM build AS migrate
RUN dotnet tool install --global dotnet-ef --version 8.0.30
ENV PATH="$PATH:/root/.dotnet/tools"
ENTRYPOINT ["dotnet", "ef"]

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

RUN apt-get update \
    && apt-get install --yes --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

COPY --from=publish --chown=$APP_UID:$APP_UID /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080 \
    DOTNET_EnableDiagnostics=0

EXPOSE 8080
USER $APP_UID

ENTRYPOINT ["dotnet", "Mystic-Journey-API.dll"]
