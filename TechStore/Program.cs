using Microsoft.EntityFrameworkCore;
using TechStore.Data;
using TechStore.Services;

var builder = WebApplication.CreateBuilder(args);

// ─── Servicios ───────────────────────────────────────────────
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<TechStoreDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("TechStoreConnection")));

builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();

// ─── Construcción de la app ──────────────────────────────────
var app = builder.Build();

// ─── Pipeline de middleware ──────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
