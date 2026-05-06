FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["backend/WalletService/WalletService.csproj", "backend/WalletService/"]
RUN dotnet restore "backend/WalletService/WalletService.csproj"
COPY . .
WORKDIR "/src/backend/WalletService"
RUN dotnet build "WalletService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WalletService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WalletService.dll"]
