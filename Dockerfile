# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files first (for better layer caching)
COPY Mystic-Journey-BE.sln ./
COPY Mystic-Journey-API/Mystic-Journey-API.csproj Mystic-Journey-API/
COPY BLL/BLL.csproj BLL/
COPY DAL/DAL.csproj DAL/

# Restore dependencies
RUN dotnet restore Mystic-Journey-API/Mystic-Journey-API.csproj

# Copy all source files
COPY . .

# Build and publish
WORKDIR /src/Mystic-Journey-API
RUN dotnet publish -c Release -o /app/publish --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create non-root user for security
RUN groupadd -r appgroup && useradd -r -g appgroup appuser

COPY --from=build /app/publish .

# Set ownership
RUN chown -R appuser:appgroup /app

# Switch to non-root user
USER appuser

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl --fail http://localhost:8080/swagger/index.html || exit 1

ENTRYPOINT ["dotnet", "Mystic-Journey-API.dll"]
