# Build stage
FROM golang:1.23.4-alpine3.20 AS build

# Install dependencies
RUN apk add --no-cache gcc libc-dev git

# Set working directory
WORKDIR /build

# Copy all project files
COPY . .

# Download Go modules now that code is present
RUN go mod tidy && go mod download

# Install swag CLI
RUN go install github.com/swaggo/swag/cmd/swag@v1.16.3

# Generate Swagger docs
RUN /go/bin/swag init

# Run tests and build binary
RUN go test ./... && go build -o pricing-service

# Final stage
FROM alpine:3.20.3

# Upgrade packages and create non-root user
RUN apk upgrade --no-cache \
    && addgroup --system --gid 3369 easytrade \
    && adduser --system --ingroup easytrade --uid 3369 easytrade

USER easytrade:easytrade
WORKDIR /home/easytrade

ENV GIN_MODE=release

# Copy built binary from build stage
COPY --from=build --chown=easytrade:easytrade /build/pricing-service .

# Start the application
CMD ["./pricing-service"]
