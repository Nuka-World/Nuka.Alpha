## Initialize Runtime Layer
FROM mcr.microsoft.com/dotnet/core/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

## Build
FROM mcr.microsoft.com/dotnet/core/sdk:5.0 AS build
WORKDIR /src

COPY . .
RUN dotnet publish -c Release -o /app

## Run
FROM base AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Nuka.Status.dll"]