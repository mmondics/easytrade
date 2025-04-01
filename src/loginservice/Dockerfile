# https://mcr.microsoft.com/v2/dotnet/sdk/tags/list
FROM registry.redhat.io/rhel8/dotnet-80:8.0-16 AS build

WORKDIR /easyTradeLoginService

USER root
RUN mkdir -p /easyTradeLoginService/obj && chmod -R 777 /easyTradeLoginService
USER default

COPY ["./easyTradeLoginService/", "./"]

RUN dotnet test && dotnet publish -c Release -o publish

# https://mcr.microsoft.com/v2/dotnet/aspnet/tags/list
FROM registry.redhat.io/rhel8/dotnet-80:8.0-16 AS runtime

# fix a problem with inotify instance limit
# https://stackoverflow.com/a/67111195
ENV DOTNET_HOSTBUILDER__RELOADCONFIGONCHANGE=false

USER root
# RUN apk add --no-cache icu-libs \
#     && apk upgrade --no-cache \
#     && addgroup --system --gid 3369 easytrade \
#     && adduser --system --ingroup easytrade --uid 3369 easytrade

RUN groupadd -g 3369 easytrade \
    && useradd -r -g easytrade -u 3369 easytrade

USER easytrade:easytrade
WORKDIR /home/easytrade

COPY --from=build --chown=easytrade:easytrade ["/easyTradeLoginService/publish", "./"]

ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "easyTradeLoginService.dll"]
