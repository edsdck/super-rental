FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build-env
WORKDIR /app

# Copy necessary files and restore as distinct layer
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish Gateway.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine
COPY --from=build-env /app/out .

EXPOSE 4200/tcp
ENV ASPNETCORE_URLS http://*:4200

ENTRYPOINT ["dotnet", "Gateway.dll"]