# Test script for verifying pagination and all routes

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Testing Pagination and All Routes" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$baseUrl = "http://localhost:5072"

# Test 1: Clients with Pagination
Write-Host "1. Testing GET /api/v1.0/clients with pagination..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$baseUrl/api/v1.0/clients?pageNumber=1&pageSize=2" -Method GET -UseBasicParsing
    Write-Host "   Status: $($response.StatusCode)" -ForegroundColor Green
    $result = $response.Content | ConvertFrom-Json
    Write-Host "   Total Count: $($result.totalCount)" -ForegroundColor Green
    Write-Host "   Page Size: $($result.pageSize)" -ForegroundColor Green
    Write-Host "   Total Pages: $($result.totalPages)" -ForegroundColor Green
    Write-Host "   Has Next: $($result.hasNext)" -ForegroundColor Green
} catch {
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 2: Properties with Pagination
Write-Host "2. Testing GET /api/v1.0/properties with pagination..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$baseUrl/api/v1.0/properties?pageNumber=1&pageSize=3" -Method GET -UseBasicParsing
    Write-Host "   Status: $($response.StatusCode)" -ForegroundColor Green
    $result = $response.Content | ConvertFrom-Json
    Write-Host "   Total Count: $($result.totalCount)" -ForegroundColor Green
} catch {
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 3: Transactions with Pagination
Write-Host "3. Testing GET /api/v1.0/transactions with pagination..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$baseUrl/api/v1.0/transactions?pageNumber=1&pageSize=5" -Method GET -UseBasicParsing
    Write-Host "   Status: $($response.StatusCode)" -ForegroundColor Green
    $result = $response.Content | ConvertFrom-Json
    Write-Host "   Total Count: $($result.totalCount)" -ForegroundColor Green
} catch {
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 4: Get specific client
Write-Host "4. Testing GET /api/v1.0/clients/1..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$baseUrl/api/v1.0/clients/1" -Method GET -UseBasicParsing
    Write-Host "   Status: $($response.StatusCode)" -ForegroundColor Green
} catch {
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 5: Create new client
Write-Host "5. Testing POST /api/v1.0/clients..." -ForegroundColor Yellow
try {
    $body = @{
        name = "Pagination Test Client"
        email = "pagination@test.com"
        phoneNumber = "555-PAGI"
    } | ConvertTo-Json

    $response = Invoke-WebRequest -Uri "$baseUrl/api/v1.0/clients" -Method POST `
        -ContentType "application/json" -Body $body -UseBasicParsing
    Write-Host "   Status: $($response.StatusCode)" -ForegroundColor Green
} catch {
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "All Routes Tested!" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
