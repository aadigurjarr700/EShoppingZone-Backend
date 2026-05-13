#!/bin/bash

# Define variables
PROJECT_KEY="EShoppingZone-Backend"
PROJECT_NAME="EShoppingZone-Backend"
# Default SonarQube token, pass it as an argument or set it here
SONAR_TOKEN=$1 
SONAR_HOST_URL="http://localhost:9000"

if [ -z "$SONAR_TOKEN" ]; then
    echo "Error: SonarQube token is required."
    echo "Usage: ./run-sonar.sh <your_sonarqube_token>"
    echo "You can generate a token in SonarQube: My Account > Security > Generate Tokens"
    exit 1
fi

echo "Starting SonarQube Analysis for $PROJECT_NAME..."

# Step 1: Begin the SonarScanner analysis
# Using opencover reports path that match where dotnet test puts them
dotnet sonarscanner begin \
  /k:"$PROJECT_KEY" \
  /n:"$PROJECT_NAME" \
  /d:sonar.host.url="$SONAR_HOST_URL" \
  /d:sonar.login="$SONAR_TOKEN" \
  /d:sonar.cs.opencover.reportsPaths="**/TestResults/**/coverage.opencover.xml"

# Step 2: Build the solution
echo "Building the solution..."
dotnet build EShoppingZone-Backend.sln --no-incremental

# Step 3: Run the tests with code coverage
echo "Running tests..."
# --collect:"XPlat Code Coverage" with Format=opencover outputs to TestResults
dotnet test EShoppingZone-Backend.sln --no-build \
  --collect:"XPlat Code Coverage" \
  --results-directory:"./TestResults" \
  -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover

# Step 4: End the SonarScanner analysis
echo "Completing SonarQube Analysis..."
dotnet sonarscanner end /d:sonar.login="$SONAR_TOKEN"

echo "SonarQube analysis finished successfully!"
