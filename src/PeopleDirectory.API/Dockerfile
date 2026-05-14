FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/PeopleDirectory.API/PeopleDirectory.API.csproj", "src/PeopleDirectory.API/"]
COPY ["src/PeopleDirectory.Application/PeopleDirectory.Application.csproj", "src/PeopleDirectory.Application/"]
COPY ["src/PeopleDirectory.Domain/PeopleDirectory.Domain.csproj", "src/PeopleDirectory.Domain/"]
COPY ["src/PeopleDirectory.Infrastructure/PeopleDirectory.Infrastructure.csproj", "src/PeopleDirectory.Infrastructure/"]
RUN dotnet restore "src/PeopleDirectory.API/PeopleDirectory.API.csproj"
COPY src/ src/
RUN dotnet publish "src/PeopleDirectory.API/PeopleDirectory.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
RUN mkdir -p /app/wwwroot/uploads
ENTRYPOINT ["dotnet", "PeopleDirectory.API.dll"]
