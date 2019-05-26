FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT Production

# Copy csproj and restore as distinct layers
COPY Customers/Customers.csproj ./Customers/
COPY SharedKernel/SharedKernel.csproj ./SharedKernel/

RUN dotnet restore Customers/Customers.csproj

COPY /Customers ./Customers
COPY /SharedKernel ./SharedKernel

RUN dotnet publish Customers/Customers.csproj -c Release -o out --no-restore

# Build runtime image
FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /app

COPY --from=build-env /app/Customers/out .
ENTRYPOINT ["dotnet", "Customers.dll"]


# docker build --no-cache -t otaviojulianons/api-customer:1.0.3 -f Customers/Dockerfile .
# docker run -d -p 9001:80 --name api-customer otaviojulianons/api-customer:1.0.3