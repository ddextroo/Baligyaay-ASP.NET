var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor(); // Add IHttpContextAccessor
builder.Services.AddScoped<SessionManager>(); // Add SessionManager

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".Baligyaay.Session";
    options.IdleTimeout = TimeSpan.FromDays(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
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

app.UseRouting();
app.UseAuthorization();
app.UseSession(); // Place this after UseAuthorization and before UseEndpoints

app.MapRazorPages();
app.MapDefaultControllerRoute();

// app.MapControllerRoute(
//     name: "login",
//     pattern: "{controller=Home}/{action=Login}/{id?}");

app.Run();
