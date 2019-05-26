
::install KONG
docker network create kong-net
docker run -d --name kong-database --network=kong-net -p 5432:5432  -e "POSTGRES_USER=kong" -e "POSTGRES_DB=kong" postgres:9.6
docker run --rm --network=kong-net -e "KONG_DATABASE=postgres" -e "KONG_PG_HOST=kong-database" -e "KONG_CASSANDRA_CONTACT_POINTS=kong-database" kong:latest kong migrations bootstrap
docker run -d --name kong --network=kong-net -e "KONG_DATABASE=postgres" -e "KONG_PG_HOST=kong-database" -e "KONG_CASSANDRA_CONTACT_POINTS=kong-database" -e "KONG_PROXY_ACCESS_LOG=/dev/stdout" -e "KONG_ADMIN_ACCESS_LOG=/dev/stdout" -e "KONG_PROXY_ERROR_LOG=/dev/stderr" -e "KONG_ADMIN_ERROR_LOG=/dev/stderr" -e "KONG_ADMIN_LISTEN=0.0.0.0:8001, 0.0.0.0:8444 ssl" -p 9000:8000 -p 8443:8443 -p 9001:8001 -p 8444:8444 kong:latest                                   
::Start KONGA
docker run -d -p 1337:1337 --network=kong-net --name konga -v /var/data/kongadata:/app/kongadata -e "NODE_ENV=production" pantsel/konga
:: RESIZER API
docker run -d -p 10000:80 --network=kong-net --name api-resizer otaviojulianons/api-resizer:1.0.2
docker run -d -p 9411:9411 --name zipkin --net=kong-net openzipkin/zipkin
docker run -d -p 3000:3000 --name=grafana --network=kong-net -e "GF_SERVER_ROOT_URL=http://grafana.server.name"  -e "GF_SECURITY_ADMIN_PASSWORD=secret"  grafana/grafana

docker run -d -p 9090:9090 -v \prometheus.yml:/etc/prometheus/prometheus.yml --name prometheus --net=kong-net prom/prometheus
docker run -d -p 9090:9090 --name prometheus --net=kong-net prom/prometheus
docker run -d -p 9090:9090 -v %cd%\prometheus.yml:/etc/prometheus/prometheus.yml --name prometheus --net=kong-net prom/prometheus
::-c '-config.file=/etc/prometheus/prometheus.yml'