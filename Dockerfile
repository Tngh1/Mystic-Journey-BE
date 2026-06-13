# Mystic Journey Backend Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY BLL/BLL.csproj BLL/
COPY DAL/DAL.csproj DAL/
COPY Mystic-Journey-API/Mystic-Journey-API.csproj Mystic-Journey-API/

# Restore dependencies
RUN dotnet restore Mystic-Journey-API/Mystic-Journey-API.csproj

# Copy all source files
COPY BLL/ BLL/
COPY DAL/ DAL/
COPY Mystic-Journey-API/ Mystic-Journey-API/

# Build
WORKDIR /src/Mystic-Journey-API
RUN dotnet build -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "Mystic-Journey-API.dll"]
