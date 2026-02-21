# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copy solution and project files
COPY ["ONT PROJECT.sln", "./"]
COPY ["ONT PROJECT/ONT PROJECT.csproj", "ONT PROJECT/"]
COPY ["IBayiLibrary/IBayiLibrary.csproj", "IBayiLibrary/"]

# Restore dependencies
RUN dotnet restore "ONT PROJECT.sln"

# Copy all code and publish the app
COPY . ./
RUN dotnet publish "ONT PROJECT.sln" -c Release -o out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Expose the port Render uses
EXPOSE 80

# Start the app
ENTRYPOINT ["dotnet", "ONT PROJECT.dll"]
