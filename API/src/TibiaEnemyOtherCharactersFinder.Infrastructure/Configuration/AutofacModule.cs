﻿using Autofac;
using System.Reflection;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Traceability;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Configuration
{
    public class AutofacModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).AsImplementedInterfaces().PreserveExistingDefaults();
            builder.RegisterAssemblyTypes(Assemblies.ApplicationAssembly).AsImplementedInterfaces().PreserveExistingDefaults();

            builder.RegisterType<HttpClientDecompressionHandler>().AsSelf().InstancePerDependency();
        }
    }
}