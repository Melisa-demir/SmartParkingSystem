FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY SmartParkingSystem/*.csproj SmartParkingSystem/
RUN dotnet restore SmartParkingSystem/SmartParkingSystem.csproj

COPY . .
RUN dotnet publish SmartParkingSystem/SmartParkingSystem.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "SmartParkingSystem.dll"]