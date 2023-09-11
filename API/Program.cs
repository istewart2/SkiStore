using API.Data;
using API.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// add services to container
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebAPIv5", Version = "v1" });
});

builder.Services.AddDbContext<StoreContext>(opt =>  
{ 
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")); 
});

builder.Services.AddCors();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (builder.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
    });
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(opt => {
    opt.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4444");
});

app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
try
{
    await context.Database.MigrateAsync();
    DbInitializer.Initialize(context);
}
catch (Exception ex)
{
    logger.LogError(ex, "Problem migrating data");
}

app.Run();