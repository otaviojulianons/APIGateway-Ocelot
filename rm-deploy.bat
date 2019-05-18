::@echo off
kubectl delete deploy api-gateway
kubectl delete deploy api-customer
kubectl delete deploy api-resizer

kubectl delete svc api-gateway
kubectl delete svc api-customer
kubectl delete svc api-resizer