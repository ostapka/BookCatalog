# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

# Build stage: Compile the Blazor WebAssembly project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Client/BookCatalog.Client.csproj", "Client/"]
COPY ["Shared/BookCatalog.Shared.csproj", "Shared/"]
RUN dotnet restore "Client/BookCatalog.Client.csproj"
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Runtime stage: Use Nginx to serve the Blazor app
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=build /app/publish/wwwroot .