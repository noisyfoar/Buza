FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Buza.csproj ./
RUN dotnet restore Buza.csproj

COPY . ./
RUN dotnet publish Buza.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production
ENV PORT=8080
ENV ConnectionStrings__DefaultConnection="Data Source=/data/database.db"

COPY --from=build /app/publish ./

RUN mkdir -p /data/uploads \
    && rm -rf /app/wwwroot/uploads \
    && ln -s /data/uploads /app/wwwroot/uploads

EXPOSE 8080
ENTRYPOINT ["dotnet", "Buza.dll"]
