using HotelPlatform.Aggregator.Clients;
var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddRedisClient("redis");
builder.Services.AddControllers();

//builder.Services.AddGrpcClient<ReviewsService.ReviewsServiceClient>("reviews-service", o =>
//{
//    o.Address = new Uri("http://reviews-service");
//});

builder.Services.AddHttpClient<IReviewsClient, ReviewsClient>(client =>
{
    client.BaseAddress = new Uri("http://reviews-service");
});

builder.Services.AddHttpClient<IReservationsClient, ReservationsClient>(client =>
{
    client.BaseAddress = new Uri("http://reservation-service");
});

builder.Services.AddHttpClient<ICatalogClient, CatalogClient>(client =>
{
    client.BaseAddress = new Uri("http://catalog-service");
});
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Hotel Aggregator API",
        Version = "v1",
        Description = "Aggregates data from multiple hotel microservices"
    });
});
var app = builder.Build();

app.UseServiceDefaults();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();