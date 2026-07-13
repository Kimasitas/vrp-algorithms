FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore ./IP_KiprasRudzinskas.csproj
RUN dotnet publish ./IP_KiprasRudzinskas.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
RUN apt-get update && apt-get install -y libgdiplus && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=build /app/publish .
COPY --from=build /src/IP_places_data_2026.xlsx .
ENTRYPOINT ["dotnet", "IP_KiprasRudzinskas.dll"]