## Requirements

- If you're running on visual studio you will need the Visual Studio 2022 Version 17.11.5 or High.
- .NET 8
- The SDK and tools can be downloaded from https://dotnet.microsoft.com/pt-br/download/dotnet/8.0
- Docker
- Linux or Windows with Hyper-V enable


## Technologies implemented:

- .NET 8 WepAPI
- Yarp (reverse Proxy)
- WORKER
- Entity Framework Core 8.0.11
- .NET Native DI
- FluentValidator
- MediatR
- Swagger UI
- Docker
- RabbitMQ
- MassTransit
- Serilog
- Elastic Search
- Kibana
- K6
- Powershell
- TestContainers
- XUnit
- SQL Server
- Redis
- Grafana

## Architecture:

- SOLID
- Clean Architecture
- CQRS
- Microservices

## Instructions
To run this application you must change your directory to the base directory of the project and then execute the following commands: 

PowerShell command to run all the containers in docker-compose.yml:
```
.\Run.ps1
```
or if you want to run just the infrastructure on docker and run the apis separetely
```
.\Run.ps1 -Environment Dev
```

Docker compose command:
```
docker-compose -f ./docker-compose.yml up -d
```

The webapis can be accessed by the API gateway

**Gateway**: ```http://localhost:9000``` or ```https://localhost:11000/```

## Load Testing

You can make changes on the k6/load-test to make use of the API, by default it is testing the post to the bids on docker.
you can install the k6 using the following commands

### Windows
```
choco install k6
```
```
winget install k6 --source winget
```

### Ubuntu

```
sudo gpg -k
sudo gpg --no-default-keyring --keyring /usr/share/keyrings/k6-archive-keyring.gpg --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys C5AD17C747E3415A3642D57D77C6C491D6AC1D69
echo "deb [signed-by=/usr/share/keyrings/k6-archive-keyring.gpg] https://dl.k6.io/deb stable main" | sudo tee /etc/apt/sources.list.d/k6.list
sudo apt-get update
sudo apt-get install k6
```

### Docker
```
docker pull grafana/k6
```

### Usage

In this project you can run on docker using the following commands and you must ensure that all the containers are up and running

#### Docker command
```
docker run --rm -v "$(pwd)/k6:/scripts" --network carbidsystem_services-network grafana/k6 run /scripts/load-test.js
```

#### Powershell command

```
.\RunLoadTest.ps1
```

## Endpoints

### Bid endpoints:

To add a new Bid


POST http://localhost:9000/bids
```json
{
  "AuctionId": 1,
  "Amount": 10000,
  "UserId": 'someuser',
}
```
To get a bid detail
GET 
```
http://localhost:9000/bids/{1}
```

To get bids of an Auction
GET 

```
http://localhost:9000/bids/1/bids?pageNumber={pageNumber}&pageSize={pageSize}
```

### Auction endpoints:

To add a new Auction

POST http://localhost:9000/auctions
```json
{
  "CarId": 10,
  "StartTime": "2024-11-23T09:00:00Z",
  "EndTime": "2024-11-31T09:00:00Z"
}
```

To get auctions

```
http://localhost:9000/auctions?pageNumber={pageNumber}&pageSize={pageSize}
```
To get an auction detail

GET 

```
http://localhost:9000/auctions/{auction.Id}
```

