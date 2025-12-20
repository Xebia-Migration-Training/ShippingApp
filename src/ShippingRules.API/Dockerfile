# Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/ShippingRules.API/ShippingRules.API.csproj", "ShippingRules.API/"]
COPY ["src/ShippingRules.Application/ShippingRules.Application.csproj", "ShippingRules.Application/"]
COPY ["src/ShippingRules.Domain/ShippingRules.Domain.csproj", "ShippingRules.Domain/"]
COPY ["src/ShippingRules.Infrastructure/ShippingRules.Infrastructure.csproj", "ShippingRules.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "ShippingRules.API/ShippingRules.API.csproj"

# Copy all source code
COPY src/ .

# Build
WORKDIR "/src/ShippingRules.API"
RUN dotnet build "ShippingRules.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ShippingRules.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Run
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "ShippingRules.API.dll"]