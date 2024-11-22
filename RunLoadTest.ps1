# Define the Docker image and command parameters
$dockerImage = "grafana/k6"
$scriptsFolder = "$(pwd)/k6:/scripts"
$networkName = "carbidsystem_services-network"
$testScriptPath = "/scripts/load-test.js"

# Run the Docker command
docker run --rm -v "$scriptsFolder" --network $networkName $dockerImage run $testScriptPath