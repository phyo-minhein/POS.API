# Category API contract

Status: Target contract for Day 2. The existing category controller and service will be updated to implement this contract in the next section.

## Existing response pattern

The project’s existing `Result<T>` response envelope and `BaseController.Execute` pattern are preserved. They are not replaced by raw DTO responses or `ProblemDetails`.

Every response with a body uses this shape:

```json
{
  "isSuccess": true,
  "isError": false,
  "isValidationError": false,
  "isSystemError": false,
  "data": {},
  "message": "Success."
}
```

| Field | Type | Description |
| --- | --- | --- |
| `isSuccess` | boolean | Indicates whether the operation succeeded. |
| `isError` | boolean | Opposite of `isSuccess`. |
| `isValidationError` | boolean | Indicates an input-validation error. |
| `isSystemError` | boolean | Indicates an unexpected server error. |
| `data` | value or `null` | Operation-specific response data. |
| `message` | string | User-readable operation result. |

## Conventions

- Base path: `/api/categories`
- Media type: `application/json`
- Public category fields are `id` and `name`; database names such as `CatId` and `CatName` are not exposed.
- Successful and unsuccessful bodies use `Result<T>`.
- HTTP status codes still describe the transport-level outcome.

## DTOs

### `CategoryResponseDto`

```json
{
  "id": 1,
  "name": "Beverages"
}
```

| Field | Type | Description |
| --- | --- | --- |
| `id` | integer | Positive database-generated category identifier. |
| `name` | string | Trimmed category name containing 1–50 characters. |

### `CreateCategoryDto`

```json
{
  "name": "Beverages"
}
```

Contains only the client-supplied name. The database generates the identifier.

### `UpdateCategoryDto`

```json
{
  "name": "Cold Beverages"
}
```

The identifier comes from the route, not the request body. Create and update use separate types so their validation and fields can evolve independently.

## List categories

```http
GET /api/categories
```

### Success

- Status: `200 OK`
- Body type: `Result<List<CategoryResponseDto>>`
- Categories are ordered by name.

```json
{
  "isSuccess": true,
  "isError": false,
  "isValidationError": false,
  "isSystemError": false,
  "data": [
    {
      "id": 1,
      "name": "Beverages"
    },
    {
      "id": 2,
      "name": "Snacks"
    }
  ],
  "message": "Categories retrieved successfully."
}
```

An empty database returns `200 OK` with `data: []`.

## Get one category

```http
GET /api/categories/{id}
```

### Success

- Status: `200 OK`
- Body type: `Result<CategoryResponseDto>`

### Not found

- Status: `404 Not Found`
- Body type: `Result<CategoryResponseDto>`

```json
{
  "isSuccess": false,
  "isError": true,
  "isValidationError": false,
  "isSystemError": false,
  "data": null,
  "message": "Category 42 does not exist."
}
```

## Create a category

```http
POST /api/categories
Content-Type: application/json
```

### Request: `CreateCategoryDto`

```json
{
  "name": "Beverages"
}
```

### Success

- Status: `201 Created`
- `Location` header: `/api/categories/{newId}`
- Body type: `Result<CategoryResponseDto>`

```json
{
  "isSuccess": true,
  "isError": false,
  "isValidationError": false,
  "isSystemError": false,
  "data": {
    "id": 1,
    "name": "Beverages"
  },
  "message": "Category created successfully."
}
```

### Invalid request

- Status: `400 Bad Request`
- Body type: `Result<CategoryResponseDto>`

```json
{
  "isSuccess": false,
  "isError": true,
  "isValidationError": true,
  "isSystemError": false,
  "data": null,
  "message": "Category name is required."
}
```

### Duplicate name

- Status: `409 Conflict`
- Body type: `Result<CategoryResponseDto>`

```json
{
  "isSuccess": false,
  "isError": true,
  "isValidationError": false,
  "isSystemError": false,
  "data": null,
  "message": "A category named 'Beverages' already exists."
}
```

## Update a category

```http
PUT /api/categories/{id}
Content-Type: application/json
```

### Request: `UpdateCategoryDto`

```json
{
  "name": "Cold Beverages"
}
```

### Success

- Status: `200 OK`
- Body type: `Result<CategoryResponseDto>`
- The response contains the updated category.

### Errors

- `400 Bad Request`: invalid name
- `404 Not Found`: category does not exist
- `409 Conflict`: another category already uses the requested name

Every error body remains a `Result<CategoryResponseDto>`.

## Delete a category

```http
DELETE /api/categories/{id}
```

### Success

- Status: `200 OK`
- Body type: `Result<CategoryResponseDto>`
- `data` contains the deleted category or is `null`, depending on the existing service convention selected during implementation.

### Errors

- `404 Not Found`: category does not exist
- `409 Conflict`: one or more products use the category

Example conflict:

```json
{
  "isSuccess": false,
  "isError": true,
  "isValidationError": false,
  "isSystemError": false,
  "data": null,
  "message": "The category cannot be deleted because products use it."
}
```

## Unexpected server error

- Status: `500 Internal Server Error`
- Body: existing `Result<T>` system-error envelope

```json
{
  "isSuccess": false,
  "isError": true,
  "isValidationError": false,
  "isSystemError": true,
  "data": null,
  "message": "An unexpected error occurred."
}
```

Production responses must not expose stack traces, connection strings, or other secrets.

## Business and validation rules

1. The server trims leading and trailing whitespace before comparing or saving a name.
2. A name is required after trimming.
3. A name cannot exceed 50 characters.
4. Category names are unique using a case-insensitive comparison.
5. Updating a category to its current name is allowed.
6. A category referenced by a product cannot be deleted.
7. Route identifiers must be positive integers.

## Endpoint summary

| Method | Path | Success | Expected errors | Response pattern |
| --- | --- | --- | --- | --- |
| `GET` | `/api/categories` | `200` | — | `Result<List<CategoryResponseDto>>` |
| `GET` | `/api/categories/{id}` | `200` | `404` | `Result<CategoryResponseDto>` |
| `POST` | `/api/categories` | `201` | `400`, `409` | `Result<CategoryResponseDto>` |
| `PUT` | `/api/categories/{id}` | `200` | `400`, `404`, `409` | `Result<CategoryResponseDto>` |
| `DELETE` | `/api/categories/{id}` | `200` | `404`, `409` | `Result<CategoryResponseDto>` |

## Out of scope for this slice

- Replacing or redesigning `Result<T>` or `BaseController.Execute`
- Authentication and role authorization
- Category search and pagination
- Bulk create, update, or delete
- Optimistic concurrency and ETags
- Localization of validation messages
