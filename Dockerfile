FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
# Copy only the project file(s) first
COPY AppSettingsEnvOverride.csproj ./
RUN dotnet restore

# Copy the rest of the application files
COPY . ./
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AppSettingsEnvOverride.dll"]
