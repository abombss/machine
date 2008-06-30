using System;
using System.Collections.Generic;

using Machine.Container.Model;
using Machine.Container.Plugins;

using NUnit.Framework;

namespace Machine.Container.Services.Impl
{
  [TestFixture]
  public class ThrowsPendingActivatorResolverTests : ScaffoldTests<ThrowsPendingActivatorResolver>
  {
    [Test]
    [ExpectedException(typeof(PendingDependencyException))]
    public void ResolveActivator_Always_Throws()
    {
      ServiceEntry serviceEntry = ServiceEntryHelper.NewEntry();
      ICreationServices creationServices = new CreationServices(Get<IActivatorStrategy>(), Get<IActivatorStore>(), Get<ILifestyleFactory>(), Get<IOverrideLookup>(), Get<IServiceEntryResolver>(), Get<IListenerInvoker>(), Get<IObjectInstances>());
      _target.ResolveActivator(creationServices, serviceEntry);
    }
  }
}