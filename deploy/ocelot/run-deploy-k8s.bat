@echo off
kubectl apply -f ocelot-gateway.yml
kubectl apply -f consul.yml
kubectl apply -f ../message-broker/rabbitmq.yml
kubectl apply -f ../apis/api-customer.yml
kubectl apply -f ../apis/api-resizer.yml
kubectl apply -f ../apis/api-fileserver.yml