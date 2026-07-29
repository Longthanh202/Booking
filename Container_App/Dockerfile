FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

EXPOSE 80
EXPOSE 443
EXPOSE 5925

# =========================
# BUILD
# =========================
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

ARG BUILD_CONFIGURATION=Release

WORKDIR /src

# Copy csproj trước để optimize cache
COPY ["Container_App/Container_App.csproj", "Container_App/"]

# Restore
RUN dotnet restore "Container_App/Container_App.csproj"

# Copy toàn bộ source
COPY . .

# Chuyển vào project chính
WORKDIR /src/Container_App

# Build
RUN dotnet build "Container_App.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build

# =========================
# PUBLISH
# =========================
FROM build AS publish

ARG BUILD_CONFIGURATION=Release

WORKDIR /src/Container_App

RUN dotnet publish "Container_App.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

# =========================
# FINAL
# =========================
FROM base AS final

WORKDIR /app

COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:5925

ENTRYPOINT ["dotnet", "Container_App.dll"]