# Test script for RealEstate API endpoints
$baseUrl = "http://localhost:5072/api"
$headers = @{
    "Content-Type" = "application/json"
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Testing RealEstate API Endpoints" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Helper function to test endpoints
function Test-Endpoint {
    param(
        [string]$Method,
        [string]$Url,
        [string]$Body = $null,
        [string]$Description
    )
    
    Write-Host "Testing: $Description" -ForegroundColor Yellow
    Write-Host "  Method: $Method" -ForegroundColor Gray
    Write-Host "  URL: $Url" -ForegroundColor Gray
    
    try {
        if ($Body) {
            Write-Host "  Body: $Body" -ForegroundColor Gray
            $response = Invoke-WebRequest -Uri $Url -Method $Method -Headers $headers -Body $Body -UseBasicParsing
        } else {
            $response = Invoke-WebRequest -Uri $Url -Method $Method -Headers $headers -UseBasicParsing
        }
        
        Write-Host "  Success - Status: $($response.StatusCode)" -ForegroundColor Green
        Write-Host "  Response: $($response.Content)" -ForegroundColor Gray
        Write-Host ""
        return $response
    } catch {
        Write-Host "  Failed - Error: $($_.Exception.Message)" -ForegroundColor Red
        if ($_.Exception.Response) {
            $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
            $responseBody = $reader.ReadToEnd()
            Write-Host "  Response: $responseBody" -ForegroundColor Red
        }
        Write-Host ""
        return $null
    }
}

# ========================================
# Test Clients Endpoints
# ========================================
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "CLIENTS ENDPOINTS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Create Client
$createClientBody = @{
    name = "Juan Perez"
    email = "juan.perez@example.com"
    phone = "+1234567890"
} | ConvertTo-Json

$createClientResponse = Test-Endpoint -Method "POST" -Url "$baseUrl/clients" -Body $createClientBody -Description "Create Client"
$clientId = if ($createClientResponse) { ($createClientResponse.Content | ConvertFrom-Json).id } else { $null }

# Get All Clients
Test-Endpoint -Method "GET" -Url "$baseUrl/clients" -Description "Get All Clients"

# Get Client by ID
if ($clientId) {
    Test-Endpoint -Method "GET" -Url "$baseUrl/clients/$clientId" -Description "Get Client by ID ($clientId)"
}

# Update Client
if ($clientId) {
    $updateClientBody = @{
        name = "Juan Perez Updated"
        email = "juan.updated@example.com"
        phone = "+0987654321"
    } | ConvertTo-Json
    
    Test-Endpoint -Method "PUT" -Url "$baseUrl/clients/$clientId" -Body $updateClientBody -Description "Update Client ($clientId)"
}

# Delete Client (we'll delete later after creating transaction)

# ========================================
# Test Properties Endpoints
# ========================================
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "PROPERTIES ENDPOINTS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Create Property
$createPropertyBody = @{
    address = "123 Main Street"
    price = 250000.00
    propertyType = "House"
    status = "Available"
} | ConvertTo-Json

$createPropertyResponse = Test-Endpoint -Method "POST" -Url "$baseUrl/properties" -Body $createPropertyBody -Description "Create Property"
$propertyId = if ($createPropertyResponse) { ($createPropertyResponse.Content | ConvertFrom-Json).id } else { $null }

# Get All Properties
Test-Endpoint -Method "GET" -Url "$baseUrl/properties" -Description "Get All Properties"

# Get Property by ID
if ($propertyId) {
    Test-Endpoint -Method "GET" -Url "$baseUrl/properties/$propertyId" -Description "Get Property by ID ($propertyId)"
}

# Update Property
if ($propertyId) {
    $updatePropertyBody = @{
        address = "123 Main Street Updated"
        price = 275000.00
        propertyType = "House"
        status = "Sold"
    } | ConvertTo-Json
    
    Test-Endpoint -Method "PUT" -Url "$baseUrl/properties/$propertyId" -Body $updatePropertyBody -Description "Update Property ($propertyId)"
}

# Delete Property (we'll delete later after creating transaction)

# ========================================
# Test Transactions Endpoints
# ========================================
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "TRANSACTIONS ENDPOINTS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Create Transaction (requires Client and Property)
if ($clientId -and $propertyId) {
    $createTransactionBody = @{
        clientId = $clientId
        propertyId = $propertyId
        transactionType = "Sale"
        amount = 275000.00
        transactionDate = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ss")
    } | ConvertTo-Json
    
    $createTransactionResponse = Test-Endpoint -Method "POST" -Url "$baseUrl/transactions" -Body $createTransactionBody -Description "Create Transaction"
    $transactionId = if ($createTransactionResponse) { ($createTransactionResponse.Content | ConvertFrom-Json).id } else { $null }
} else {
    Write-Host "Skipping transaction creation - missing client or property ID" -ForegroundColor Yellow
    Write-Host ""
}

# Get All Transactions
Test-Endpoint -Method "GET" -Url "$baseUrl/transactions" -Description "Get All Transactions"

# Get Transaction by ID
if ($transactionId) {
    Test-Endpoint -Method "GET" -Url "$baseUrl/transactions/$transactionId" -Description "Get Transaction by ID ($transactionId)"
}

# ========================================
# Cleanup - Delete Created Resources
# ========================================
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "CLEANUP - DELETE RESOURCES" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Delete Property
if ($propertyId) {
    Test-Endpoint -Method "DELETE" -Url "$baseUrl/properties/$propertyId" -Description "Delete Property ($propertyId)"
}

# Delete Client
if ($clientId) {
    Test-Endpoint -Method "DELETE" -Url "$baseUrl/clients/$clientId" -Description "Delete Client ($clientId)"
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Testing Complete!" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
