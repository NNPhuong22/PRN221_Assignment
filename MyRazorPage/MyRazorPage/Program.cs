// Gọi phương thức để khởi tạo web server: Kestrel
using MyRazorPage.HubCustom;
using MyRazorPage.Models;
using SignalRLab.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
// Bổ sung kiến trúc Razor vào container của web server
builder.Services.AddRazorPages();
builder.Services.AddScoped(typeof(PRN221DBContext));
builder.Services.AddSession();
builder.Services.AddSignalR();

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles
);
// Build web server
var app = builder.Build();

app.UseStaticFiles();
// Ánh xạ xử lý request của user tới website dựa trên kiến trúc Razor
app.UseSession();
app.UseHttpsRedirection();
app.MapRazorPages();
app.UseRouting();



app.UseStatusCodePages("text/html", "<div >\r\n\t\t<div>\r\n\t\t\t<div >\r\n\t\t\t<h3 style=\"margin-left:580px;margin-top:60px;font-size:40px;margin-bottom:50px\">Oops! Page not found</h3>\r\n\t\t\t<h1 style=\"margin-left:680px;margin-bottom:50px\">\r\n\t\t\t\t<span style=\"font-size:100px\">4</span>\r\n\t\t\t\t<span style =\"font-size:100px\">0</span>\r\n\t\t\t\t<span style=\"font-size:100px;\">4</span>\r\n\t\t\t</h1>\r\n\t\t\t</div>\r\n\t\t<h2 style=\"margin-left:400px;margin-bottom:100px\">We are sorry, but the page you requested was not found <a asp-page=\"/Index\">Return page</a></h2>\r\n\t\t</div>\r\n\t</div>");



app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    endpoints.MapHub<SignalrServer>("/signalrServer");
});
app.MapHub<HubServer>("/hub");

// Thực thi ứng dụng web bằng web server
app.Run();
