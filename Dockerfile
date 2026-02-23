# -------- Build stage --------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Inredningsbutik.Web/*.csproj src/Inredningsbutik.Web/
COPY src/Inredningsbutik.Core/*.csproj src/Inredningsbutik.Core/
COPY src/Inredningsbutik.Infrastructure/*.csproj src/Inredningsbutik.Infrastructure/
COPY src/Inredningsbutik.Api/*.csproj src/Inredningsbutik.Api/

RUN dotnet restore src/Inredningsbutik.Web/Inredningsbutik.Web.csproj

COPY . .

WORKDIR /src/src/Inredningsbutik.Web
RUN dotnet publish -c Release -o /app/publish

# -------- Runtime stage --------
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Inredningsbutik.Web.dll"]