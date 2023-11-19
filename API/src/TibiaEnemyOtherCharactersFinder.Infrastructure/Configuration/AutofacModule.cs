using Autofac;
using System.Reflection;
using MediatR;
using TibiaEnemyOtherCharactersFinder.Application.Behaviors;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Traceability;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Configuration
{
    public class AutofacModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).AsImplementedInterfaces().PreserveExistingDefaults();
            builder.RegisterAssemblyTypes(Assemblies.ApplicationAssembly).AsImplementedInterfaces().PreserveExistingDefaults();

            // builder.RegisterGeneric(typeof(LoggingPipelineBehavior<,>)).As(typeof(IPipelineBehavior<,>)).InstancePerLifetimeScope();
            // UNDONE: probably to delete /\

            builder.RegisterType<HttpClientDecompressionHandler>().AsSelf().InstancePerDependency();
        }
    }
}