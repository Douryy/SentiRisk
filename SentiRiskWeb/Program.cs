using SentiRiskWeb.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:5123");

builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient<ApiClientService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5089/");
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();
