using FluentValidation;

namespace HotelReviews.Application.Requests.Commands.CreateRequest;
public class CreateRequestCommandValidator : AbstractValidator<CreateRequestCommand>
{
    public CreateRequestCommandValidator()
    {
        RuleFor(x => x.ClientId)
            .GreaterThan(0)
            .WithMessage("ID клієнта повинен бути додатнім числом");

        RuleFor(x => x.RoomId)
            .GreaterThan(0)
            .WithMessage("ID кімнати повинен бути додатнім числом");

        RuleFor(x => x.RequestText)
            .NotEmpty()
            .WithMessage("Текст запиту не може бути порожнім")
            .MinimumLength(10)
            .WithMessage("Запит повинен містити щонайменше 10 символів")
            .MaximumLength(5000)
            .WithMessage("Запит не може перевищувати 5000 символів");

        RuleFor(x => x.Category)
            .NotEmpty()
            .WithMessage("Категорія не може бути порожньою")
            .MaximumLength(50)
            .WithMessage("Категорія не може перевищувати 50 символів");

        RuleFor(x => x.Priority)
            .InclusiveBetween(1, 5)
            .WithMessage("Пріоритет повинен бути від 1 до 5");
    }
}