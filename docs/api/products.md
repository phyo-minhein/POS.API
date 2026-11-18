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

| Method | Path | Request | Success | Response data |
| --- | --- | --- | --- | --- |
| `GET` | `/api/products` | Query parameters | `200 OK` | `PagedResult<ProductResponseDto>` |
| `GET` | `/api/products/{id}` | — | `200 OK` | `ProductResponseDto` |
| `POST` | `/api/products` | `CreateProductDto` | `201 Created` | `ProductResponseDto` |
| `PUT` | `/api/products/{id}` | `UpdateProductDto` | `200 OK` | `ProductResponseDto` |
| `DELETE` | `/api/products/{id}` | — | `200 OK` | `ProductResponseDto` |

The create response includes a `Location` header for the new product. Invalid input returns `400`, a missing product returns `404`, and duplicate product codes or deletion of a product used in a sale returns `409`. All async endpoints accept and pass through the request cancellation token.

## Business and validation rules

- Product name is required and cannot exceed 100 characters.
- Product code is required, cannot exceed 10 characters, and is unique without regard to casing.
- Category is optional; when selected, it must exist.
- Price must be between `0` and `99,999,999.99`.
- Stock quantity and reorder level cannot be negative.
- A product that has been used in a sale cannot be deleted.
- Names and codes are trimmed before they are stored.

## Product list query

`GET /api/products` accepts these query parameters:

- `search`: partial product-name or product-code search, limited to 100 characters.
- `categoryId`: returns only products in that category.
- `lowStockOnly`: when `true`, returns products whose stock is at or below their reorder level.
- `sortBy`: `name`, `code`, `category`, `price`, `stockQuantity`, or `reorderLevel`. The default is `name`.
- `sortDescending`: reverses the selected sort order.
- `page`: one-based page number. The default is `1`.
- `pageSize`: number of products per page from `1` to `100`. The default is `10`.

Filters can be combined. Sorting always includes product ID as a stable secondary order so that products do not move unpredictably between pages.

The response data is `PagedResult<ProductResponseDto>`:

- `items`
- `page`
- `pageSize`
- `totalCount`
- `totalPages`
