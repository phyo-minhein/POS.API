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

    public Task<Result<PagedResult<ProductResponseModel>>> GetProductsPagingAsync(
        ProductParamModel param,
        CancellationToken cancellationToken = default) =>
        SendAsync<PagedResult<ProductResponseModel>>(
            new HttpRequestMessage(HttpMethod.Get, BuildPagingUri(param)),
            cancellationToken);

    public Task<Result<ProductResponseModel>> GetProductAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        SendAsync<ProductResponseModel>(
            new HttpRequestMessage(HttpMethod.Get, $"api/products/{id}"),
            cancellationToken);

    public Task<Result<ProductResponseModel>> CreateProductAsync(
        ProductRequestModel product,
        CancellationToken cancellationToken = default) =>
        SendAsync<ProductResponseModel>(
            new HttpRequestMessage(HttpMethod.Post, "api/products")
            {
                Content = JsonContent.Create(product)
            },
            cancellationToken);

    public Task<Result<ProductResponseModel>> UpdateProductAsync(
        int id,
        ProductRequestModel product,
        CancellationToken cancellationToken = default) =>
        SendAsync<ProductResponseModel>(
            new HttpRequestMessage(HttpMethod.Put, $"api/products/{id}")
            {
                Content = JsonContent.Create(product)
            },
            cancellationToken);

    public Task<Result<ProductResponseModel>> DeleteProductAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        SendAsync<ProductResponseModel>(
            new HttpRequestMessage(HttpMethod.Delete, $"api/products/{id}"),
            cancellationToken);

    private static string BuildPagingUri(ProductParamModel param)
    {
        var parameters = new List<KeyValuePair<string, string?>>();

        if (!string.IsNullOrWhiteSpace(param.Search))
        {
            parameters.Add(new("search", param.Search.Trim()));
        }

        if (param.CategoryId.HasValue)
        {
            parameters.Add(new(
                "categoryId",
                param.CategoryId.Value.ToString(CultureInfo.InvariantCulture)));
        }

        parameters.Add(new("lowStockOnly", param.LowStockOnly.ToString().ToLowerInvariant()));
        parameters.Add(new("sortBy", param.SortBy));
        parameters.Add(new("sortDescending", param.SortDescending.ToString().ToLowerInvariant()));
        parameters.Add(new("page", param.Page.ToString(CultureInfo.InvariantCulture)));
        parameters.Add(new("pageSize", param.PageSize.ToString(CultureInfo.InvariantCulture)));

        return $"api/products{QueryString.Create(parameters)}";
    }
}
