cd ..\src
docker login
docker build --no-cache -t otaviojulianons/api-gateway:1.0.3 -f APIGateway/Dockerfile .
docker build --no-cache -t otaviojulianons/api-resizer:1.0.4 -f Resizer/Dockerfile .
docker build --no-cache -t otaviojulianons/api-customer:1.0.4 -f Customers/Dockerfile .
docker build --no-cache -t otaviojulianons/api-fileserver:1.0.4 -f FileServer/Dockerfile .

docker push otaviojulianons/api-gateway:1.0.3
docker push otaviojulianons/api-resizer:1.0.4
docker push otaviojulianons/api-customer:1.0.4
docker push otaviojulianons/api-fileserver:1.0.4

cd ..\deploy


