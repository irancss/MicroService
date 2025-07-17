# Simple IdentityServer8 Test
$baseUrl = "http://localhost:5062"

Write-Host "Testing IdentityServer8..." -ForegroundColor Green

# Test Health Check
Write-Host "Health Check..." -ForegroundColor Yellow
try {
    $health = Invoke-RestMethod -Uri "$baseUrl/health" -Method GET
    Write-Host "Health: $health" -ForegroundColor Green
} catch {
    Write-Host "Health check failed" -ForegroundColor Red
}

# Test Home Page
Write-Host "Home Page..." -ForegroundColor Yellow
try {
    $home = Invoke-WebRequest -Uri "$baseUrl/" -Method GET
    Write-Host "Home Status: $($home.StatusCode)" -ForegroundColor Green
} catch {
    Write-Host "Home page failed" -ForegroundColor Red
}

# Test IdentityServer Config
Write-Host "IdentityServer Config..." -ForegroundColor Yellow
try {
    $config = Invoke-RestMethod -Uri "$baseUrl/.well-known/openid_configuration" -Method GET
    Write-Host "Issuer: $($config.issuer)" -ForegroundColor Green
} catch {
    Write-Host "Config failed" -ForegroundColor Red
}

Write-Host "Testing complete!" -ForegroundColor Green
