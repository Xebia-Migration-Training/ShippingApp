# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy solution file
COPY ["ShippingRules.sln", "./"]

# Copy project files
COPY ["src/ShippingRules.API/ShippingRules.API.csproj", "src/ShippingRules.API/"]
COPY ["src/ShippingRules.Application/ShippingRules.Application.csproj", "src/ShippingRules.Application/"]
COPY ["src/ShippingRules.Domain/ShippingRules.Domain.csproj", "src/ShippingRules.Domain/"]
COPY ["src/ShippingRules.Infrastructure/ShippingRules.Infrastructure.csproj", "src/ShippingRules.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/ShippingRules.API/ShippingRules.API.csproj"

# Copy all source code
COPY src/ src/

# Build
WORKDIR "/src/src/ShippingRules.API"
RUN dotnet build "ShippingRules.API.csproj" -c $BUILD_CONFIGURATION -o /app/build --no-restore

# Publish Stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ShippingRules.API.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    --no-build \
    /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Create non-root user
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Copy published files
COPY --from=publish /app/publish .

# Create logs directory with proper permissions
RUN mkdir -p /app/logs && chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

# Environment variables
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
    CMD curl --fail http://localhost:8080/health || exit 1

EXPOSE 8080

ENTRYPOINT ["dotnet", "ShippingRules.API.dll"]