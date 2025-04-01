# https://mcr.microsoft.com/v2/dotnet/sdk/tags/list
FROM registry.redhat.io/rhel8/dotnet-80:8.0-16 AS build

WORKDIR /src
COPY ["easyTradeManager.csproj", "./"]

# Add MySQL EF Core package to the project
# RUN dotnet add easyTradeManager.csproj package Pomelo.EntityFrameworkCore.MySql --version 9.0.0

USER root
RUN mkdir -p /src/obj && chmod -R 777 /src
USER default

RUN dotnet restore "easyTradeManager.csproj"

COPY [".", "."]

RUN dotnet publish "easyTradeManager.csproj" -c Release -o publish

# https://mcr.microsoft.com/v2/dotnet/aspnet/tags/list
FROM registry.redhat.io/rhel8/dotnet-80:8.0-16 AS runtime

# fix a problem with inotify instance limit
# https://stackoverflow.com/a/67111195
ENV DOTNET_HOSTBUILDER__RELOADCONFIGONCHANGE=false

# RUN apk add --no-cache icu-libs \
#     && apk upgrade --no-cache \
#     && addgroup --system --gid 3369 easytrade \
#     && adduser --system --ingroup easytrade --uid 3369 easytrade

USER root
RUN groupadd -r easytrade --gid 3369 && \
    useradd -r -g easytrade --uid 3369 easytrade

USER easytrade:easytrade
WORKDIR /home/easytrade

COPY --from=build --chown=easytrade:easytrade ["/src/publish", "./"]

ENV ASPNETCORE_ENVIRONMENT=Development

CMD ["dotnet", "easyTradeManager.dll"]
