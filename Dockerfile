#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5215
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["demo/hybrid/Caviar.Demo.Hybrid/Caviar.Demo.Hybrid.csproj", "demo/hybrid/Caviar.Demo.Hybrid/"]
COPY ["demo/hybrid/Caviar.Demo.Wasm/Caviar.Demo.Wasm.csproj", "demo/hybrid/Caviar.Demo.Wasm/"]
COPY ["src/Caviar.SharedKernel/Caviar.SharedKernel.csproj", "src/Caviar.SharedKernel/"]
COPY ["src/Caviar.AntDesignUI/Caviar.AntDesignUI.csproj", "src/Caviar.AntDesignUI/"]
COPY ["src/Caviar.Infrastructure/Caviar.Infrastructure.csproj", "src/Caviar.Infrastructure/"]
COPY ["src/Caviar.Core/Caviar.Core.csproj", "src/Caviar.Core/"]
RUN dotnet restore "demo/hybrid/Caviar.Demo.Hybrid/Caviar.Demo.Hybrid.csproj"
COPY . .
WORKDIR "/src/demo/hybrid/Caviar.Demo.Hybrid"
RUN dotnet build "Caviar.Demo.Hybrid.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Caviar.Demo.Hybrid.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Caviar.Demo.Hybrid.dll"]