# Test script for verifying .NET 9 best practices implementation (Updated for API Versioning)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Testing RealEstate API - Best Practices" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$baseUrl = "http://localhost:5072"

# Test 1: Health Check
Write-Host "1. Testing Health Check Endpoint..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$baseUrl/health" -Method GET -UseBasicParsing
    Write-Host "   Status: $($response.StatusCode)" -ForegroundColor Green
    Write-Host "   Response: $($response.Content)" -ForegroundColor Green
} catch {
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 2: Get All Clients (API v1.0)
Write-Host "2. Testing GET /api/v1.0/clients (API Versioning)..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$baseUrl/api/v1.0/clients" -Method GET -UseBasicParsing
    Write-Host "   Status: $($response.StatusCode)" -ForegroundColor Green
    $clients = $response.Content | ConvertFrom-Json
    Write-Host "   Clients found: $($clients.Count)" -ForegroundColor Green
    Write-Host "   API Version header: $($response.Headers['api-supported-versions'])" -ForegroundColor Cyan
} catch {
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 3: API Version via Header
Write-Host "3. Testing API Version via Header..." -ForegroundColor Yellow
try {
    $headers = @{
        "X-Api-Version" = "1.0"
    }
    $response = Invoke-WebRequest -Uri "$baseUrl/api/v1.0/clients" -Method GET -Headers $headers -UseBasicParsing
    Write-Host "   Status: $($response.StatusCode)" -ForegroundColor Green
    Write-Host "   API Version accepted via header" -ForegroundColor Green
} catch {
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 4: Validation Error (invalid email)
Write-Host "4. Testing FluentValidation (invalid data)..." -ForegroundColor Yellow
try {
    $body = @{
        name = ""
        email = "invalid-email"
        phoneNumber = "123"
    } | ConvertTo-Json

    $response = Invoke-WebRequest -Uri "$baseUrl/api/v1.0/clients" -Method POST `
        -ContentType "application/json" -Body $body -UseBasicParsing
    Write-Host "   Unexpected success: $($response.StatusCode)" -ForegroundColor Red
} catch {
    $errorResponse = $_.ErrorDetails.Message | ConvertFrom-Json
    Write-Host "   Status: $($_.Exception.Response.StatusCode.value__)" -ForegroundColor Green
    Write-Host "   Message: $($errorResponse.message)" -ForegroundColor Green
    if ($errorResponse.errors) {
        Write-Host "   Validation Errors:" -ForegroundColor Green
        $errorResponse.errors.PSObject.Properties | ForEach-Object {
            Write-Host "     - $($_.Name): $($_.Value -join ', ')" -ForegroundColor Green
        }
    }
}
Write-Host ""

# Test 5: Valid Client Creation
Write-Host "5. Testing Valid Client Creation (v1.0)..." -ForegroundColor Yellow
try {
    $body = @{
        name = "Test Client API v1"
        email = "testv1@example.com"
        phoneNumber = "555-9999"
    } | ConvertTo-Json

    $response = Invoke-WebRequest -Uri "$baseUrl/api/v1.0/clients" -Method POST `
        -ContentType "application/json" -Body $body -UseBasicParsing
    Write-Host "   Status: $($response.StatusCode)" -ForegroundColor Green
    $client = $response.Content | ConvertFrom-Json
    Write-Host "   Created Client ID: $($client.id)" -ForegroundColor Green
    Write-Host "   Name: $($client.name)" -ForegroundColor Green
} catch {
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 6: Check Logs Directory
Write-Host "6. Checking Serilog Logs..." -ForegroundColor Yellow
$logsPath = "RealEstate.API\Logs"
if (Test-Path $logsPath) {
    $logFiles = Get-ChildItem $logsPath -Filter "log-*.txt" | Sort-Object LastWriteTime -Descending | Select-Object -First 1
    if ($logFiles) {
        Write-Host "   Log file found: $($logFiles.Name)" -ForegroundColor Green
        Write-Host "   Size: $([math]::Round($logFiles.Length / 1KB, 2)) KB" -ForegroundColor Green
        Write-Host "   Last modified: $($logFiles.LastWriteTime)" -ForegroundColor Green
    } else {
        Write-Host "   No log files found yet" -ForegroundColor Yellow
    }
} else {
    Write-Host "   Logs directory not created yet" -ForegroundColor Yellow
}
Write-Host ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Testing Complete!" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
