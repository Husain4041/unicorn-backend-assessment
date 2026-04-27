````md
# Unicorn Backend Assessment – Claims Processing API

## 1. About the Project

This project is a **.NET 10 Web API** that simulates an insurance claims processing system.

It is designed around a **two-stage workflow system**:

- **Makers**: Review and validate incoming claims
- **Checkers**: Perform final verification and approval

The system supports:
- Claim ingestion
- Role-based workflows
- State transitions
- Concurrency-safe assignment
- Paginated and filtered retrieval of claims

The backend uses:
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- Docker (for containerized deployment)

---

## 2. Setup & Run Instructions

### Prerequisites

Make sure you have installed:

- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- Git

---

### Running with Docker (Recommended)

From the project root:

```bash
docker compose up --build
````

This will:

* Start PostgreSQL database
* Build the .NET API
* Run both services together

---

### Access the API

Once running:

* API Base URL:

```
http://localhost:5000
```

* Swagger UI:

```
http://localhost:5000/swagger
```

---

### Running Locally (Without Docker)

1. Start PostgreSQL locally
2. Update connection string in `appsettings.json`:

```
Host=localhost;Port=5432;Database=claimsdb;Username=postgres;Password=postgres
```

3. Run migrations:

```bash
dotnet ef database update
```

4. Start API:

```bash
dotnet run
```

---

## 3. API Design

### Base URL

```
/api/v1
```

---

## Overview

This API supports a structured insurance claim workflow involving:

* Claim creation
* Maker review
* Checker approval
* Final completion

---

## 1. Claim Ingestion

### Create Claim

**POST** `/claims`

Request:

```json
{
  "patientName": "John Doe",
  "patientAge": 45,
  "patientNationality": "UAE",
  "insuranceCompanyId": 1,
  "claimType": "Surgery",
  "medicalReason": "Appendicitis",
  "claimAmount": 5000
}
```

Response:

```json
{
  "id": 1,
  "status": "New",
  "createdAt": "2026-01-01T10:00:00Z"
}
```

---

## 2. Maker Flow

### Get New Claims

**GET** `/claims?status=New`

Response:

```json
[
  {
    "id": 1,
    "patientName": "John Doe",
    "claimAmount": 5000,
    "status": "New"
  }
]
```

---

### Assign Claim

**POST** `/claims/{id}/assign`

Request:

```json
{
  "makerId": 101
}
```

Response:

```json
{
  "id": 1,
  "status": "AssignedToMaker"
}
```

Errors:

* 404 → Claim not found
* 409 → Already assigned

---

### Maker Review

**POST** `/claims/{id}/maker-review`

Request:

```json
{
  "makerId": 101,
  "recommendation": "Approve",
  "feedback": "All documents valid"
}
```

Response:

```json
{
  "claimId": 1,
  "status": "MakerReviewed"
}
```

Errors:

* 400 → Invalid state transition
* 403 → Unauthorized role
* 404 → Not found

---

## 3. Checker Flow

### Get Maker Reviewed Claims

**GET** `/claims?status=MakerReviewed`

---

### Get Claim Details

**GET** `/claims/{id}`

Response:

```json
{
  "id": 1,
  "patientName": "John Doe",
  "claimAmount": 5000,
  "status": "MakerReviewed",
  "makerReview": {
    "makerId": 101,
    "recommendation": "Approve",
    "feedback": "All documents valid",
    "createdAt": "2026-01-01T11:00:00Z"
  }
}
```

---

### Checker Review

**POST** `/claims/{id}/checker-review`

Request:

```json
{
  "checkerId": 201,
  "decision": "Approve",
  "feedback": "Verified and approved"
}
```

Response:

```json
{
  "claimId": 1,
  "status": "Completed"
}
```

---

## 4. Claim History & Filtering

### Get Claims (Paginated)

**GET** `/claims`

Query Parameters:

* status
* insuranceCompanyId
* fromDate
* toDate
* page (default: 1)
* pageSize (default: 10)

Example:

```
GET /claims?status=Completed&page=1&pageSize=10
```

Response:

```json
{
  "data": [
    {
      "id": 1,
      "status": "Completed",
      "claimAmount": 5000
    }
  ],
  "page": 1,
  "pageSize": 10,
  "total": 100
}
```

---

## 5. Status Codes

* 200 → Success
* 201 → Created
* 400 → Bad Request
* 403 → Forbidden
* 404 → Not Found
* 409 → Conflict
* 500 → Server Error

---

## 6. State Transitions

Valid flow:

* New → AssignedToMaker
* AssignedToMaker → MakerReviewed
* MakerReviewed → Completed

Invalid actions:

* Skipping steps
* Reprocessing completed claims
* Double assignment

---

## Notes

* All timestamps use ISO 8601 (UTC)
* Maker/Checker actions are immutable once submitted
* Concurrency is enforced during assignment and transitions
* Pagination is required for list endpoints

