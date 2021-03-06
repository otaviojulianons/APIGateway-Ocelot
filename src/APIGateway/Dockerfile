﻿FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY APIGateway/APIGateway.csproj ./APIGateway/
COPY Customers/Customers.csproj ./Customers/
COPY SharedKernel/SharedKernel.csproj ./SharedKernel/

RUN dotnet restore APIGateway/APIGateway.csproj

COPY /APIGateway ./APIGateway
COPY /Customers ./Customers
COPY /SharedKernel ./SharedKernel

RUN dotnet publish APIGateway/APIGateway.csproj -c Release -o out --no-restore

# Build runtime image
FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /app

COPY --from=build-env /app/APIGateway/out .
ENTRYPOINT ["dotnet", "APIGateway.dll"]

# docker build --no-cache -t otaviojulianons/api-gateway:1.0.3 -f APIGateway/Dockerfile .
# docker run -d -p 5005:80 --name api-gateway --net=consul otaviojulianons/api-gateway:1.0.3 