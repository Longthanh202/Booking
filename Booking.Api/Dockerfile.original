# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# Build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy file csproj
COPY Booking.Api/Booking.Api.csproj Booking.Api/
COPY Booking.Common/Booking.Common.csproj Booking.Common/
COPY Booking.Core/Booking.Core.csproj Booking.Core/
COPY Booking.Data/Booking.Data.csproj Booking.Data/
COPY Booking.Service/Booking.Service.csproj Booking.Service/
COPY Booking.Storage/Booking.Storage.csproj Booking.Storage/

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