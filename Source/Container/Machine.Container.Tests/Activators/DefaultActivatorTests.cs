using System;
using System.Collections.Generic;

using Machine.Container.Activators;
using Machine.Container.Model;
using Machine.Container.Services;
using Machine.Container.Services.Impl;

using NUnit.Framework;

using Rhino.Mocks;

namespace Machine.Container.Activators
{
  [TestFixture]
  public class DefaultActivatorTests : ScaffoldTests<DefaultActivator>
  {
    #region Member Data
    ServiceEntry _entry;
    object _instance;
    object _parameter1;
    #endregion

    #region Test Setup and Teardown Methods
    public override void Setup()
    {
      _entry = ServiceEntryHelper.NewEntry();
      _instance = new object();
      _parameter1 = new object();
      base.Setup();
    }
    #endregion

    #region Test Methods
    [Test]
    [ExpectedException(typeof(YouFoundABugException))]
    public void Create_NotResolved_Throws()
    {
      using (_mocks.Record())
      {
      }
      using (_mocks.Playback())
      {
        _target.Activate(Get<ICreationServices>());
      }
    }

    [Test]
    public void Create_NoDependencies_CallsConstructor()
    {
      using (_mocks.Record())
      {
        SetupMocks();
        Expect.Call(Get<IObjectFactory>().CreateObject(_entry.ConstructorCandidate.Candidate, new object[0])).Return(_instance);
      }
      using (_mocks.Playback())
      {
        _target.CanActivate(Get<ICreationServices>());
        Assert.AreEqual(_instance, _target.Activate(Get<ICreationServices>()));
      }
    }

    [Test]
    public void Create_OneDependency_CallsConstructor()
    {
      ResolvedServiceEntry resolvedServiceEntry = new ResolvedServiceEntry(ServiceEntryHelper.NewEntry(), Get<IActivator>(), Get<IObjectInstances>());
      using (_mocks.Record())
      {
        SetupMocks(typeof(IService1));
        Expect.Call(Get<IServiceEntryResolver>().ResolveEntry(Get<ICreationServices>(), typeof(IService1), false)).
          Return(resolvedServiceEntry);
        Expect.Call(Get<IActivator>().Activate(Get<ICreationServices>())).Return(_parameter1);
        Expect.Call(Get<IObjectFactory>().CreateObject(_entry.ConstructorCandidate.Candidate, new object[] {_parameter1})).Return(
          _instance);
      }
      using (_mocks.Playback())
      {
        _target.CanActivate(Get<ICreationServices>());
        Assert.AreEqual(_instance, _target.Activate(Get<ICreationServices>()));
      }
    }
    #endregion

    #region Methods
    protected virtual void SetupMocks(params Type[] dependencies)
    {
      _entry.ConstructorCandidate = new ResolvedConstructorCandidate(CreateCandidate(typeof(Service1DependsOn2), dependencies), new List<ResolvedServiceEntry>());
      SetupResult.For(Get<ICreationServices>().DependencyGraphTracker).Return(new DependencyGraphTracker());
      SetupResult.For(Get<ICreationServices>().ActivatorStore).Return(Get<IActivatorStore>());
      SetupResult.For(Get<IServiceDependencyInspector>().SelectConstructor(typeof(Service1DependsOn2))).Return(
        _entry.ConstructorCandidate.Candidate);
    }

    protected override DefaultActivator Create()
    {
      return new DefaultActivator(Get<IObjectFactory>(), Get<IServiceDependencyInspector>(),
        Get<IServiceEntryResolver>(), _entry);
    }
    #endregion
  }
}