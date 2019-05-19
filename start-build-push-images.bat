::@echo off
cd src
docker login
docker build -t otaviojulianons/api-gateway:1.0.1 -f APIGateway/Dockerfile .
docker build -t otaviojulianons/api-resizer:1.0.1 -f Resizer/Dockerfile .
docker build -t otaviojulianons/api-customer:1.0.1 -f Customers/Dockerfile .
docker build -t otaviojulianons/api-fileserver:1.0.1 -f FileServer/Dockerfile .

docker push otaviojulianons/api-gateway:1.0.1
docker push otaviojulianons/api-resizer:1.0.1
docker push otaviojulianons/api-customer:1.0.1
docker push otaviojulianons/api-fileserver:1.0.1

pause


