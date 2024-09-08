# Use the .NET SDK image for the build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project files and restore dependencies
COPY PedidosApi/PedidosApi.csproj PedidosApi/
RUN dotnet restore PedidosApi/PedidosApi.csproj

# Copy the rest of the files and build the project
COPY . .
RUN dotnet build PedidosApi/PedidosApi.csproj -c Release -o /app/build

# Publish the application
RUN dotnet publish PedidosApi/PedidosApi.csproj -c Release -o /app/publish

# Final image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
COPY PedidosApi/appsettings.json /app/
ENTRYPOINT ["dotnet", "PedidosApi.dll"]