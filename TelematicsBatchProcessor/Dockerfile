FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TelematicsBatchProcessor/TelematicsBatchProcessor.csproj", "TelematicsBatchProcessor/"]
COPY ["TelematicsCore/TelematicsCore.csproj", "TelematicsCore/"]
COPY ["TelematicsData/TelematicsData.csproj", "TelematicsData/"]
RUN dotnet restore "TelematicsBatchProcessor/TelematicsBatchProcessor.csproj"
COPY . .
WORKDIR "/src/TelematicsBatchProcessor"
RUN dotnet build "TelematicsBatchProcessor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TelematicsBatchProcessor.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TelematicsBatchProcessor.dll"]
