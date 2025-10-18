FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["RepetaitorAPI/RepetaitorAPI.csproj", "RepetaitorAPI/"]
COPY ["Infrastructure.MailSenderService/Infrastructure.MailSenderService.csproj", "Infrastructure.MailSenderService/"]
COPY ["Core.Application/Core.Application.csproj", "Core.Application/"]
COPY ["Core.Domain/Core.Domain.csproj", "Core.Domain/"]
COPY ["Infrastructure.ImagesStore/Infrastructure.ImagesStore.csproj", "Infrastructure.ImagesStore/"]
COPY ["Infrastructure.AIService/Infrastructure.AIService.csproj", "Infrastructure.AIService/"]
COPY ["Infrastructure.Persistence/Infrastructure.Persistence.csproj", "Infrastructure.Persistence/"]
COPY ["Infrastructure.ProjectServices/Infrastructure.ProjectServices.csproj", "Infrastructure.ProjectServices/"]
RUN dotnet restore "RepetaitorAPI/RepetaitorAPI.csproj"
COPY . .
WORKDIR "/src/RepetaitorAPI"
RUN dotnet build "./RepetaitorAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./RepetaitorAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RepetaitorAPI.dll"]
