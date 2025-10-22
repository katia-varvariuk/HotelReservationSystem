using HotelReviews.Domain.Interfaces;
using HotelReviews.Infrastructure.Configuration;
using HotelReviews.Infrastructure.Persistence;
using HotelReviews.Infrastructure.Persistence.Indexes;
using HotelReviews.Infrastructure.Persistence.Repositories;
using HotelReviews.Infrastructure.Seeding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HotelReviews.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(
            configuration.GetSection(MongoDbSettings.SectionName));
        services.AddSingleton<MongoDbContext>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IRequestRepository, RequestRepository>();
        services.AddSingleton<MongoIndexCreator>();
        services.AddScoped<IDataSeeder, MongoDataSeeder>();
        return services;
    }
    public static async Task InitializeMongoDbAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var indexCreator = scope.ServiceProvider.GetRequiredService<MongoIndexCreator>();
        await indexCreator.CreateIndexesAsync();
        var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
        await seeder.SeedAsync();
    }
}