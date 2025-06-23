FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TelematicsApi/TelematicsApi.csproj", "TelematicsApi/"]
COPY ["TelematicsCore/TelematicsCore.csproj", "TelematicsCore/"]
COPY ["TelematicsData/TelematicsData.csproj", "TelematicsData/"]
RUN dotnet restore "TelematicsApi/TelematicsApi.csproj"
COPY . .
WORKDIR "/src/TelematicsApi"
RUN dotnet build "TelematicsApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TelematicsApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TelematicsApi.dll"]
