using HotelReviews.Domain.Common;
using HotelReviews.Domain.Exceptions;
using HotelReviews.Domain.ValueObjects;
using MongoDB.Bson.Serialization.Attributes;

namespace HotelReviews.Domain.Entities;

[BsonCollection("reviews")]
public class Review : BaseEntity
{
    [BsonElement("clientId")]
    public int ClientId { get; private set; }

    [BsonElement("roomId")]
    public int RoomId { get; private set; }

    [BsonElement("rating")]
    public Rating Rating { get; private set; }

    [BsonElement("comment")]
    public string Comment { get; private set; }

    [BsonElement("date")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime Date { get; private set; }

    [BsonElement("isVerified")]
    public bool IsVerified { get; private set; }

    [BsonElement("isApproved")]
    public bool IsApproved { get; private set; }

    [BsonElement("rejectionReason")]
    public string? RejectionReason { get; private set; }

    private Review()
    {
        Comment = string.Empty;
        Rating = Rating.Create(5);
    }

    private Review(int clientId, int roomId, Rating rating, string comment)
    {
        ClientId = clientId;
        RoomId = roomId;
        Rating = rating;
        Comment = comment;
        Date = DateTime.UtcNow;
        IsVerified = false;
        IsApproved = false;
    }

    public static Review Create(int clientId, int roomId, int rating, string comment)
    {
        if (clientId <= 0)
            throw new ValidationException(nameof(ClientId), "ID клієнта повинен бути додатнім числом");

        if (roomId <= 0)
            throw new ValidationException(nameof(RoomId), "ID кімнати повинен бути додатнім числом");

        if (string.IsNullOrWhiteSpace(comment))
            throw new ValidationException(nameof(Comment), "Коментар не може бути порожнім");

        if (comment.Length < 10)
            throw new ValidationException(nameof(Comment), "Коментар повинен містити щонайменше 10 символів");

        if (comment.Length > 2000)
            throw new ValidationException(nameof(Comment), "Коментар не може перевищувати 2000 символів");

        var ratingValue = Rating.Create(rating);

        return new Review(clientId, roomId, ratingValue, comment);
    }

    public void Update(int rating, string comment)
    {
        if (string.IsNullOrWhiteSpace(comment))
            throw new ValidationException(nameof(Comment), "Коментар не може бути порожнім");

        if (comment.Length < 10)
            throw new ValidationException(nameof(Comment), "Коментар повинен містити щонайменше 10 символів");

        if (comment.Length > 2000)
            throw new ValidationException(nameof(Comment), "Коментар не може перевищувати 2000 символів");

        Rating = Rating.Create(rating);
        Comment = comment;
        UpdateTimestamp();
    }

    public void Verify()
    {
        if (IsVerified)
            throw new ConflictException("Відгук вже верифікований");

        IsVerified = true;
        UpdateTimestamp();
    }

    public void Approve()
    {
        if (IsApproved)
            throw new ConflictException("Відгук вже схвалено");

        IsApproved = true;
        RejectionReason = null;
        UpdateTimestamp();
    }

    public void Reject(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ValidationException(nameof(reason), "Вкажіть причину відхилення");

        if (!IsApproved)
            throw new ConflictException("Відгук вже відхилено");

        IsApproved = false;
        RejectionReason = reason;
        UpdateTimestamp();
    }

    [BsonIgnore]
    public bool IsPositive => Rating.IsHighRating;

    [BsonIgnore]
    public bool IsNegative => Rating.IsLowRating;
}

[AttributeUsage(AttributeTargets.Class)]
public class BsonCollectionAttribute : Attribute
{
    public string CollectionName { get; }

    public BsonCollectionAttribute(string collectionName)
    {
        CollectionName = collectionName;
    }
}