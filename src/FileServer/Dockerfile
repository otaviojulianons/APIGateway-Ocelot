FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY FileServer/FileServer.csproj ./FileServer/
COPY SharedKernel/SharedKernel.csproj ./SharedKernel/

RUN dotnet restore FileServer/FileServer.csproj

COPY /FileServer ./FileServer
COPY /SharedKernel ./SharedKernel

RUN dotnet publish FileServer/FileServer.csproj -c Release -o out --no-restore

# Build runtime image
FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /app

RUN mkdir logs
RUN chmod 777 /app/logs

VOLUME [ "/app/logs" ]

COPY --from=build-env /app/FileServer/out .
ENTRYPOINT ["dotnet", "FileServer.dll"]

# docker build --no-cache -t otaviojulianons/api-fileserver:1.0.4 -f FileServer/Dockerfile .
# docker run -d -p 9003:80 -v /tmp/logs:/app/logs --name api-fileserver otaviojulianons/api-fileserver:1.0.4