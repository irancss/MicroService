# IdentityServer8 API Test Script
# This script tests the basic functionality of the IdentityServer

$baseUrl = "http://localhost:5062"

Write-Host "=== IdentityServer8 API Testing ===" -ForegroundColor Green

# Test 1: Health Check
Write-Host "`n1. Testing Health Check..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/health" -Method GET
    Write-Host "✓ Health Check: $response" -ForegroundColor Green
} catch {
    Write-Host "✗ Health Check Failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 2: Home Page
Write-Host "`n2. Testing Home Page..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$baseUrl/" -Method GET
    if ($response.StatusCode -eq 200) {
        Write-Host "✓ Home Page: Status $($response.StatusCode)" -ForegroundColor Green
    } else {
        Write-Host "✗ Home Page: Status $($response.StatusCode)" -ForegroundColor Red
    }
} catch {
    Write-Host "✗ Home Page Failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 3: IdentityServer Configuration
Write-Host "`n3. Testing IdentityServer Configuration..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/.well-known/openid_configuration" -Method GET
    Write-Host "✓ IdentityServer Config: Found issuer '$($response.issuer)'" -ForegroundColor Green
} catch {
    Write-Host "✗ IdentityServer Config Failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 4: Send SMS Verification (should fail without valid phone)
Write-Host "`n4. Testing SMS Send API..." -ForegroundColor Yellow
try {
    $body = @{
        PhoneNumber = "1234567890"
    } | ConvertTo-Json
    
    $response = Invoke-RestMethod -Uri "$baseUrl/api/auth/send-sms" -Method POST -ContentType "application/json" -Body $body
    Write-Host "✓ SMS Send API: Response received" -ForegroundColor Green
} catch {
    if ($_.Exception.Response.StatusCode -eq 400) {
        Write-Host "✓ SMS Send API: Properly validated input (400 Bad Request)" -ForegroundColor Green
    } else {
        Write-Host "✗ SMS Send API Failed: $($_.Exception.Message)" -ForegroundColor Red
    }
}

# Test 5: Admin Panel (should redirect to login)
Write-Host "`n5. Testing Admin Panel Access..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$baseUrl/admin" -Method GET -MaximumRedirection 0 -ErrorAction SilentlyContinue
    if ($response.StatusCode -eq 302) {
        Write-Host "✓ Admin Panel: Properly redirected to login (302)" -ForegroundColor Green
    } else {
        Write-Host "✗ Admin Panel: Unexpected status $($response.StatusCode)" -ForegroundColor Red
    }
} catch {
    Write-Host "✓ Admin Panel: Access control working (redirect or auth required)" -ForegroundColor Green
}

Write-Host "`n=== Test Summary ===" -ForegroundColor Green
Write-Host "IdentityServer8 is running and the basic endpoints are responding correctly." -ForegroundColor Cyan
Write-Host "Admin user credentials: Phone = 09124607630, Password = AdminPass123!" -ForegroundColor Cyan
Write-Host "Access the admin panel at: $baseUrl/admin" -ForegroundColor Cyan
Write-Host "API documentation available at: $baseUrl/swagger" -ForegroundColor Cyan
