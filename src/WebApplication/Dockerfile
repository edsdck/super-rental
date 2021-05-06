FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build-env
WORKDIR /app

# Copy necessary files and restore as distinct layer
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish WebApplication.csproj -c Release -o /app

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine
COPY --from=build-env /app .

EXPOSE 5554/tcp
ENV ASPNETCORE_URLS http://*:5554

ENTRYPOINT ["dotnet", "WebApplication.dll"]