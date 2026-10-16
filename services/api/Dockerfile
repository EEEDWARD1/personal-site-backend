# ---------- Build stage ----------
# Full .NET SDK image: has the compiler and tools needed to build the app.
# This stage is thrown away at the end; only its output is kept.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy only the project file first and restore NuGet packages.
# Docker caches this layer, so packages are only re-downloaded
# when the .csproj changes (i.e. when you add/update a package).
COPY src/EduardTeodor.Api/EduardTeodor.Api.csproj src/EduardTeodor.Api/
RUN dotnet restore src/EduardTeodor.Api

# Copy the rest of the source code and publish a Release build.
# Code changes only invalidate from here down, so rebuilds stay fast.
# --no-restore skips restoring again since it was done above.
COPY . .
RUN dotnet publish src/EduardTeodor.Api -c Release -o /app --no-restore

# ---------- Runtime stage ----------
# Slim ASP.NET Core runtime image: can run the app but can't build it.
# Much smaller and has less attack surface than the SDK image.
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Copy only the published output from the build stage.
# Source code, SDK and build artifacts never reach the final image.
COPY --from=build /app .

# Documents the port the app listens on (.NET 8+ defaults to 8080).
# Doesn't publish it; docker-compose's "ports:" does that.
EXPOSE 8080

# Start the API when the container runs.
# File name is case-sensitive on Linux.
ENTRYPOINT ["dotnet", "EduardTeodor.Api.dll"]