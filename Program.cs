using MovieTicketBookingSystem.DAL;

var builder = WebApplication.CreateBuilder(args);

// Register DatabaseHelper for Dependency Injection
builder.Services.AddScoped<DatabaseHelper>();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add session services to the container.
builder.Services.AddDistributedMemoryCache(); // Provides in-memory storage for session data
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout duration
    options.Cookie.HttpOnly = true; // Make the cookie inaccessible to JavaScript
    options.Cookie.IsEssential = true; // Mark the session cookie as essential
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
