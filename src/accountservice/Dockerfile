FROM eclipse-temurin:21.0.6_7-jdk AS build

WORKDIR /app
# make gradlew download gradle files before copying source files to use caching
COPY ["gradle", "gradle"]
COPY ["build.gradle", "settings.gradle", "gradlew", "./"]
RUN ./gradlew --version

COPY ["src", "./src"]

RUN ./gradlew bootJar

FROM eclipse-temurin:21.0.5_11-jre

RUN addgroup --system --gid 3369 spring \
    && adduser --system --ingroup spring --uid 3369 spring
USER spring:spring
WORKDIR /home/spring

COPY --from=build --chown=spring:spring ["/app/build/libs/*.jar", "app.jar"]

CMD ["java", "-jar", "app.jar"]