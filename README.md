# EasyTrade

Sample stock trading application modified to run on both Intel/x86 (amd64) architecture as well as IBM LinuxONE (s390x).

## Architecture diagram

![EasyTrade architecture](./img/architecture.jpg)

<!-- ## Database diagram

![EasyTrade database](./img/database.jpg) -->

## Service list

EasyTrade consists of the following services/components:

| Service                                                              | Proxy port | Proxy endpoint               |
| -------------------------------------------------------------------- | ---------- | ---------------------------- |
| [Account service](src/accountservice/README.md)                      | 80         | `/accountservice`            |
| [Aggregator service](src/aggregator-service/README.md)               | 80         | `---`                        |
| [Broker service](src/broker-service/README.md)                       | 80         | `/broker-service`            |
| [Calculation service](src/calculationservice/README.md)              | 80         | `---`                        |
| [Content creator](src/contentcreator/README.md)                      | 80         | `---`                        |
| [Credit card order service](src/credit-card-order-service/README.md) | 80         | `/credit-card-order-service` |
| [Db](src/db/README.md)                                               | 80         | `---`                        |
| [Engine](src/engine/README.md)                                       | 80         | `/engine`                    |
| [Feature flag service](src/feature-flag-service/README.md)           | 80         | `/feature-flag-service`      |
| [Frontend](src/frontend/README.md)                                   | 80         | `/`                          |
| [Frontend reverse-proxy](src/frontendreverseproxy/README.md)         | 80         | `---`                        |
| [Loadgen](src/loadgen/README.md)                                     | --         | `---`                        |
| [Login service](src/loginservice/README.md)                          | 80         | `/loginservice`              |
| [Manager](src/manager/easyTradeManager/README.md)                    | 80         | `/manager`                   |
| [Offer service](src/offerservice/README.md)                          | 80         | `/offerservice`              |
| [Pricing service](src/pricing-service/README.md)                     | 80         | `/pricing-service`           |
| [Problem operator](src/problem-operator/README.md)                   | 80         | `---`                        |
| [RabbitMQ](src/rabbitmq/README.md)                                   | 80         | `---`                        |
| [Third party service](src/third-party-service/README.md)             | 80         | `/third-party-service`       |

To learn more about endpoints / swagger for the services go to their respective readmes

## Mixed-architecture Cluster
This version of easyTrade can run on a mixed-architecture OpenShift cluster, i.e. one that has both amd64 and s390x compute nodes.

```
~ oc get no -l node-role.kubernetes.io/worker -L kubernetes.io/arch
NAME                     STATUS   ROLES    AGE   VERSION            ARCH
stg1.ma1.cpolab.local    Ready    worker   82d   v1.29.10+67d3387   amd64
stg2.ma1.cpolab.local    Ready    worker   82d   v1.29.10+67d3387   amd64
stg3.ma1.cpolab.local    Ready    worker   82d   v1.29.10+67d3387   amd64
work1.ma1.cpolab.local   Ready    worker   82d   v1.29.10+67d3387   amd64
work2.ma1.cpolab.local   Ready    worker   82d   v1.29.10+67d3387   amd64
work3.ma1.cpolab.local   Ready    worker   82d   v1.29.10+67d3387   amd64
work4.ma1.cpolab.local   Ready    worker   82d   v1.29.10+67d3387   amd64
work5.ma1.cpolab.local   Ready    worker   57d   v1.29.10+67d3387   s390x
work6.ma1.cpolab.local   Ready    worker   57d   v1.29.10+67d3387   s390x
work7.ma1.cpolab.local   Ready    worker   57d   v1.29.10+67d3387   s390x
work8.ma1.cpolab.local   Ready    worker   57d   v1.29.10+67d3387   s390x
```

Follow the instructions below and in the [README](kubernetes-manifests/release-mixed-arch/), making sure to specify the `release-mixed-arch` release.

## OpenShift Instructions

To deploy on OpenShift:

```bash
# Create the `easytrade` project
oc new-project easytrade

# Give the default user elevated credentials (don't do this in production 😊)
oc -n easytrade adm policy add-scc-to-user anyuid -z default

# then use the manifests to deploy
# use 'release-multiarch' to deploy to either s390x or amd64
# use 'release-mixed-arch' to deploy to a clustesr that includes both s390x and amd64
oc -n easytrade apply -f ./kubernetes-manifests/release-multiarch

# Optional: if you want the problem patterns to be automatically
# enabled once a day, deploy these manifests too
# kubectl -n easytrade apply -f ./kubernetes-manifests/problem-patterns

# Expose the easytrade frontend service as a route 
oc -n easytrade expose svc frontendreverseproxy-easytrade
```

## Kubernetes Instructions

To deploy on Kubernetes: 

```bash
# first create the namespace for it
kubectl create namespace easytrade

# then use the manifests to deploy
kubectl -n easytrade apply -f ./kubernetes-manifests/release-multiarch

# Optional: if you want the problem patterns to be automatically
# enabled once a day, deploy these manifests too
# kubectl -n easytrade apply -f ./kubernetes-manifests/problem-patterns

# to get the ip of reverse proxy
# look for EXTERNAL-IP of frontendreverseproxy
# it may take some time before it gets assigned
kubectl -n easytrade get svc

# to delete the deployment
kubectl delete namespace easytrade
```

## Where to start

After starting easyTrade application you can:

- go to the frontend and try it out. Navigate to the OpenShift route and you should see the login page. You can either create a new user, or use one of superusers (with easy passwords) like "demouser/demopass" or "specialuser/specialpass". Remember that in order to buy stocks you need money, so visit the deposit page first.
- go to some services swagger endpoint - you will find proper instructions in the dedicated service readmes.

## EasyTrade users

If you want to use easyTrade, then you will need a user. You can either:

- use an existing user - they have some preinserted data and new data is automatically generated:

  - login: james_norton
  - pass: pass_james_123

- create a new user - click on "Sign up" on the login page and create a new user.

> **NOTE:** After creating a new user there is no confirmation given, no email sent and you are not redirected... Just go back to login page and try to login.

<!-- ## Problem patterns

Currently there are 4 problem patterns supported in easyTrade:

1. DbNotResponding - after turning it on no new trades can be created as the database will throw an error. This problem pattern is kind of proof on concept that problem patterns work. Running it for around 20 minutes should generate a problem in dynatrace.

2. ErgoAggregatorSlowdown - after turning it on 2 of the aggregators will start receiving slower responses which will make them stop sending requests after some time. A potential run could take:

   - 15 min - then we will notice a small slowdown (for 150 seconds) followed by 40% lower traffic for 15 minutes on some requests
   - 20 min - then we will notice a small slowdown (for 150 seconds) followed by 40% lower traffic for 30 minutes on some requests

3. FactoryCrisis - when enabled, the factory won't produce new cards, which will cause the Third party service not to process credit card orders. This will block the Credit Card Order service.

4. HighCpuUsage - this problem pattern causes a slowdown of broker-service response time and highly increases CPU usage during that time. If the app is deployed on K8s, a CPU resource limit is also applied by the problem operator. This should generate CPU throttling on the pod.

To turn a plugin on/off send a request similar to the following:

```sh
curl -X PUT "http://{IP_ADDRESS}/feature-flag-service/v1/flags/{FEATURE_ID}/" \
-H  "accept: application/json" \
-d '{"enabled": {VALUE}}'
```

You can also manage enabled problem patterns via the easyTrade frontend.

> **NOTE:** More information on the feature flag service's parameters available in [feature flag service's doc](src/feature-flag-service/README.md).

You can also apply [these cronjobs](./kubernetes-manifests/problem-patterns/), which will enable the problem patterns once a day. -->