using HotelReviews.Domain.Exceptions;
using MongoDB.Bson.Serialization.Attributes;

namespace HotelReviews.Domain.ValueObjects;

[BsonSerializer(typeof(RatingSerializer))]
public sealed class Rating : ValueObject
{
    public int Value { get; }

    private Rating(int value)
    {
        Value = value;
    }

    public static Rating Create(int value)
    {
        if (value < 1 || value > 5)
        {
            throw new ValidationException($"Рейтинг повинен бути від 1 до 5. Отримано: {value}");
        }

        return new Rating(value);
    }

    public string ToStars() => new string('⭐', Value);

    [BsonIgnore]
    public bool IsHighRating => Value >= 4;

    [BsonIgnore]
    public bool IsLowRating => Value <= 2;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => $"{Value}/5";

    public static implicit operator int(Rating rating) => rating.Value;
}

public class RatingSerializer : MongoDB.Bson.Serialization.Serializers.SerializerBase<Rating>
{
    public override Rating Deserialize(MongoDB.Bson.Serialization.BsonDeserializationContext context, MongoDB.Bson.Serialization.BsonDeserializationArgs args)
    {
        var value = context.Reader.ReadInt32();
        return Rating.Create(value);
    }

    public override void Serialize(MongoDB.Bson.Serialization.BsonSerializationContext context, MongoDB.Bson.Serialization.BsonSerializationArgs args, Rating value)
    {
        context.Writer.WriteInt32(value.Value);
    }
}