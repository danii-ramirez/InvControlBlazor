FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["InvControl/Server/InvControl.Server.csproj", "InvControl.Server/"]
COPY ["InvControl/Client/InvControl.Client.csproj", "InvControl.Client/"]
COPY ["InvControl/Shared/InvControl.Shared.csproj", "InvControl.Shared/"]
RUN dotnet restore "InvControl/Server/InvControl.Server.csproj"
COPY . .
WORKDIR "/src/Server"
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "InvControl.Server.dll"]