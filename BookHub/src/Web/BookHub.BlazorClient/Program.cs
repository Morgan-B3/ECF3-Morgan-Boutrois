using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BookHub.BlazorClient;
using BookHub.BlazorClient.Services;
using Blazored.LocalStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiSettings = builder.Configuration.GetSection("ApiSettings");

// HttpClients pour chaque service
//var catalogClient = new HttpClient { BaseAddress = new Uri(apiSettings["CatalogServiceUrl"]) };
//var userClient = new HttpClient { BaseAddress = new Uri(apiSettings["UserServiceUrl"]) };
//var loanClient = new HttpClient { BaseAddress = new Uri(apiSettings["LoanServiceUrl"]) };
var httpClient = new HttpClient { BaseAddress = new Uri(apiSettings["GatewayUrl"]) };

// Enregistre les services en passant le même HttpClient
builder.Services.AddScoped<IBookService>(sp => new BookService(httpClient));
builder.Services.AddScoped<IAuthService>(sp => new AuthService(httpClient));
builder.Services.AddScoped<ILoanService>(sp => new LoanService(httpClient));

//// Enregistre les services en passant explicitement leur HttpClient
//builder.Services.AddScoped<IBookService>(sp => new BookService(catalogClient));
//builder.Services.AddScoped<IAuthService>(sp => new AuthService(userClient));
//builder.Services.AddScoped<ILoanService>(sp => new LoanService(loanClient));

builder.Services.AddScoped<AuthStateProvider>();

builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();
