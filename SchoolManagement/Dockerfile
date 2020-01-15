FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["SchoolManagement/SchoolManagement.csproj", "SchoolManagement/"]
COPY ["Model/Model.csproj", "Model/"]
COPY ["EmailSender/EmailSender.csproj", "EmailSender/"]
RUN dotnet restore "SchoolManagement/SchoolManagement.csproj"
COPY . .
WORKDIR "/src/SchoolManagement"
RUN dotnet build "SchoolManagement.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "SchoolManagement.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "SchoolManagement.dll"]
