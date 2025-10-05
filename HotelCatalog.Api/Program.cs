using Microsoft.EntityFrameworkCore;
using HotelCatalog.Dal.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<HotelCatalogDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("HotelCatalogDB")));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<HotelCatalogDbContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();