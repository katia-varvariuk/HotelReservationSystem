namespace HotelReviews.Domain.Exceptions;
public class NotFoundException : DomainException
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string entityName, object key)
        : base($"Сутність '{entityName}' з ключем '{key}' не знайдена")
    {
    }

    public NotFoundException(string entityName, string propertyName, object propertyValue)
        : base($"Сутність '{entityName}' з {propertyName} = '{propertyValue}' не знайдена")
    {
    }
}