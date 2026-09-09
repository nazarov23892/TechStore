using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TechStore.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


var apiBaseUri = builder.Configuration["ApiBaseUrl"];
builder.Services.AddScoped(
    sp => new HttpClient 
    { 
        BaseAddress = new Uri(apiBaseUri ?? builder.HostEnvironment.BaseAddress) 
    });

await builder.Build().RunAsync();
