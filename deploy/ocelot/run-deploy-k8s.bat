::@echo off
kubectl apply -f api-gateway.yml
kubectl apply -f api-customer.yml
kubectl apply -f api-resizer.yml
kubectl apply -f api-fileserver.yml
kubectl apply -f rabbitmq.yml
kubectl apply -f consul.yml