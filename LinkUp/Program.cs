using LinkUp.Core.Application;
using LinkUp.Infrastructure.Identity;
using LinkUp.Infrastructure.Persistence;
using LinkUp.Infrastructure.Shared;
using LinkUp.Helpers;
using LinkUp.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor
builder.Services.AddControllersWithViews();

// Capas de aplicación
builder.Services.AddApplicationLayer(builder.Configuration);
builder.Services.AddPersistenceInfrastructure(builder.Configuration);
builder.Services.AddSharedInfrastructure(builder.Configuration);
builder.Services.AddIdentityInfrastructure(builder.Configuration);

builder.Services.AddScoped<IFileManager, FileManager>();

// Servicio en segundo plano para timeouts de Battleship
builder.Services.AddHostedService<TimedOutGamesBackgroundService>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/Login";
    options.Events.OnRedirectToLogin = context =>
    {
        var returnUrl = Uri.EscapeDataString(context.Request.Path + context.Request.QueryString);
        context.Response.Redirect($"/Account/Login?msg=auth&ReturnUrl={returnUrl}");
        return Task.CompletedTask;
    };
});

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
