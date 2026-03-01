using FluentValidation;

namespace Pereprodai.Catalog.Application.Commands.UpdateAd;

public class UpdateAdCommandValidator : AbstractValidator<UpdateAdCommand>
{
    public UpdateAdCommandValidator()
    {
        RuleFor(x => x.AdId).NotEmpty();

        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);

        RuleFor(x => x.Description).NotEmpty().MaximumLength(4000);

        RuleFor(x => x.Category).NotEmpty().MaximumLength(100);

        RuleFor(x => x.PriceAmount).GreaterThanOrEqualTo(0);

        RuleFor(x => x.PriceCurrency).IsInEnum();

        RuleFor(x => x.City).NotEmpty().MaximumLength(100);

        RuleFor(x => x.Phone).NotEmpty().MaximumLength(14);

        RuleFor(x => x.Email).MaximumLength(100).When(x => x.Email is not null);
    }
}
