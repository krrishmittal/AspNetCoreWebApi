using FluentValidation;
using ValidationForDataModels.DTO;

namespace ValidationForDataModels.Validators
{
    public class UpdateUserValidator : AbstractValidator<UpdateUser>
    {
        public UpdateUserValidator()
        {
            // Username is optional, but if provided, must meet requirements
            RuleFor(user => user.Username)
                .MinimumLength(3).WithMessage("Name must be at least 3 characters long.")
                .MaximumLength(10).WithMessage("Name cannot exceed 100 characters.")
                .When(user => !string.IsNullOrEmpty(user.Username));

            // Password is optional, but if provided, must meet requirements
            RuleFor(user => user.Password)
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
                .MaximumLength(15).WithMessage("Password cannot exceed 50 characters.")
                .When(user => !string.IsNullOrEmpty(user.Password));
        }
    }
}
