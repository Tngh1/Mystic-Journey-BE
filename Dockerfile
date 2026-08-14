# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY Directory.Build.props ./
COPY BLL/BLL.csproj BLL/
COPY DAL/DAL.csproj DAL/
COPY Mystic-Journey-API/Mystic-Journey-API.csproj Mystic-Journey-API/

RUN dotnet restore Mystic-Journey-API/Mystic-Journey-API.csproj

COPY BLL/ BLL/
COPY DAL/ DAL/
COPY Mystic-Journey-API/ Mystic-Journey-API/

FROM build AS publish
RUN dotnet publish Mystic-Journey-API/Mystic-Journey-API.csproj \
    --configuration $BUILD_CONFIGURATION \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080 \
    DOTNET_EnableDiagnostics=0

EXPOSE 8080

USER $APP_UID

ENTRYPOINT ["dotnet", "Mystic-Journey-API.dll"]

FROM build AS migrations-build
ENV PATH="$PATH:/root/.dotnet/tools"
RUN dotnet tool install --global dotnet-ef --version 8.0.30
RUN dotnet ef migrations bundle \
    --project DAL/DAL.csproj \
    --startup-project Mystic-Journey-API/Mystic-Journey-API.csproj \
    --configuration Release \
    --output /app/efbundle

FROM final AS migrations
COPY --from=migrations-build --chown=$APP_UID:$APP_UID /app/efbundle /app/efbundle
ENTRYPOINT ["/app/efbundle"]
