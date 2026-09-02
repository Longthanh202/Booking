# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# Build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy file csproj
COPY Booking.Api/Booking.Api.csproj Booking.Api/
COPY Container_App.Common/Container_App.Common.csproj Container_App.Common/
COPY Container_App.Core/Container_App.Core.csproj Container_App.Core/
COPY Container_App.Data/Container_App.Data.csproj Container_App.Data/
COPY Container_App.Service/Container_App.Service.csproj Container_App.Service/

# Restore
RUN dotnet restore Booking.Api/Booking.Api.csproj

# Copy source
COPY . .

# Build
WORKDIR /src/Booking.Api
RUN dotnet build Booking.Api.csproj -c Debug -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish Booking.Api.csproj \
    -c Debug \
    -o /app/publish \
    /p:UseAppHost=false

# Runtime
FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Booking.Api.dll"]