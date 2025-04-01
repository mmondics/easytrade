FROM node:22.13.1 AS build

WORKDIR /app

COPY [".", "."]

RUN npm install --verbose \
    # && npm run build-test \
    && npm run build --verbose

FROM nginx:1.26.3-alpine

WORKDIR /app

COPY --from=build ["/app/dist", "."]

RUN rm /etc/nginx/conf.d/default.conf

COPY ["nginx.conf", "/etc/nginx/nginx.conf"]

CMD ["nginx", "-g", "daemon off;"]