# =========================
# Build Stage
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["CampusTouch.API/CampusTouch.API.csproj", "CampusTouch.API/"]
COPY ["CampusTouch.Application/CampusTouch.Application.csproj", "CampusTouch.Application/"]
COPY ["CampusTouch.Domain/CampusTouch.Domain.csproj", "CampusTouch.Domain/"]
COPY ["CampusTouch.Infrastructure/CampusTouch.Infrastructure.csproj", "CampusTouch.Infrastructure/"]

RUN dotnet restore "CampusTouch.API/CampusTouch.API.csproj"

COPY . .

WORKDIR "/src/CampusTouch.API"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false


# =========================
# Runtime Stage
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Development

COPY --from=build /app/publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "CampusTouch.API.dll"]