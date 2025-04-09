# release-mixed-arch

This directory contains YAML files that will deploy easytrade onto an OpenShift or Kubernetes cluster that includes compute nodes on both IBM LinuxONE (s390x architecture) and Intel/x86 (amd64). 

The deployment YAML files specify which architecture node each microservice will deploy onto via nodeSelectors.

Follow the instructions in the easytrade [README](../../README.md#openshift-instructions) making sure to specify the `release-mixed-arch` directory.

| Service                                                              | Architecture |
| -------------------------------------------------------------------- | ------------ |
| [Account service](src/accountservice/README.md)                      | amd64        |
| [Aggregator service](src/aggregator-service/README.md)               | amd64        |
| [Broker service](src/broker-service/README.md)                       | s390x        |
| [Calculation service](src/calculationservice/README.md)              | amd64        |
| [Content creator](src/contentcreator/README.md)                      | amd64        |
| [Credit card order service](src/credit-card-order-service/README.md) | amd64        |
| [Db (MSSQL)](src/db/README.md)                                       | amd64        |
| [Db (MySQL)](src/db/README.md)                                       | s390x        |
| [Engine](src/engine/README.md)                                       | s390x        |
| [Feature flag service](src/feature-flag-service/README.md)           | amd64        |
| [Frontend](src/frontend/README.md)                                   | amd64        |
| [Frontend reverse-proxy](src/frontendreverseproxy/README.md)         | amd64        |
| [Loadgen](src/loadgen/README.md)                                     | amd64        |
| [Login service](src/loginservice/README.md)                          | amd64        |
| [Manager](src/manager/easyTradeManager/README.md)                    | amd64        |
| [Offer service](src/offerservice/README.md)                          | amd64        |
| [Pricing service](src/pricing-service/README.md)                     | amd64        |
| [Problem operator](src/problem-operator/README.md)                   | amd64        |
| [RabbitMQ](src/rabbitmq/README.md)                                   | amd64        |
| [Third party service](src/third-party-service/README.md)             | amd64        |
