using System;
using CacheManager.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MovieTicketBookingSystem.DAL;

var builder = WebApplication.CreateBuilder(args);

// Configure CacheManager
var cache = CacheFactory.Build<string>(settings =>
{
    settings.WithUpdateMode(CacheUpdateMode.Up) // Define update behavior
            .WithDictionaryHandle() // In-memory caching
            .WithExpiration(ExpirationMode.Sliding, TimeSpan.FromMinutes(30)); // Sliding expiration
});

// Register CacheManager as a Singleton
builder.Services.AddSingleton(cache);

// Register DatabaseHelper for Dependency Injection
builder.Services.AddScoped<DatabaseHelper>();

// Add services to the container
builder.Services.AddControllersWithViews();

// Add session services to the container
builder.Services.AddDistributedMemoryCache(); // Provides in-memory storage for session data
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout duration
    options.Cookie.HttpOnly = true; // Make the cookie inaccessible to JavaScript
    options.Cookie.IsEssential = true; // Mark the session cookie as essential
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Use session middleware
app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
