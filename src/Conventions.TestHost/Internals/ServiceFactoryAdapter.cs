#if NETSTANDARD2_1
using System.Reflection;

namespace Rocket.Surgery.Conventions.Internals
{
    internal class ServiceFactoryAdapter : DispatchProxy
    {
        public static IServiceFactoryAdapterProxy Create(object factory)
        {
            var result = DispatchProxy.Create<IServiceFactoryAdapterProxy, ServiceFactoryAdapter>();
            ( (ServiceFactoryAdapter)( result ) ).SetFactory(factory);
            return result;
        }

        private object _factory;

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            return _factory.GetType().GetMethod(targetMethod.Name).Invoke(_factory, args);
        }

        private void SetFactory(object factory)
        {
            _factory = factory;
        }
    }
}
#endif
