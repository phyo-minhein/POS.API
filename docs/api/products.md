# Product API contract

## Conventions

- Base path: `/api/products`
- Media type: `application/json`
- Every response body preserves the existing `Result<T>` envelope.
- Database property names such as `ProductId`, `ProductName`, and `CatId` are not exposed publicly.

## Product response

`ProductResponseDto` has this shape:

```json
{
  "id": 1,
  "name": "Shark Energy Drink",
  "code": "001",
  "categoryId": 2,
  "categoryName": "Beverages",
  "price": 2500.00,
  "stockQuantity": 0,
  "reorderLevel": 5,
  "isLowStock": true
}
```

`isLowStock` is true when `stockQuantity` is less than or equal to `reorderLevel`.

## Create and update requests

`CreateProductDto` and `UpdateProductDto` contain only client-editable fields:

```json
{
  "name": "Shark Energy Drink",
  "code": "001",
  "categoryId": 2,
  "price": 2500.00,
  "stockQuantity": 20,
  "reorderLevel": 5
}
```

The identifier is database-generated for creates and comes from the route for updates.

## Current endpoints

| Method | Path | Request | Response data |
| --- | --- | --- | --- |
| `GET` | `/api/products` | — | `List<ProductResponseDto>` |
| `GET` | `/api/products/{id}` | — | `ProductResponseDto` |
| `POST` | `/api/products` | `CreateProductDto` | `ProductResponseDto` |
| `PUT` | `/api/products/{id}` | `UpdateProductDto` | `ProductResponseDto` |
| `DELETE` | `/api/products/{id}` | — | `ProductResponseDto` or `null` |

Validation, conflict handling, correct not-found semantics, `201 Created`, and cancellation tokens belong to Day 3 task 3.

## Planned paged list contract

`ProductQueryDto` defines the upcoming query parameters:

- `search`
- `categoryId`
- `lowStockOnly`
- `sortBy`
- `sortDescending`
- `page`
- `pageSize`

After Day 3 task 4, the list endpoint will return `PagedResult<ProductResponseDto>` containing:

- `items`
- `page`
- `pageSize`
- `totalCount`
- `totalPages`

Search, filtering, sorting, and paging behavior are intentionally not active yet.
