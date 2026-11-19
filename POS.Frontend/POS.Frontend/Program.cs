using POS.Frontend.Components;
using POS.Frontend.Clients;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var apiBaseUrl = builder.Configuration["ApiBaseUrl"];

if (!Uri.TryCreate(apiBaseUrl, UriKind.Absolute, out var apiBaseAddress) ||
    (apiBaseAddress.Scheme != Uri.UriSchemeHttp && apiBaseAddress.Scheme != Uri.UriSchemeHttps))
{
    throw new InvalidOperationException("ApiBaseUrl must be an absolute HTTP or HTTPS URL.");
}

void ConfigureApiClient(HttpClient client)
{
    client.BaseAddress = apiBaseAddress;
    client.Timeout = TimeSpan.FromSeconds(10);
}

builder.Services.AddHttpClient<CategoriesClient>(ConfigureApiClient);
builder.Services.AddHttpClient<ProductsClient>(ConfigureApiClient);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
