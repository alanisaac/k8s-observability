# k8s-observability
A repository to demonstrate a Kubernetes cluster that uses modern, open-source observability tools for logging, metrics, and tracing.

## Deployment

### Prequisites
- You will need the a k8s cluster.
  - The default cluster provided by Docker Desktop is sufficient (Mac or WSL2 should both work)
- You will need to install [helmfile](https://github.com/roboll/helmfile)
  - You may need to install [helm-diff](https://github.com/databus23/helm-diff) separately as well

### Installation
Run the following command to install onto a k8s cluster:

```sh
helmfile apply
```

It may take several minutes for all services to stabilize and enter the running state.  Once that's completed, you can access services using port forwards:

```sh
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

## The Observability Stack

### Logging
The logging solution is a full ELK stack, using the following tools:

- Filebeat as the log collector and forwarder
- Kafka to act as a log buffering mechanism
- Logstash as the log aggregator and shipper
- Elasticsearch as log storage
- Kibana as the discovery and visualization tool

See [this article](https://logz.io/blog/deploying-kafka-with-elk/) from logz.io for diagrams and more on how these tools are set up and work together.

### Metrics
The metrics solution is the Prometheus & Grafana stack.

### Tracing
The tracing solution is Jaeger, with an Elasticsearch backend.  Since Elasticsearch and Kafka are already deployed for logging, the same deployments are reused.

See the Jaeger [architecture diagram](https://www.jaegertracing.io/docs/1.22/architecture/) for more information.