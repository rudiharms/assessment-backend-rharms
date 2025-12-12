

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]

COPY ["src/Assecor.Api.Person/Assecor.Api.Person.csproj", "src/Assecor.Api.Person/"]
COPY ["src/Assecor.Api.Infrastructure/Assecor.Api.Infrastructure.csproj", "src/Assecor.Api.Infrastructure/"]
COPY ["src/Assecor.Api.Application/Assecor.Api.Application.csproj", "src/Assecor.Api.Application/"]
COPY ["src/Assecor.Api.Domain/Assecor.Api.Domain.csproj", "src/Assecor.Api.Domain/"]

RUN dotnet restore "src/Assecor.Api.Person/Assecor.Api.Person.csproj"

COPY ["src/", "src/"]

WORKDIR "/src/src/Assecor.Api.Person"
RUN dotnet build "Assecor.Api.Person.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Assecor.Api.Person.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Assecor.Api.Person.dll"]