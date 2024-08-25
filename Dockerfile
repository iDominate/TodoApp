FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy all the layers' csproj files into respective folders
COPY ["./TodoApp.Api/TodoApp.Api.csproj", "src/TodoApp.Api/"]
COPY ["./TodoApp.Infrastructure/TodoApp.Infrastructure.csproj", "src/TodoApp.Infrastructure/"]
COPY ["./TodoApp.Application/TodoApp.Application.csproj", "src/TodoApp.Application/"]
COPY ["./TodoApp.Domain/TodoApp.Domain.csproj", "src/TodoApp.Domain/"]

# run restore over API project - this pulls restore over the dependent projects as well
RUN dotnet restore "src/TodoApp.Api/TodoApp.Api.csproj"

COPY . .

# run build over the API project
WORKDIR "/src/TodoApp.Api/"
RUN dotnet build -c Release -o /app/build

# run publish over the API project
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS runtime
WORKDIR /app

COPY --from=publish /app/publish .
RUN ls -l
ENTRYPOINT [ "dotnet", "TodoApp.Api.dll" ]