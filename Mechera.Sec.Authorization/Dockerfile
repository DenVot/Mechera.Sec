FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Mechera.Sec.Authorization/Mechera.Sec.Authorization.csproj", "Mechera.Sec.Authorization/"]
RUN dotnet restore "Mechera.Sec.Authorization/Mechera.Sec.Authorization.csproj"
COPY . .
WORKDIR "/src/Mechera.Sec.Authorization"
RUN dotnet build "Mechera.Sec.Authorization.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Mechera.Sec.Authorization.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mechera.Sec.Authorization.dll"]