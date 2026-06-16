FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["ReservationsService/ReservationsService.csproj", "ReservationsService/"]
RUN dotnet restore "ReservationsService/ReservationsService.csproj"

COPY . .
WORKDIR "/src/ReservationsService"
RUN dotnet publish "ReservationsService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ReservationsService.dll"]