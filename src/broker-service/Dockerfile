# https://mcr.microsoft.com/v2/dotnet/sdk/tags/list
FROM registry.redhat.io/rhel8/dotnet-80:8.0-16 AS build

WORKDIR /BrokerService
COPY ["BrokerService/", "./"]

USER root

# RUN dotnet test test/BrokerService.test.csproj \
#     && dotnet publish src/BrokerService.csproj -o /publish

RUN dotnet publish src/BrokerService.csproj -o /publish

USER default

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

RUN groupadd -r -g 3369 easytrade && \
    useradd -r -u 3369 -g easytrade -m -d /home/easytrade -s /sbin/nologin easytrade

USER default

USER easytrade:easytrade
WORKDIR /home/easytrade

COPY --from=build --chown=easytrade:easytrade ["/publish", "./"]

ENV ASPNETCORE_ENVIRONMENT=Development
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

ENTRYPOINT ["dotnet", "BrokerService.dll"]