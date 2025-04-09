FROM ubuntu/mysql:latest

ENV MYSQL_ROOT_PASSWORD=password

# Copy the SQL dump file
COPY dump.sql /docker-entrypoint-initdb.d/dump.sql

# Change ownership and permissions
RUN chown mysql:mysql /docker-entrypoint-initdb.d/dump.sql && \
    chmod 644 /docker-entrypoint-initdb.d/dump.sql