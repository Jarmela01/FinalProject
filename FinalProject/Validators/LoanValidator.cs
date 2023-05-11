using FluentValidation;
using System.Collections.Generic;
using System;
using Users.Domain;

namespace LOAN_API.Validators
{
    public class LoanValidator : AbstractValidator<Loan>
    {
        public LoanValidator() 
        {
            var loanTypes = new List<string>() {"auto loan","fast Loan","installment"};
            RuleFor(l => l.LoanType).NotEmpty().WithMessage("Loan Type Is Required!")
              .Must(l => loanTypes.Contains(l))
              .WithMessage("We Have Only These Loans: " + String.Join(",", loanTypes));

            RuleFor(m => m.Money).NotEmpty().WithMessage("Money Is Required!")
                .GreaterThan(0).WithMessage("Money Should Be More Than 0!")
                .LessThan(100000).WithMessage("Money Should Be Less Than 100K!");

            var currencies = new List<string>() { "USD", "EUR", "GEL" };
            RuleFor(c => c.Currency).NotEmpty().WithMessage("Currency Is Required!")
                .Must(c => currencies.Contains(c))
                .WithMessage("We Have Only These Currencies: " + String.Join(",", currencies));

            RuleFor(p => p.Period).NotEmpty().WithMessage("Period Is Required!")
                .GreaterThan(0).WithMessage("Period Should Be More Than 0!")
                .LessThan(120).WithMessage("Our Maximum Loan Is 120 Months!");

            
        }
    }
}
