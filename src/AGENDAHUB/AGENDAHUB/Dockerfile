# Use a imagem oficial do .NET SDK como a imagem de build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copie o arquivo de projeto e restaure as dependências
COPY *.csproj ./
RUN dotnet restore

# Copie o restante dos arquivos e construa a aplicação
COPY . ./
RUN dotnet publish "AGENDAHUB.csproj" -c Release -o /app/publish

# Use a imagem oficial do .NET Runtime como a imagem de runtime
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Exponha a porta em que a aplicação será executada
EXPOSE 80

# Defina o comando de entrada para executar a aplicação
ENTRYPOINT ["dotnet", "AGENDAHUB.dll"]
