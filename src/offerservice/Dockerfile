FROM node:22.11.0-alpine3.20 AS build

WORKDIR /app

COPY ["*.json", "./"]
COPY ["./src", "./src"]

RUN npm ci \
    && npm run build

FROM node:22.11.0-alpine3.20 AS dev

RUN apk upgrade --no-cache \
    && addgroup --system --gid 3369 easytrade \
    && adduser --system --ingroup easytrade --uid 3369 easytrade
USER easytrade:easytrade
WORKDIR /home/easytrade

COPY --chown=easytrade:easytrade ["package.json", "package-lock.json", "./"]

RUN npm ci --omit=dev \
    && npm cache clean --force

COPY --chown=easytrade:easytrade --from=build ["app/dist", "./dist"]

CMD [ "node", "./dist/src/app.js" ]
