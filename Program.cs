using MYPRODUCTPRO.Models;

var builder = WebApplication.CreateBuilder(args);

// Register IHttpContextAccessor in DI container
builder.Services.AddHttpContextAccessor();

// Register DataAccessLayer_Class as a service
builder.Services.AddScoped<DataAccessLayer_Class>();

// Add session service
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);  // Set session timeout if needed
    options.Cookie.HttpOnly = true;  // Cookie settings
});

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Enable session in the HTTP request pipeline
app.UseSession();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Authentication}/{action=LoginPage}/{id?}");

app.Run();
