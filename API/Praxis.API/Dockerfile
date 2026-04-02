FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy solution + all projects
COPY . ./

# Go to API project
WORKDIR /app/API/Praxis.API

RUN dotnet restore
RUN dotnet publish -c Release -o /out

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /out ./

EXPOSE 8080

ENTRYPOINT ["dotnet", "Praxis.API.dll"]