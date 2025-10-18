using FluentValidation;
using HotelCatalog.Bll.DTOs;

namespace HotelCatalog.Bll.Validators
{
    public class CreateDiscountCategoryDtoValidator : AbstractValidator<CreateDiscountCategoryDto>
    {
        public CreateDiscountCategoryDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Discount name is required")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

            RuleFor(x => x.DiscountPercent)
                .InclusiveBetween(0, 100).WithMessage("Discount percent must be between 0 and 100")
                .Must(BeAValidPercent).WithMessage("Discount percent must have maximum 2 decimal places");
        }

        private bool BeAValidPercent(decimal percent)
        {
            return decimal.Round(percent, 2) == percent;
        }
    }

    public class UpdateDiscountCategoryDtoValidator : AbstractValidator<UpdateDiscountCategoryDto>
    {
        public UpdateDiscountCategoryDtoValidator()
        {
            RuleFor(x => x.DiscountId)
                .GreaterThan(0).WithMessage("Discount ID must be greater than 0");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Discount name is required")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

            RuleFor(x => x.DiscountPercent)
                .InclusiveBetween(0, 100).WithMessage("Discount percent must be between 0 and 100");
        }
    }
}