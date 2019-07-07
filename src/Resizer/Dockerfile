FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY Resizer/Resizer.csproj ./Resizer/
COPY SharedKernel/SharedKernel.csproj ./SharedKernel/

RUN dotnet restore Resizer/Resizer.csproj

COPY /Resizer ./Resizer
COPY /SharedKernel ./SharedKernel

RUN dotnet publish Resizer/Resizer.csproj -c Release -o out --no-restore

# Build runtime image
FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /app

RUN mkdir logs
RUN chmod 777 /app/logs

VOLUME [ "/app/logs" ]

COPY --from=build-env /app/Resizer/out .
ENTRYPOINT ["dotnet", "Resizer.dll"]

# docker build --no-cache -t otaviojulianons/api-resizer:1.0.4 -f Resizer/Dockerfile .
# docker run -d -p 9002:80 -v /tmp/logs:/app/logs --name api-resizer otaviojulianons/api-resizer:1.0.4