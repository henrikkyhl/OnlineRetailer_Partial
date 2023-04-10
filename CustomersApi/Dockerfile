FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["CustomersApi/CustomersApi.csproj", "CustomersApi/"]
COPY ["SharedModels/SharedModels.csproj", "SharedModels/"]
COPY ["ProductApi/ProductApi.csproj", "ProductApi/"]
RUN dotnet restore "CustomersApi/CustomersApi.csproj"
COPY . .
WORKDIR "/src/CustomersApi"
RUN dotnet build "CustomersApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CustomersApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CustomersApi.dll"]
