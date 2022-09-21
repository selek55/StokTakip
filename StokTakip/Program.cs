using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StokTakip.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<StokTakipContext>(o => o.UseInMemoryDatabase("StokTakipDb"));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.WebHost
    .UseKestrel(options =>
    {
        options.ListenAnyIP(5000);
        options.ListenAnyIP(5001, configure => configure.UseHttps()); 
    })
    .UseUrls("http://*:5000", "https://*:5001");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); 
    app.UseHsts();
}

//app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

void OpenBrowser(string url)
{
    Console.WriteLine(url);

    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {
        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
    }
    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
    {
        Process.Start("xdg-open", url);
    }
    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
    {
        Process.Start("open", url);
    }
    else
    {
        // throw 
    }
}

app.Start();

OpenBrowser("http://localhost:5000/");

app.WaitForShutdown();