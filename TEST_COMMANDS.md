# Testing Commands

## Run All Tests

```powershell
dotnet test RealEstate.API.Tests
```

## Run Tests with Detailed Output

```powershell
dotnet test RealEstate.API.Tests --logger "console;verbosity=detailed"
```

## Run Specific Test Class

```powershell
# Client Service Tests
dotnet test RealEstate.API.Tests --filter "FullyQualifiedName~ClientServiceTests"

# Property Service Tests
dotnet test RealEstate.API.Tests --filter "FullyQualifiedName~PropertyServiceTests"

# Transaction Service Tests
dotnet test RealEstate.API.Tests --filter "FullyQualifiedName~TransactionServiceTests"
```

## Run Specific Test Method

```powershell
dotnet test RealEstate.API.Tests --filter "FullyQualifiedName~GetAllClientsAsync_ReturnsAllClients"
```

## Build and Test

```powershell
# Build first, then test
dotnet build RealEstate.API.Tests
dotnet test RealEstate.API.Tests --no-build
```

## Watch Mode (Auto-run on changes)

```powershell
dotnet watch test --project RealEstate.API.Tests
```
