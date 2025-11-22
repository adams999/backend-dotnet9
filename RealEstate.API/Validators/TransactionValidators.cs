using FluentValidation;
using RealEstate.API.DTOs;

namespace RealEstate.API.Validators;

public class CreateTransactionDtoValidator : AbstractValidator<CreateTransactionDto>
{
    public CreateTransactionDtoValidator()
    {
        RuleFor(x => x.PropertyId)
            .GreaterThan(0).WithMessage("PropertyId must be greater than 0");

        RuleFor(x => x.ClientId)
            .GreaterThan(0).WithMessage("ClientId must be greater than 0");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than 0");

        RuleFor(x => x.TransactionType)
            .NotEmpty().WithMessage("TransactionType is required")
            .Must(type => type == "Sale" || type == "Rent" || type == "Lease")
            .WithMessage("TransactionType must be either 'Sale', 'Rent', or 'Lease'");
    }
}
