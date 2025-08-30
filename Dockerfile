# ========== build stage ==========
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy only the project file first to leverage Docker cache for restore
COPY StreamApi.csproj ./

# Restore dependencies for the specific project
RUN dotnet restore ./StreamApi.csproj

# Copy the rest of the sources
COPY . .

# Publish the specific project into a folder the runtime stage will consume
# Note: run publish WITHOUT --no-restore to ensure all packages (e.g. ILLink.Tasks) are fetched
RUN dotnet publish ./StreamApi.csproj -c Release -o /app/publish

# ========== runtime stage ==========
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Timezone (useful for logs)
ENV TZ=Asia/Almaty
# Install tzdata and wget for healthchecks
RUN apt-get update \
    && apt-get install -y tzdata wget \
    && rm -rf /var/lib/apt/lists/* \
    && ln -snf /usr/share/zoneinfo/$TZ /etc/localtime \
    && echo $TZ > /etc/timezone

# Copy published app from build stage
COPY --from=build /app/publish ./

# Create a non-root user and give ownership of /app
RUN useradd -m appuser \
    && chown -R appuser:appuser /app
USER appuser

# Port inside the container
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Health endpoint (uses wget which is installed above)
HEALTHCHECK --interval=20s --timeout=3s --retries=3 CMD wget -qO- http://127.0.0.1:8080/health || exit 1

ENTRYPOINT ["dotnet", "StreamApi.dll"]
