# Stage 1: Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Restore
COPY ["EventTicketingManagementSystem/EventTicketingManagementSystem.Worker/EventTicketingManagementSystem.Worker.csproj", "EventTicketingManagementSystem/EventTicketingManagementSystem.Worker/EventTicketingManagementSystem.Worker.csproj"]
COPY ["EventTicketingManagementSystem/EventTicketingManagementSystem.Data/EventTicketingManagementSystem.Data.csproj", "EventTicketingManagementSystem/EventTicketingManagementSystem.Data/EventTicketingManagementSystem.Data.csproj"]
COPY ["EventTicketingManagementSystem/EventTicketingManagementSystem.Services/EventTicketingManagementSystem.Services.csproj", "EventTicketingManagementSystem/EventTicketingManagementSystem.Services/EventTicketingManagementSystem.Services.csproj"]
COPY ["EventTicketingManagementSystem/EventTicketingMananagementSystem.Core/EventTicketingMananagementSystem.Core.csproj", "EventTicketingManagementSystem/EventTicketingMananagementSystem.Core/EventTicketingMananagementSystem.Core.csproj"]

RUN dotnet restore "EventTicketingManagementSystem/EventTicketingManagementSystem.Worker/EventTicketingManagementSystem.Worker.csproj"

# Build
COPY ["EventTicketingManagementSystem/EventTicketingManagementSystem.Worker", "EventTicketingManagementSystem/EventTicketingManagementSystem.Worker"]
COPY ["EventTicketingManagementSystem/EventTicketingManagementSystem.Data", "EventTicketingManagementSystem/EventTicketingManagementSystem.Data"]
COPY ["EventTicketingManagementSystem/EventTicketingManagementSystem.Services", "EventTicketingManagementSystem/EventTicketingManagementSystem.Services"]
COPY ["EventTicketingManagementSystem/EventTicketingMananagementSystem.Core", "EventTicketingManagementSystem/EventTicketingMananagementSystem.Core"]

RUN dotnet build "EventTicketingManagementSystem/EventTicketingManagementSystem.Worker/EventTicketingManagementSystem.Worker.csproj" -c Release -o /app/build

# Stage 2: Publish stage
FROM build AS publish
RUN dotnet publish "EventTicketingManagementSystem/EventTicketingManagementSystem.Worker/EventTicketingManagementSystem.Worker.csproj" -c Release -o /app/publish

# Stage 3: Run stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Install dependencies for SkiaSharp
RUN apt-get update && apt-get install -y \
    libc6-dev \
    libfontconfig1 \
    libgdiplus \
    && rm -rf /var/lib/apt/lists/*

ENV ASPNETCORE_HTTP_PORTS=5002
EXPOSE 5002
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "EventTicketingManagementSystem.Worker.dll" ]
