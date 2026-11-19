using System.Globalization;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using POS.Frontend.Models;

namespace POS.Frontend.Clients;

public class ProductsClient : ApiClientBase
{
    public ProductsClient(HttpClient httpClient) : base(httpClient)
    {
    }

    public Task<Result<PagedResult<ProductModel>>> GetProductsPagingAsync(
        ProductQueryModel query,
        CancellationToken cancellationToken = default) =>
        SendAsync<PagedResult<ProductModel>>(
            new HttpRequestMessage(HttpMethod.Get, BuildPagingUri(query)),
            cancellationToken);

    public Task<Result<ProductModel>> GetProductAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        SendAsync<ProductModel>(
            new HttpRequestMessage(HttpMethod.Get, $"api/products/{id}"),
            cancellationToken);

    public Task<Result<ProductModel>> CreateProductAsync(
        ProductFormModel product,
        CancellationToken cancellationToken = default) =>
        SendAsync<ProductModel>(
            new HttpRequestMessage(HttpMethod.Post, "api/products")
            {
                Content = JsonContent.Create(product)
            },
            cancellationToken);

    public Task<Result<ProductModel>> UpdateProductAsync(
        int id,
        ProductFormModel product,
        CancellationToken cancellationToken = default) =>
        SendAsync<ProductModel>(
            new HttpRequestMessage(HttpMethod.Put, $"api/products/{id}")
            {
                Content = JsonContent.Create(product)
            },
            cancellationToken);

    public Task<Result<ProductModel>> DeleteProductAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        SendAsync<ProductModel>(
            new HttpRequestMessage(HttpMethod.Delete, $"api/products/{id}"),
            cancellationToken);

    private static string BuildPagingUri(ProductQueryModel query)
    {
        var parameters = new List<KeyValuePair<string, string?>>();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            parameters.Add(new("search", query.Search.Trim()));
        }

        if (query.CategoryId.HasValue)
        {
            parameters.Add(new(
                "categoryId",
                query.CategoryId.Value.ToString(CultureInfo.InvariantCulture)));
        }

        parameters.Add(new("lowStockOnly", query.LowStockOnly.ToString().ToLowerInvariant()));
        parameters.Add(new("sortBy", query.SortBy));
        parameters.Add(new("sortDescending", query.SortDescending.ToString().ToLowerInvariant()));
        parameters.Add(new("page", query.Page.ToString(CultureInfo.InvariantCulture)));
        parameters.Add(new("pageSize", query.PageSize.ToString(CultureInfo.InvariantCulture)));

        return $"api/products{QueryString.Create(parameters)}";
    }
}
