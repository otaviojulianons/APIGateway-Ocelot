FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

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

ENV ASPNETCORE_URLS http://localhost:80

EXPOSE 8500/tcp

ENTRYPOINT ["dotnet", "Customers.dll"]

# docker build -t customersapi -f Customers/Dockerfile .
# docker run -d -p 9001:80  --name customersapi-local customersapi