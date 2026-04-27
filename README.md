# API DESIGN

Base URL: /api/v1

---

## Overview

This API supports the ingestion, review, and processing of insurance claims through a two-stage workflow involving Makers and Checkers.

---

## 1. Claim Ingestion

### POST /claims

Creates a new claim.

Request Body:
{
  "patientName": "John Doe",
  "patientAge": 45,
  "patientNationality": "UAE",
  "insuranceCompanyId": 1,
  "claimType": "Surgery",
  "medicalReason": "Appendicitis",
  "claimAmount": 5000
}

Response (201 Created):
{
  "id": 1,
  "status": "New",
  "createdAt": "2026-01-01T10:00:00Z"
}

---

## 2. Maker Flow

### GET /claims?status=New

Retrieve claims available for Makers.

Response (200 OK):
[
  {
    "id": 1,
    "patientName": "John Doe",
    "claimAmount": 5000,
    "status": "New"
  }
]

---

### POST /claims/{id}/assign

Assign a claim to a Maker.

Request Body:
{
  "makerId": 101
}

Response (200 OK):
{
  "id": 1,
  "status": "AssignedToMaker"
}

Errors:
- 404 Not Found → Claim does not exist
- 409 Conflict → Claim already assigned

---

### POST /claims/{id}/maker-review

Submit Maker review.

Request Body:
{
  "makerId": 101,
  "recommendation": "Approve",
  "feedback": "All documents valid"
}

Response (200 OK):
{
  "claimId": 1,
  "status": "MakerReviewed"
}

Errors:
- 400 Bad Request → Invalid state transition
- 403 Forbidden → Unauthorized role
- 404 Not Found → Claim not found

---

## 3. Checker Flow

### GET /claims?status=MakerReviewed

Retrieve claims available for Checkers.

Response (200 OK):
[
  {
    "id": 1,
    "status": "MakerReviewed"
  }
]

---

### GET /claims/{id}

Retrieve full claim details including Maker review.

Response (200 OK):
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

Errors:
- 404 Not Found → Claim not found

---

### POST /claims/{id}/checker-review

Submit Checker decision.

Request Body:
{
  "checkerId": 201,
  "decision": "Approve",
  "feedback": "Verified and approved"
}

Response (200 OK):
{
  "claimId": 1,
  "status": "Completed"
}

Errors:
- 400 Bad Request → Invalid state transition
- 403 Forbidden → Unauthorized role
- 404 Not Found → Claim not found

---

## 4. Claim History & Filtering

### GET /claims

Retrieve paginated and filtered claims.

Query Parameters:
- status
- insuranceCompanyId
- fromDate
- toDate
- page (default: 1)
- pageSize (default: 10)

Example:
GET /claims?status=Completed&page=1&pageSize=10

Response (200 OK):
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

---

## 5. Forwarding Representation

Forwarding is handled internally by the system after Checker decision.

Behavior:
- When Checker submits decision → claim marked as "Completed"
- System logs forwarding event internally

Optional Representation:
{
  "claimId": 1,
  "forwarded": true,
  "forwardedAt": "2026-01-01T12:00:00Z"
}

---

## 6. Status Codes

- 200 OK → Successful request
- 201 Created → Resource successfully created
- 400 Bad Request → Validation or invalid state transition
- 403 Forbidden → Unauthorized action based on role
- 404 Not Found → Resource not found
- 409 Conflict → Concurrency issue or duplicate action
- 500 Internal Server Error → Unexpected server error

---

## 7. State Transition Rules

Valid transitions:

- New → AssignedToMaker
- AssignedToMaker → MakerReviewed
- MakerReviewed → Completed

Invalid transitions:
- Skipping Maker review
- Re-reviewing completed claims
- Assigning already assigned claims

Invalid transitions result in:
- 400 Bad Request OR
- 409 Conflict

---

## Notes

- All timestamps are in ISO 8601 format (UTC)
- Reviews are immutable once submitted
- Concurrency is handled at assignment and state transition level
- Pagination is required for all list endpoints
