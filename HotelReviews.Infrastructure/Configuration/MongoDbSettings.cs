namespace HotelReviews.Infrastructure.Configuration;
public class MongoDbSettings
{
    public const string SectionName = "MongoDbSettings";
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = "hotel_reviews";
    public int ConnectionTimeoutSeconds { get; set; } = 30;
    public int OperationTimeoutSeconds { get; set; } = 30;
    public int MaxConnectionPoolSize { get; set; } = 100;
    public bool UseSsl { get; set; } = false;
}