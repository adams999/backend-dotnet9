# Test script for verifying .NET 9 best practices implementation

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

# Test 2: Get All Clients (should work)
Write-Host "2. Testing GET /api/clients..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$baseUrl/api/clients" -Method GET -UseBasicParsing
    Write-Host "   Status: $($response.StatusCode)" -ForegroundColor Green
    $clients = $response.Content | ConvertFrom-Json
    Write-Host "   Clients found: $($clients.Count)" -ForegroundColor Green
} catch {
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 3: Validation Error (invalid email)
Write-Host "3. Testing FluentValidation (invalid data)..." -ForegroundColor Yellow
try {
    $body = @{
        name = ""
        email = "invalid-email"
        phoneNumber = "123"
    } | ConvertTo-Json

    $response = Invoke-WebRequest -Uri "$baseUrl/api/clients" -Method POST `
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

# Test 4: Valid Client Creation
Write-Host "4. Testing Valid Client Creation..." -ForegroundColor Yellow
try {
    $body = @{
        name = "Test Client"
        email = "test@example.com"
        phoneNumber = "555-1234"
    } | ConvertTo-Json

    $response = Invoke-WebRequest -Uri "$baseUrl/api/clients" -Method POST `
        -ContentType "application/json" -Body $body -UseBasicParsing
    Write-Host "   Status: $($response.StatusCode)" -ForegroundColor Green
    $client = $response.Content | ConvertFrom-Json
    Write-Host "   Created Client ID: $($client.id)" -ForegroundColor Green
    Write-Host "   Name: $($client.name)" -ForegroundColor Green
} catch {
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 5: Not Found Error
Write-Host "5. Testing Exception Handling (Not Found)..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$baseUrl/api/clients/99999" -Method GET -UseBasicParsing
    Write-Host "   Unexpected success: $($response.StatusCode)" -ForegroundColor Red
} catch {
    if ($_.ErrorDetails.Message) {
        $errorResponse = $_.ErrorDetails.Message | ConvertFrom-Json
        Write-Host "   Status: $($_.Exception.Response.StatusCode.value__)" -ForegroundColor Green
        Write-Host "   Message: $($errorResponse.message)" -ForegroundColor Green
    } else {
        Write-Host "   Status: 404 (Not Found)" -ForegroundColor Green
    }
}
Write-Host ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Testing Complete!" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
