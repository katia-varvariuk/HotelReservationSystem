using FluentValidation;
using MongoDB.Bson;

namespace HotelReviews.Application.Reviews.Commands.UpdateReview;
public class UpdateReviewCommandValidator : AbstractValidator<UpdateReviewCommand>
{
    public UpdateReviewCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("ID відгуку не може бути порожнім")
            .Must(BeValidObjectId)
            .WithMessage("Невірний формат ID відгуку");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5)
            .WithMessage("Рейтинг повинен бути від 1 до 5");

        RuleFor(x => x.Comment)
            .NotEmpty()
            .WithMessage("Коментар не може бути порожнім")
            .MinimumLength(10)
            .WithMessage("Коментар повинен містити щонайменше 10 символів")
            .MaximumLength(2000)
            .WithMessage("Коментар не може перевищувати 2000 символів");
    }

    private bool BeValidObjectId(string id)
    {
        return ObjectId.TryParse(id, out _);
    }
}