using FluentValidation;
using HotelCatalog.Bll.DTOs;

namespace HotelCatalog.Bll.Validators
{
    public class CreateRoomCategoryDtoValidator : AbstractValidator<CreateRoomCategoryDto>
    {
        public CreateRoomCategoryDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category name is required")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters")
                .Must(BeAValidCategoryName).WithMessage("Category name must start with a capital letter");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters")
                .When(x => !string.IsNullOrEmpty(x.Description));
        }

        private bool BeAValidCategoryName(string name)
        {
            return !string.IsNullOrEmpty(name) && char.IsUpper(name[0]);
        }
    }

    public class UpdateRoomCategoryDtoValidator : AbstractValidator<UpdateRoomCategoryDto>
    {
        public UpdateRoomCategoryDtoValidator()
        {
            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Category ID must be greater than 0");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category name is required")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");
        }
    }
}