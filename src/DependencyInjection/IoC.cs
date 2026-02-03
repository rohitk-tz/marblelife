using Core.Application.Exceptions;
using Microsoft.Practices.Unity;
using System;
using System.Diagnostics;

namespace DependencyInjection
{
    internal static class IoC
    {
        private static readonly UnityContainer UnityContainer = new UnityContainer();

        internal static AbstractType Resolve<AbstractType>()
        {
            return UnityContainer.Resolve<AbstractType>();
        }

        [DebuggerNonUserCode]
        internal static TBase Resolve<TBase>(string name)
        {
            return UnityContainer.Resolve<TBase>(name);
        }

        internal static object Resolve(Type type)
        {
            return UnityContainer.Resolve(type);
        }

        internal static void Register(Type @base, Type concreteType)
        {
            if (!@base.IsAssignableFrom(concreteType) && !@base.IsGenericType)
            {
                throw new InvalidDependencyRegistrationException(@base, @concreteType);
            }

            UnityContainer.RegisterType(@base, concreteType);
        }


        internal static void Register(Type @base, Type concreteType, LifetimeManager lifetimeManager)
        {
            if (!@base.IsAssignableFrom(concreteType) && !@base.IsGenericType)
            {
                throw new InvalidDependencyRegistrationException(@base, @concreteType);
            }

            UnityContainer.RegisterType(@base, concreteType, lifetimeManager);
        }


        internal static void Register<AbstractType, ConcreteType>() where ConcreteType : AbstractType
        {
            UnityContainer.RegisterType<AbstractType, ConcreteType>();
        }

        internal static void Register<TBase>(string name, Type type)
        {
            if (!typeof(TBase).IsAssignableFrom(type))
            {
                throw new InvalidDependencyRegistrationException(typeof(TBase), type);
            }
            UnityContainer.RegisterType(typeof(TBase), type, name);
        }

        internal static void Register<AbstractType, ConcreteType>(LifetimeManager manager) where ConcreteType : AbstractType
        {
            UnityContainer.RegisterType<AbstractType, ConcreteType>(manager);
        }

        internal static void Register<AbstractType, ConcreteType>(string name, params InjectionMember[] injectionMembers) where ConcreteType : AbstractType
        {
            UnityContainer.RegisterType<AbstractType, ConcreteType>(name, injectionMembers);
        }

        internal static void Register<AbstractType, ConcreteType>(params InjectionMember[] injectionMembers) where ConcreteType : AbstractType
        {
            UnityContainer.RegisterType<AbstractType, ConcreteType>(injectionMembers);
        }

        internal static void RegisterInstance<AbstractType>(AbstractType instance)
        {
            UnityContainer.RegisterInstance(instance);
        }

        internal static void RegisterInstance<AbstractType>(AbstractType instance, string name, LifetimeManager manager)
        {
            UnityContainer.RegisterInstance(instance.GetType(), name, instance, manager);
        }

        internal static void RegisterPerThread<AbstractType, ConcreteType>() where ConcreteType : AbstractType
        {
            UnityContainer.RegisterType<AbstractType, ConcreteType>(new PerThreadLifetimeManager());
        }
    }
}
