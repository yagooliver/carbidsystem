param(
    [string]$Environment = ""  # Accepts an optional "Environment" parameter
)

# Function to check if all containers in the compose file are up and healthy
function Wait-For-Containers($ComposeFile) {
    Write-Host "Ensuring all containers in $ComposeFile are up and running..."

    # Start docker-compose services
    docker-compose -f $ComposeFile up -d

    # Get the list of services defined in the compose file
    $services = docker-compose -f $ComposeFile config --services

    Write-Host "Waiting for all containers to become healthy..."
    # Loop until all services are healthy
    while ($true) {
        $healthyContainers = docker-compose -f $ComposeFile ps --services --filter "status=running" | `
                              ForEach-Object { docker inspect --format='{{.State.Health.Status}}' $_ }

        # Check if all services are healthy
        $allHealthy = $true
        foreach ($service in $services) {
            if (-not ($healthyContainers -contains "healthy")) {
                $allHealthy = $false
                break
            }
        }

        if ($allHealthy) {
            Write-Host "All containers defined in $ComposeFile are healthy and running."
            break
        }
        Start-Sleep -Seconds 5
    }
}


# Define paths
$ComposeFile = "docker-compose.yml"
$ComposeDevFile = "docker-compose-dev.yml"
$AuctionServicePath = "./src/AuctionService/CarBidSystem.Auctions.Service"
$BidServicePath = "./src/BidService/CarBidSystem.Bids.Service"
$GatewayPath = "./src/Gateway/CarBidSystem.Gateway"

if ($Environment -eq "Dev") {
    Write-Host "Running in Dev environment..."

    Wait-For-Containers $ComposeDevFile

    Write-Host "Starting .NET applications in separate consoles..."
    Start-Process "powershell" -ArgumentList "dotnet run --project $AuctionServicePath" -NoNewWindow:$false
    Start-Sleep -Seconds 3
    Start-Process "powershell" -ArgumentList "dotnet run --project $BidServicePath" -NoNewWindow:$false
    Start-Sleep -Seconds 3
    Start-Process "powershell" -ArgumentList "dotnet run --project $GatewayPath" -NoNewWindow:$false

} else {
    Write-Host "Running in default environment..."

    docker-compose -f $ComposeFile up -d
}