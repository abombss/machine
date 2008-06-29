using System;
using System.Collections.Generic;

using Machine.Container.Model;

namespace Machine.Container.Services
{
  public interface IHighLevelContainer : IDisposable
  {
    void AddService(Type serviceType, LifestyleType lifestyleType);
    void AddService<TService>();
    void AddService<TService>(Type implementation);
    void AddService<TService, TImpl>(LifestyleType lifestyleType);
    void AddService<TService, TImpl>();
    void AddService<TService>(LifestyleType lifestyleType);
    void AddService<TService>(object instance);
    T Resolve<T>();
    object Resolve(Type type);
    T ResolveWithOverrides<T>(params object[] serviceOverrides);
    T New<T>(params object[] serviceOverrides);
    void Release(object instance);
    bool HasService<T>();
    IEnumerable<ServiceRegistration> RegisteredServices { get; }
    void Initialize();
  }
}