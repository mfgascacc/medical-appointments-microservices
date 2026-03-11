using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;

namespace Prescriptions.Api.DependencyInjection
{
    public class SimpleDependencyResolver : IDependencyResolver
    {

        private readonly Dictionary<Type, Func<object>> _registrations = new Dictionary<Type, Func<object>>();
        public void Register<TService>(Func<object> factory)
        {
            _registrations[typeof(TService)] = factory;
        }
        public object GetService(Type serviceType)
        {
            if (_registrations.TryGetValue(serviceType, out var factory))
            {
                return factory();
            }

            if (serviceType.IsInterface || serviceType.IsAbstract)
            {
                return null;
            }

            var constructor = serviceType
                .GetConstructors()
                .OrderByDescending(c => c.GetParameters().Length)
                .FirstOrDefault();

            if (constructor == null)
            {
                return null;
            }

            var parameters = constructor.GetParameters();
            if (parameters.Length == 0)
            {
                return Activator.CreateInstance(serviceType);
            }

            var args = new object[parameters.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
                var dependency = GetService(parameters[i].ParameterType);
                if (dependency == null)
                {
                    return null;
                }

                args[i] = dependency;
            }

            return Activator.CreateInstance(serviceType, args);
        }
        public IEnumerable<object> GetServices(Type serviceType)
        {
            var service = GetService(serviceType);
            return service == null ? Enumerable.Empty<object>() : new[] { service };
        }
        public IDependencyScope BeginScope()
        {
            return this;
        }
        public void Dispose()
        {
        }

    }
}