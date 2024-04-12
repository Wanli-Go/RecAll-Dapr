using Module = Autofac.Module;
using MediatR;
using RecAll.Core.List.Api.Commands;
using System.Reflection;
using Autofac;
using FluentValidation;
using RecAll.Core.List.Api.Behaviors;
using RecAll.Core.List.Api.Validators;

namespace RecAll.Core.List.Api.AutofacModules;

public class MediatorModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {

        builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly).AsImplementedInterfaces();

        builder.RegisterAssemblyTypes(typeof(CreateListCommand).GetTypeInfo().Assembly)
            .AsClosedTypesOf(typeof(IRequestHandler<,>));

        builder.RegisterAssemblyTypes(typeof(CreateListCommandValidator)
                .GetTypeInfo().Assembly)
            .Where(p => p.IsClosedTypeOf(typeof(IValidator<>)))
            .AsImplementedInterfaces();

        // Validate before logging
        builder.RegisterGeneric(typeof(ValidatorBehavior<,>))
            .As(typeof(IPipelineBehavior<,>));

        builder.RegisterGeneric(typeof(LoggingBehavior<,>))
            .As(typeof(IPipelineBehavior<,>));

        builder.RegisterGeneric(typeof(TransactionBehaviour<,>))
            .As(typeof(IPipelineBehavior<,>));

    }
}