# REQUIREMENTS ANALYSIS

---

## Entities:

### 1. Claim
- Id  
- PatientName  
- PatientAge  
- PatientNationality  
- InsuranceCompanyId (FK)  
- ClaimType  
- MedicalReason  
- ClaimAmount  
- Status (New, AssignedToMaker, MakerReviewed, CheckerReviewed, Completed)  
- CreatedAt  
- UpdatedAt  

### 2. MakerReview
- Id  
- ClaimId (FK)  
- MakerId  
- Recommendation (Approve / Reject)  
- Feedback  
- CreatedAt  

### 3. CheckerReview
- Id  
- ClaimId (FK)  
- CheckerId  
- Decision (Approve / Reject)  
- Feedback  
- CreatedAt  

### 4. InsuranceCompany
- Id  
- Name  

### 5. User
- Id  
- Name  
- Role (Maker / Checker)  

---

## Actors and Actions:

1. Maker  
- Reviews claims  
- Adds feedback  
- Submits recommendation (Approve/Reject)  

2. Checker  
- Reviews Maker's decision  
- Adds feedback  
- Submits final decision  

3. System  
- Stores claims  
- Manages workflow and state transitions  
- Ensures concurrency control  
- Logs forwarding to insurance company  

4. Insurance Company (External)  
- Receives finalized claim data (no direct interaction required)  

---

## Functional Requirements:

ID: FR-001  
Requirement: The system shall accept structured claim data input and create a new claim with status "New".  
Priority: High  

ID: FR-002  
Requirement: The system shall allow a Maker to retrieve a list of claims with status "New".  
Priority: High  

ID: FR-003  
Requirement: The system shall assign a claim to a Maker and update its status to "AssignedToMaker".  
Priority: High  

ID: FR-004  
Requirement: The system shall allow a Maker to submit a review with recommendation (Approve/Reject) and feedback.  
Priority: High  

ID: FR-005  
Requirement: The system shall update the claim status to "MakerReviewed" after a Maker submits a review.  
Priority: High  

ID: FR-006  
Requirement: The system shall allow a Checker to retrieve claims with status "MakerReviewed".  
Priority: High  

ID: FR-007  
Requirement: The system shall allow a Checker to view Maker feedback and recommendation for a claim.  
Priority: High  

ID: FR-008  
Requirement: The system shall allow a Checker to submit a final decision (Approve/Reject) with feedback.  
Priority: High  

ID: FR-009  
Requirement: The system shall update the claim status to "CheckerReviewed" and then "Completed" after Checker decision.  
Priority: High  

ID: FR-010  
Requirement: The system shall enforce valid claim state transitions and prevent skipping workflow steps.  
Priority: High  

ID: FR-011  
Requirement: The system shall provide a paginated list of claims.  
Priority: Medium  

ID: FR-012  
Requirement: The system shall allow filtering of claims by status, insurance company, and date range.  
Priority: Medium  

ID: FR-013  
Requirement: The system shall log or mark claims as forwarded after final decision by Checker.  
Priority: Medium  

---

## Non-Functional Requirements:

ID: NFR-001  
Requirement: The system shall prevent multiple Makers from being assigned to the same claim concurrently.  
Priority: High  

ID: NFR-002  
Requirement: The system shall ensure atomic and consistent claim state transitions under concurrent access.  
Priority: High  

ID: NFR-003  
Requirement: The system shall enforce data integrity by restricting invalid workflow transitions.  
Priority: High  

ID: NFR-004  
Requirement: The system shall record all user actions with timestamps and user identifiers for auditability.  
Priority: High  

ID: NFR-005  
Requirement: The system shall support processing of hundreds to thousands of claims per day without degradation.  
Priority: Medium  

ID: NFR-006  
Requirement: The system shall return paginated responses for large datasets to ensure performance.  
Priority: Medium  

ID: NFR-007  
Requirement: The system shall be scalable to support increasing users and claim volume.  
Priority: Medium  

ID: NFR-008  
Requirement: The system shall maintain data consistency in case of failures or partial updates.  
Priority: High  

---

## Assumptions:

1. Each claim is handled by exactly one Maker and one Checker  
2. Reviews are immutable once submitted  
3. Upstream service provides valid structured data  
4. No real integration with insurance companies is required  
5. Claims cannot be modified after Checker decision  
6. A claim must go through Maker before Checker  

---

## Edge Cases:

1. Multiple Makers attempt to claim the same claim simultaneously  
2. Maker does not complete review after assignment  
3. Checker attempts to review before Maker completes  
4. Duplicate claim ingestion  
5. Claim updated after Maker review (should be restricted)  
6. Invalid state transitions (e.g., skipping steps)  
7. Concurrent updates to the same claim 