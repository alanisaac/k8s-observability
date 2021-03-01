docker build ./src -t local/my-app:1.0.0
helm upgrade --install my-test-app-release ./charts/my-app --namespace=apps --create-namespace