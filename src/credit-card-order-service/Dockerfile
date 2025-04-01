FROM eclipse-temurin:21.0.6_7-jdk AS build

WORKDIR /app
# make gradlew download gradle files before copying source files to use caching
COPY ["build.gradle", "settings.gradle", "gradlew", "./"]
COPY ["gradle", "gradle"]
RUN ./gradlew --version

COPY ["src", "./src"]

RUN ./gradlew test
RUN ./gradlew bootJar

FROM eclipse-temurin:21.0.5_11-jre

RUN groupadd --system --gid 3369 easytrade \
    && useradd --system --gid easytrade --create-home --uid 3369 easytrade
USER easytrade:easytrade
WORKDIR /home/easytrade

COPY --from=build --chown=easytrade:easytrade ["/app/build/libs/*.jar", "app.jar"]

CMD ["java", "-jar", "app.jar"]