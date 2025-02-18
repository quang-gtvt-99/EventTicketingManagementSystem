# Stage 1: Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Restore
COPY ["EventTicketingManagementSystem/EventTicketingManagementSystem/EventTicketingManagementSystem.API.csproj", "EventTicketingManagementSystem/EventTicketingManagementSystem/EventTicketingManagementSystem.API.csproj"]
COPY ["EventTicketingManagementSystem/EventTicketingManagementSystem.Data/EventTicketingManagementSystem.Data.csproj", "EventTicketingManagementSystem/EventTicketingManagementSystem.Data/EventTicketingManagementSystem.Data.csproj"]
COPY ["EventTicketingManagementSystem/EventTicketingManagementSystem.Services/EventTicketingManagementSystem.Services.csproj", "EventTicketingManagementSystem/EventTicketingManagementSystem.Services/EventTicketingManagementSystem.Services.csproj"]
COPY ["EventTicketingManagementSystem/EventTicketingMananagementSystem.Core/EventTicketingMananagementSystem.Core.csproj", "EventTicketingManagementSystem/EventTicketingMananagementSystem.Core/EventTicketingMananagementSystem.Core.csproj"]

RUN dotnet restore "EventTicketingManagementSystem/EventTicketingManagementSystem/EventTicketingManagementSystem.API.csproj"

# Build
COPY ["EventTicketingManagementSystem/EventTicketingManagementSystem", "EventTicketingManagementSystem/EventTicketingManagementSystem"]
COPY ["EventTicketingManagementSystem/EventTicketingManagementSystem.Data", "EventTicketingManagementSystem/EventTicketingManagementSystem.Data"]
COPY ["EventTicketingManagementSystem/EventTicketingManagementSystem.Services", "EventTicketingManagementSystem/EventTicketingManagementSystem.Services"]
COPY ["EventTicketingManagementSystem/EventTicketingMananagementSystem.Core", "EventTicketingManagementSystem/EventTicketingMananagementSystem.Core"]

RUN dotnet build "EventTicketingMananagementSystem/EventTicketingManagementSystem/EventTicketingManagementSystem.API.csproj" -c Release -o /app/build

# Stage 2: Publish stage
FROM build AS publish
RUN dotnet publish "EventTicketingMananagementSystem/EventTicketingManagementSystem/EventTicketingManagementSystem.API.csproj" -c Release -o /app/publish

# Stage 3: Run stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
ENV ASPNETCORE_HTTP_PORTS=5001
EXPOSE 5001
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "EventTicketingManagementSystem.dll" ]
