@echo off
kubectl apply -f postgres.yml
kubectl apply -f kong_migration_postgres.yml
kubectl apply -f kong_postgres.yml
kubectl apply -f konga.yml
kubectl apply -f ../message-broker/rabbitmq.yml
kubectl apply -f ../apis/api-customer.yml
kubectl apply -f ../apis/api-resizer.yml
kubectl apply -f ../apis/api-fileserver.yml