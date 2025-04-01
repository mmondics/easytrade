FROM rabbitmq:3.13.7-management-alpine

COPY --chown=rabbitmq:rabbitmq ["rabbitmq.conf", "definitions.json", "/etc/rabbitmq/"]

CMD ["rabbitmq-server"]
