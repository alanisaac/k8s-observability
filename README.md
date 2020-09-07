# basic-k8s-stack
A repository to set up a basic k8s stack with monitoring

## Setup
- You will need the default k8s cluster provided by Docker Desktop (Mac or WSL2 should both work)
- You will need to install [helmfile](https://github.com/roboll/helmfile)
  - You may need to install [helm-diff](https://github.com/databus23/helm-diff)

Then run the following command to install onto a k8s cluster:
```
helmfile apply
```

Create a looping container in the 'execution' namespace:
```
kubectl create namespace execution
kubectl apply -f busybox-loop.yaml -n execution
```

### Grafana
- Set up a port forward for Grafana for `3000:3000`
- Navigate to http://localhost:3000
- Sign in as `admin`/`prom-operator`

### Kibana
- Set up a port forward for Kibana for `5601:5601`
- Navigate to http://localhost:5601
- Set up the default index pattern as `logstash-*`

## Notes
- This helmfile has been tweaked to support Docker Desktop.  For example:
  - The main Elasticsearch chart requires a multi-node cluster by default.  The values here are [for Docker Desktop](https://github.com/elastic/helm-charts/blob/master/elasticsearch/examples/docker-for-mac/values.yaml) and a single-node cluster.
  - `metrics-server` is not enabled by default in Docker Desktop, and installed by this helmfile
