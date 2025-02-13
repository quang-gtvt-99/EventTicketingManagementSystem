# Stage 1: Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Restore
COPY ["EventTicketingManagementSystem/EventTicketingManagementSystem.csproj", "EventTicketingManagementSystem/"]
RUN dotnet restore "EventTicketingManagementSystem/EventTicketingManagementSystem.csproj"

# Build
COPY ["EventTicketingManagementSystem", "EventTicketingManagementSystem/"]
RUN dotnet build "EventTicketingManagementSystem/EventTicketingManagementSystem.csproj" -c Release -o /app/build

# Stage 2: Publish stage
FROM build AS publish
RUN dotnet publish "EventTicketingManagementSystem/EventTicketingManagementSystem.csproj" -c Release -o /app/publish

# Stage 3: Run stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
ENV ASPNETCORE_HTTP_PORTS=5001
EXPOSE 5001
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "EventTicketingManagementSystem.dll" ]
