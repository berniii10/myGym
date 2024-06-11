# Use the official dotnet SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory in the container
WORKDIR /app

# Copy the csproj file to the working directory
COPY ApiDemo/*.csproj ./

# Restore NuGet packages
RUN dotnet restore

# Copy the remaining source code to the working directory
COPY ApiDemo/ ./

# Build the project
RUN dotnet build -c Release -o /app/build

# Run tests if any
# RUN dotnet test

# Publish the application
RUN dotnet publish -c Release -o /app/publish

# Final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Set the working directory in the container
WORKDIR /app

# Copy the published output from the build stage to the runtime stage
COPY --from=build /app/publish .

# Expose the port your application listens on
EXPOSE 80

# Command to run the application
ENTRYPOINT ["dotnet", "ApiDemo.dll"]

