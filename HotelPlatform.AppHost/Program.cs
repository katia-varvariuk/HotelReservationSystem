var builder = DistributedApplication.CreateBuilder(args);

var mongodb = builder.AddMongoDB("mongodb")
    .WithDataVolume("mongodb-data")
    .WithMongoExpress();

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume("postgres-data")
    .WithPgAdmin();

var redis = builder.AddRedis("redis")
    .WithDataVolume("redis-data")
    .WithRedisCommander();

var mongoDatabase = mongodb.AddDatabase("hotel-reviews-dev");
var reservationDb = postgres.AddDatabase("HotelReservationDb");
var catalogDb = postgres.AddDatabase("HotelCatalogDb");

var reviewsService = builder.AddProject("reviews-service",
    "../HotelReviews.WebApi/HotelReviews.WebApi.csproj")
    .WithReference(mongodb)
    .WithReference(redis)  
    .WithEnvironment("MongoDbSettings__ConnectionString", mongodb.Resource.ConnectionStringExpression)
    .WithEnvironment("MongoDbSettings__DatabaseName", "hotel-reviews-dev");

var reservationService = builder.AddProject("reservation-service",
    "../HotelReservationSystem/HotelReservation.Api.csproj")
    .WithReference(reservationDb)
    .WithReference(redis);  

var catalogService = builder.AddProject("catalog-service",
    "../HotelCatalog.Api/HotelCatalog.Api.csproj")
    .WithReference(catalogDb)
    .WithReference(redis);  

var aggregator = builder.AddProject("aggregator",
    "../HotelPlatform.Aggregator/HotelPlatform.Aggregator.csproj")
    .WithReference(reviewsService)
    .WithReference(reservationService)
    .WithReference(catalogService)
    .WithReference(redis);  

var gateway = builder.AddProject("gateway",
    "../HotelPlatform.ApiGateway/HotelPlatform.ApiGateway.csproj")
    .WithExternalHttpEndpoints()
    .WithReference(reviewsService)
    .WithReference(reservationService)
    .WithReference(catalogService)
    .WithReference(aggregator);

builder.Build().Run();