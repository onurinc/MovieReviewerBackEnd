#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["MR.Api/MR.Api.csproj", "MR.Api/"]
COPY ["MR.DataAccessLayer/MR.DataAccessLayer.csproj", "MR.DataAccessLayer/"]
RUN dotnet restore "MR.Api/MR.Api.csproj"
COPY . .
WORKDIR "/src/MR.Api"
RUN dotnet build "MR.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MR.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MR.Api.dll"]