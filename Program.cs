using Inmobiliaria.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// para bd
// builder.Services.AddDbContext<MyDbContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IRepositorioContrato, RepositorioContraro>();
builder.Services.AddScoped<IRepositorioInmueble, RepositorioInmueble>();
builder.Services.AddScoped<IRepositorioTipoInmueble, RepositorioTipoInmueble>();
builder.Services.AddScoped<IRepositorioPropietario, RepositorioPropietario>();
builder.Services.AddScoped<IRepositorioInquilino, RepositorioInquilino>();
builder.Services.AddScoped<IRepositorioPago, RepositorioPago>();
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
