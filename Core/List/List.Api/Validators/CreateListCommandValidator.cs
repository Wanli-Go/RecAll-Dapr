using Ddd.Domain.SeedWork;
using FluentValidation;
using List.Domain.AggregateModels;
using RecAll.Core.List.Api.Commands;

namespace RecAll.Core.List.Api.Validators;

public class CreateListCommandValidator : AbstractValidator<CreateListCommand>
{
    public CreateListCommandValidator(
        ILogger<CreateListCommandValidator> logger)
    {
        RuleFor(p => p.Name).NotEmpty();
        RuleFor(p => p.TypeId).NotEmpty();
        RuleFor(p => p.TypeId).Must(Enumeration.IsValidValue<ListType>)
            .WithMessage("无效的Type ID");
        logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
    }
}