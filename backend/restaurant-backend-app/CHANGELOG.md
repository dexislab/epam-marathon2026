# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.1.0] - yyyy-MM-dd

### Added
- **Level 1: Owner group-session snapshot.** Added `GET /guest/groups/{groupReservationId}/sessions` so the reservation owner can retrieve the current group pre-order snapshot. The response includes each participant's session ID, participant type, display name, dish details and quantities, participant subtotal, and submission state, plus the total across participants.
- Added owner authorization for the group-session snapshot. The handler requires the authenticated Cognito user ID, validates that the group and its parent reservation exist, and rejects callers who do not own the reservation. Missing authentication, non-owner access, and missing group or reservation are returned as the corresponding `401`, `403`, and `404` responses.
- Added a reusable group-snapshot builder shared by the GET and existing DELETE flows. Snapshot responses use camelCase JSON and resolve the owner participant's display name from the owner's profile; guest participants continue to use their guest names.
- Registered the Level 1 route in the Lambda route registry and corrected the API Gateway resource from the singular `/guest/group/{groupReservationId}/sessions` path to `/guest/groups/{groupReservationId}/sessions`, retaining Cognito authorization.
- Updated the OpenAPI response schema for participant items to match the runtime's nested `{ dish, quantity }` shape and documented the owner-only behavior and error responses. Updated the API routes guide with the homework response example.
- **Level 2: Owner guest-session lookup.** Added `GET /reservations/{reservationId}/guest-session-id`. It verifies the reservation owner and returns the existing owner guest-session ID, creating and saving an owner session with an empty dish list when one does not yet exist. Missing reservations and groups return `404`; non-owners receive `403`.
- Owner-session initialization now runs when a group reservation is created and is ensured for legacy groups when the owner snapshot or session-ID endpoint is requested. The owner-ID response remains unchanged; internally the authenticated Cognito bearer token hash is associated with the owner session so the existing dish-add and cart-submit authorization flow can record owner activity.
- **Level 2: Consolidated group pre-order submission.** Added `POST /group-reservations/{groupReservationId}/submit`, accepting a `reservationId` request body. The service verifies the group, reservation association, owner identity, and group status before submission.
- Group submission includes only sessions marked submitted, merges quantities for matching `dishId` values, writes the merged lines to the reservation's order as pending items, changes the order status to `SubmittedByCustomer`, and changes the group-reservation status to `Submitted`. The response is the merged list of `{ dishId, quantity }` entries.
- Added validation for missing groups or reservations, mismatched reservation/group IDs, non-owner requests, already submitted or closed groups, absence of submitted sessions, submitted sessions with no dishes, and missing orders. These cases are mapped to the documented `400`, `403`, or `404` responses.
- Added both Level 2 endpoints to the Lambda route registry and API Gateway deployment configuration with Cognito authorization. Updated the OpenAPI paths, security declarations, request/response schemas, status codes, and API routes guide to use the homework paths and payloads.
- Updated guest-session repository counting to exclude persisted owner-session rows; the participant-capacity calculation then adds the owner once. This keeps stored owner sessions from consuming a guest seat. Owner sessions are also represented as `Owner` participants in the snapshot.
- Added service tests for owner-session reuse and creation, owner-only access, snapshot owner mapping, submitted-session filtering, dish aggregation, order/group status updates, capacity counting, and submit validation failures.
- **Level 3: Owner guest-session removal.** Completed `DELETE /guest/groups/{groupReservationId}/sessions/{guestSessionId}` so only the reservation owner can remove a guest session, and the response is the refreshed group participant snapshot with recalculated participant and group totals.
- The delete operation returns `404` for a missing group, parent reservation, or session; returns `400` if the session belongs to a different group; and now rejects attempts to delete the owner's own session so owner-session lookup and future owner actions remain available.
- Updated DELETE response serialization to use the same camelCase JSON shape as the Level 1 snapshot endpoint. Updated the OpenAPI response schema/status documentation and API routes guide to show the homework snapshot contract and owner-only/error behavior.

## [1.0.0] - yyyy-MM-dd

### Added
    -  Added items

### Changed
    -  Changed items 

### Removed
    -  Removed items 
