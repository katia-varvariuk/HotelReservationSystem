using FluentValidation;
using HotelCatalog.Bll.DTOs;

namespace HotelCatalog.Bll.Validators
{
    public class CreateServiceDtoValidator : AbstractValidator<CreateServiceDto>
    {
        public CreateServiceDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Service name is required")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters")
                .Matches("^[a-zA-Z0-9 ]+$").WithMessage("Name can only contain letters, numbers and spaces");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price must be greater than or equal to 0")
                .LessThanOrEqualTo(100000).WithMessage("Price cannot exceed 100,000");
        }
    }

    public class UpdateServiceDtoValidator : AbstractValidator<UpdateServiceDto>
    {
        public UpdateServiceDtoValidator()
        {
            RuleFor(x => x.ServiceId)
                .GreaterThan(0).WithMessage("Service ID must be greater than 0");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Service name is required")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price must be greater than or equal to 0");
        }
    }
}