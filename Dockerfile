# Use the official .NET 7 SDK image as a build stage.
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app

# Expose the port your application listens on
EXPOSE 80
EXPOSE 443

# Copy the project and restore dependencies.
COPY . ./
RUN dotnet restore

# Copy the project file and build the application.
COPY . ./
RUN dotnet publish -c Release -o out

# Use the official ASP.NET runtime image.
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build-env /app/out .

# Set the entry point for your application.
ENTRYPOINT ["dotnet", "BankingApi.dll"]
