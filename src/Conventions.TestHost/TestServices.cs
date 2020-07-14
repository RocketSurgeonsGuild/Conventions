#if NETSTANDARD2_1
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Rocket.Surgery.Conventions
{
    public class TestServices : IServiceCollection, IDisposable
    {
        private readonly IServiceCollection _serviceCollection;
        private readonly IDisposable _disposable;
        public TestServices(IHost host, IServiceCollection serviceCollection)
        {
            _disposable = host;
            _serviceCollection = serviceCollection;
        }

        public IEnumerator<ServiceDescriptor> GetEnumerator() => _serviceCollection.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ( (IEnumerable)_serviceCollection ).GetEnumerator();

        public void Add(ServiceDescriptor item) => _serviceCollection.Add(item);

        public void Clear() => _serviceCollection.Clear();

        public bool Contains(ServiceDescriptor item) => _serviceCollection.Contains(item);

        public void CopyTo(ServiceDescriptor[] array, int arrayIndex) => _serviceCollection.CopyTo(array, arrayIndex);

        public bool Remove(ServiceDescriptor item) => _serviceCollection.Remove(item);

        public int Count => _serviceCollection.Count;

        public bool IsReadOnly => _serviceCollection.IsReadOnly;

        public int IndexOf(ServiceDescriptor item) => _serviceCollection.IndexOf(item);

        public void Insert(int index, ServiceDescriptor item) => _serviceCollection.Insert(index, item);

        public void RemoveAt(int index) => _serviceCollection.RemoveAt(index);

        public ServiceDescriptor this[int index]
        {
            get => _serviceCollection[index];
            set => _serviceCollection[index] = value;
        }

        public void Dispose() => _disposable.Dispose();
    }
}
#endif