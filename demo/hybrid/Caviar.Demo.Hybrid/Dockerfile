#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5215
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Demo/Hybrid/Caviar.Demo.Hybrid/Caviar.Demo.Hybrid.csproj", "Demo/Hybrid/Caviar.Demo.Hybrid/"]
COPY ["Demo/Hybrid/Caviar.Demo.Wasm/Caviar.Demo.Wasm.csproj", "Demo/Hybrid/Caviar.Demo.Wasm/"]
COPY ["Src/Caviar.SharedKernel/Caviar.SharedKernel.csproj", "Src/Caviar.SharedKernel/"]
COPY ["Src/Caviar.AntDesignUI/Caviar.AntDesignUI.csproj", "Src/Caviar.AntDesignUI/"]
COPY ["Src/Caviar.Infrastructure/Caviar.Infrastructure.csproj", "Src/Caviar.Infrastructure/"]
COPY ["Src/Caviar.Core/Caviar.Core.csproj", "Src/Caviar.Core/"]
RUN dotnet restore "Demo/Hybrid/Caviar.Demo.Hybrid/Caviar.Demo.Hybrid.csproj"
COPY . .
WORKDIR "/src/Demo/Hybrid/Caviar.Demo.Hybrid"
RUN dotnet build "Caviar.Demo.Hybrid.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Caviar.Demo.Hybrid.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Caviar.Demo.Hybrid.dll"]