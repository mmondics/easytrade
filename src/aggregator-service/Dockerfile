FROM golang:1.23.4-alpine3.20 AS build

RUN apk add --no-cache gcc libc-dev

WORKDIR /build

COPY ["go.mod", "go.sum", "./"]

RUN go mod download

COPY [".", "."]

RUN go test ./... \
    && go build -ldflags="-linkmode=external" -o aggregator-service

FROM alpine:3.20.3

RUN apk upgrade --no-cache \
    && addgroup --system --gid 3369 easytrade \
    && adduser --system --ingroup easytrade --uid 3369 easytrade
USER easytrade:easytrade
WORKDIR /home/easytrade

COPY --from=build --chown=easytrade:easytrade ["/build/aggregator-service", "/build/config.yaml", "./"]

CMD [ "./aggregator-service" ]