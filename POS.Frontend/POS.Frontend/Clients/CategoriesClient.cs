using System.Net.Http.Json;
using POS.Frontend.Models;

namespace POS.Frontend.Clients;

public class CategoriesClient : ApiClientBase
{
    public CategoriesClient(HttpClient httpClient) : base(httpClient)
    {
    }

    public Task<Result<List<CategoryModel>>> GetCategoriesAsync(CancellationToken cancellationToken = default) =>
        SendAsync<List<CategoryModel>>(new HttpRequestMessage(HttpMethod.Get, "api/categories"), cancellationToken);

    public Task<Result<CategoryModel>> GetCategoryAsync(int id, CancellationToken cancellationToken = default) =>
        SendAsync<CategoryModel>(new HttpRequestMessage(HttpMethod.Get, $"api/categories/{id}"), cancellationToken);

    public Task<Result<CategoryModel>> CreateCategoryAsync(
        CategoryFormModel category,
        CancellationToken cancellationToken = default) =>
        SendAsync<CategoryModel>(
            new HttpRequestMessage(HttpMethod.Post, "api/categories")
            {
                Content = JsonContent.Create(category)
            },
            cancellationToken);

    public Task<Result<CategoryModel>> UpdateCategoryAsync(
        int id,
        CategoryFormModel category,
        CancellationToken cancellationToken = default) =>
        SendAsync<CategoryModel>(
            new HttpRequestMessage(HttpMethod.Put, $"api/categories/{id}")
            {
                Content = JsonContent.Create(category)
            },
            cancellationToken);

    public Task<Result<CategoryModel>> DeleteCategoryAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        SendAsync<CategoryModel>(new HttpRequestMessage(HttpMethod.Delete, $"api/categories/{id}"), cancellationToken);
}
