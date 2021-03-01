# k8s-observability
A repository to demonstrate a Kubernetes cluster that uses modern, open-source observability tools for logging, metrics, and tracing.

## Deployment

### Prequisites
- You will need the a k8s cluster.
  - The default cluster provided by Docker Desktop is sufficient (Mac or WSL2 should both work)
- You will need to install [helmfile](https://github.com/roboll/helmfile)
  - You may need to install [helm-diff](https://github.com/databus23/helm-diff) separately as well

### Installation
For a fresh install, there are two components to deploy:

```sh
# deploy the observability stack
./deploy-observability.sh

# deploy the sample service
./deploy-service.sh
```

It may take several minutes for all services to stabilize and enter the running state.  Once that's completed, you can access services using port forwards:

```sh
# Service
kubectl port-forward -n apps deployment/my-app-release 8080:80
# Kibana
kubectl port-forward -n observability deployment/kibana-kibana 5601:5601
# Grafana
kubectl port-forward -n observability deployment/prometheus-grafana 3000:3000
# Jaeger
kubectl port-forward -n observability deployment/jaeger-query 16686:16686
```

### Notes
Values have been tweaked specifically to support a local Kubernetes cluster.  For example:
- The main Elasticsearch chart requires a multi-node cluster by default.  The values here are [for Docker Desktop](https://github.com/elastic/helm-charts/blob/master/elasticsearch/examples/docker-for-mac/values.yaml) and a single-node cluster.
- The Prometheus Node Exporter cannot run on local docker-desktop at the current version, so an older one is used.  See the helmfile for more details.

## The Stack

### Logging
The logging solution is a full ELK stack, using the following tools:

- Filebeat for the log collector and forwarder
- Kafka for the log buffering mechanism
- Logstash for the log aggregator and shipper
- Elasticsearch for log storage
- Kibana for the discovery and visualization tool

See [the ELK architecture with Kafka](https://logz.io/blog/deploying-kafka-with-elk/) from logz.io for more information.

### Metrics
The metrics solution is the Prometheus & Grafana stack:

- Kube State Metrics for Kubernetes object metrics
- Prometheus for metric scraping and storage
- Grafana for metric visualization

See the [Prometheus architecture](https://prometheus.io/docs/introduction/overview/#architecture) for more information.

### Tracing
The tracing solution is Jaeger, with an Elasticsearch backend.  Since Elasticsearch and Kafka are already deployed for logging, the same deployments are reused:

- Jaeger for tracing agents and collectors
- Kafka for the trace buffering mechanism
- Elasticsearch for the storage backend

See the [Jaeger architecture](https://www.jaegertracing.io/docs/1.22/architecture/) for more information.

### Service
The configurations of logging, metrics, and tracing above form an "observability contract": any service which conforms to that contract will be monitored without additional configuration.  The contract is:

- Services that are deployed in the "apps" namespace
- Which log to the console in JSON format
- Which expose metrics at `/metrics` in the Prometheus text format
- Which send Jaeger trace data over UDP to the endpoint specified by the env variables `JAEGER_AGENT_HOST:JAEGER_AGENT_PORT`

Other best practices include:

- Excluding core endpoints from logging / metrics / tracing under normal operations, such as:
  - Health checks
  - The `/metrics` endpoint