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

COPY --from=build-env /app/Resizer/out .
ENTRYPOINT ["dotnet", "Resizer.dll"]

# docker build -t resizerapi -f Resizer/Dockerfile .
# docker run -d -p 8081:80 --name resizerapi-local resizerapi