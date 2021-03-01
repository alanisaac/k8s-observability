docker build ./service/src -t local/my-app:1.0.0
helm upgrade --install my-app-release ./service/charts/my-app --namespace=apps --create-namespace --force