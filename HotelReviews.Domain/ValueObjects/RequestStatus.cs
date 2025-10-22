using HotelReviews.Domain.Exceptions;
using MongoDB.Bson.Serialization.Attributes;

namespace HotelReviews.Domain.ValueObjects;
[BsonSerializer(typeof(RequestStatusSerializer))]
public sealed class RequestStatus : ValueObject
{
    public string Value { get; }

    private RequestStatus(string value)
    {
        Value = value;
    }

    public static readonly RequestStatus Pending = new("pending");
    public static readonly RequestStatus Approved = new("approved");
    public static readonly RequestStatus Rejected = new("rejected");
    public static readonly RequestStatus InProgress = new("in_progress");
    public static readonly RequestStatus Completed = new("completed");

    private static readonly RequestStatus[] AllStatuses =
    {
        Pending, Approved, Rejected, InProgress, Completed
    };

    public static RequestStatus Create(string value)
    {
        var normalizedValue = value?.ToLowerInvariant().Trim();

        if (string.IsNullOrWhiteSpace(normalizedValue))
        {
            throw new ValidationException("Статус запиту не може бути порожнім");
        }

        var status = AllStatuses.FirstOrDefault(s => s.Value == normalizedValue);

        if (status == null)
        {
            throw new ValidationException(
                $"Невідомий статус: '{value}'. Допустимі значення: {string.Join(", ", AllStatuses.Select(s => s.Value))}");
        }

        return status;
    }

    public bool CanTransitionTo(RequestStatus newStatus)
    {
        return (Value, newStatus.Value) switch
        {
            ("pending", "approved") => true,
            ("pending", "rejected") => true,
            ("pending", "in_progress") => true,
            ("in_progress", "completed") => true,
            ("in_progress", "rejected") => true,
            ("approved", "in_progress") => true,
            _ => false
        };
    }

    [BsonIgnore]
    public bool IsFinal => Value is "rejected" or "completed";

    [BsonIgnore]
    public bool IsActive => Value is "pending" or "approved" or "in_progress";

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(RequestStatus status) => status.Value;
}
public class RequestStatusSerializer : MongoDB.Bson.Serialization.Serializers.SerializerBase<RequestStatus>
{
    public override RequestStatus Deserialize(MongoDB.Bson.Serialization.BsonDeserializationContext context, MongoDB.Bson.Serialization.BsonDeserializationArgs args)
    {
        var value = context.Reader.ReadString();
        return RequestStatus.Create(value);
    }

    public override void Serialize(MongoDB.Bson.Serialization.BsonSerializationContext context, MongoDB.Bson.Serialization.BsonSerializationArgs args, RequestStatus value)
    {
        context.Writer.WriteString(value.Value);
    }
}