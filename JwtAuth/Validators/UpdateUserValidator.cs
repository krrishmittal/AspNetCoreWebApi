using FluentValidation;
using JwtAuth.DTO;

namespace JwtAuthentication.Validators
{
    public class UpdateUserValidator : AbstractValidator<UpdateUser>
    {
        public UpdateUserValidator()
        {
            // Username is optional, but if provided, must meet requirements
            RuleFor(user => user.Username)
                .MinimumLength(3)
                .WithMessage("Name must be at least 3 characters long.")
                .WithErrorCode("minimum length")
                .MaximumLength(20)
                .WithMessage("Name cannot exceed 20 characters.")
                .WithErrorCode("maximum length")
                .When(user => !string.IsNullOrEmpty(user.Username));

            // Password is optional, but if provided, must meet requirements
            RuleFor(user => user.Password)
                .MinimumLength(6)
                .WithMessage("Password must be at least 6 characters long.")
                .WithErrorCode("minimum length")
                .MaximumLength(255)
                .WithMessage("Password cannot exceed 255 characters.")
                .WithErrorCode("maximum length")
                .When(user => !string.IsNullOrEmpty(user.Password));
        }
    }
}
