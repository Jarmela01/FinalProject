using FluentValidation;
using Users.Domain;

namespace LOAN_API.Services
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        { 
            RuleFor(f=>f.FirstName).NotEmpty().WithMessage("Please Enter Firstname!")
                .Length(0,30).WithMessage("Firstname Should Be Between 0-30 Symbols!");

            RuleFor(l => l.LastName).NotEmpty().WithMessage("Please Enter Lastname!")
                .Length(0, 30).WithMessage("Lastname Should Be Between 0-30 Symbols!");

            RuleFor(u=>u.UserName).NotEmpty().WithMessage("Please Enter Username!")
                .Length(0, 15).WithMessage("Username Should Be Between 0-15 Symbols!");

            RuleFor(a => a.Age.ToString()).NotEmpty().WithMessage("Please Enter Age!");

            RuleFor(a => a.Age).GreaterThanOrEqualTo(18).WithMessage("You Should Be Adult!")
                .LessThan(100).WithMessage("You Should Be Under 100!");

            RuleFor(e => e.Email).NotEmpty().WithMessage("Please Enter Email!")
                .EmailAddress().WithMessage("Please Enter Valid Email!");

            RuleFor(s => s.Salary).NotEmpty().WithMessage("Please Enter Salary!");

            RuleFor(p => p.Password).NotEmpty().WithMessage("Your password cannot be empty")
                    .MinimumLength(8).WithMessage("Your password length must be at least 8.")
                    .MaximumLength(16).WithMessage("Your password length must not exceed 16.")
                    .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
                    .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
                    .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.");
        }

    }
}
