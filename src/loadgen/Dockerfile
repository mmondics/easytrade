FROM node:22.11.0-bookworm AS builder

WORKDIR /usr/src/app

COPY ["./", "./"]

RUN npm ci \
    && npm run build

FROM node:22.11.0-bookworm AS dev

ARG CHROME_LIBS="libasound2 libatk-bridge2.0-0 libatk1.0-0 libcairo2 libcups2 libdrm-dev libgbm-dev libglib2.0-0 libnss3 libpango-1.0-0 libxcomposite1 libxdamage1 libxfixes-dev libxkbcommon-dev libxrandr-dev"

WORKDIR /usr/src/app

RUN apt update \
    && apt upgrade -y \
    && apt install -y ${CHROME_LIBS} \
    && apt install dumb-init \
    && rm -rf /var/lib/apt/lists/*

COPY ["package.json", "package-lock.json", "./"]

RUN npm ci --omit=dev \
    && npm cache clean --force

COPY --from=builder ["/usr/src/app/build", "./build"]

ENTRYPOINT [ "/usr/bin/dumb-init", "--" ]
CMD [ "node", "./build/src/main.js" ]
