#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["S4C.API/С4S.API.csproj", "S4C.API/"]
COPY ["C4S.Shared/C4S.Shared.csproj", "C4S.Shared/"]
COPY ["S4C.DB/C4S.DB.csproj", "S4C.DB/"]
COPY ["S4C.Services/C4S.Services.csproj", "S4C.Services/"]
RUN dotnet restore "./S4C.API/./С4S.API.csproj"
COPY . .
WORKDIR "/src/S4C.API"
RUN dotnet build "./С4S.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./С4S.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "С4S.API.dll"]