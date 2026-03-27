# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos primero el archivo .csproj para aprovechar la cache
COPY BlackCanvasApp/BlackCanvasApp.csproj BlackCanvasApp/
RUN dotnet restore BlackCanvasApp/BlackCanvasApp.csproj

# Copiamos el resto del código
COPY . .
WORKDIR /src/BlackCanvasApp
RUN dotnet publish -c Release -o /app/publish

# Etapa final (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Exponemos el puerto dinámico
ENV ASPNETCORE_URLS=http://+:$PORT
ENV ASPNETCORE_ENVIRONMENT=Production

# Comando de inicio
ENTRYPOINT ["dotnet", "BlackCanvasApp.dll"]
