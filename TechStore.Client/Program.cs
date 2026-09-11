using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TechStore.Client;
using TechStore.Client.HttpServices;
using TechStore.Client.HttpServices.Concrete;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Http clients.
var apiBaseUri = builder.Configuration["ApiBaseUrl"];
builder.Services.AddHttpClient<IProductHttpService, ProductHttpService>(
    client =>
    {
        client.BaseAddress = new Uri(apiBaseUri ?? builder.HostEnvironment.BaseAddress);
    });

await builder.Build().RunAsync();
