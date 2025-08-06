FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Upstart.Api/Upstart.Api.csproj", "Upstart.Api/"]
COPY ["Upstart.Application/Upstart.Application.csproj", "Upstart.Application/"]
COPY ["Upstart.Domain/Upstart.Domain.csproj", "Upstart.Domain/"]
COPY ["Upstart.Persistence/Upstart.Persistence.csproj", "Upstart.Persistence/"]
RUN dotnet restore "Upstart.Api/Upstart.Api.csproj"
COPY . .
WORKDIR "/src/Upstart.Api"
RUN dotnet build "Upstart.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Upstart.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Upstart.Api.dll"]