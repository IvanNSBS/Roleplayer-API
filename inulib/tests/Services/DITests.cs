// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using System;
using NUnit.Framework;
using Microsoft.MinIoC;

namespace Tests.Runtime.Services.IoCTests
{
    #pragma warning disable 1591
    public class ContainerTests
    {
        [Test]
        public void SimpleReflectionConstruction()
        {
            var container = new DiContainer();
            container.Register<IFoo>(typeof(Foo));

            object instance = container.Resolve<IFoo>();

            // Instance should be of the registered type 
            Assert.That(instance, Is.InstanceOf(typeof(Foo)));
        }

        [Test]
        public void RecursiveReflectionConstruction()
        {
            var container = new DiContainer();

            container.Register<IFoo>(typeof(Foo));
            container.Register<IBar>(typeof(Bar));
            container.Register<IBaz>(typeof(Baz));

            IBaz instance = container.Resolve<IBaz>();

            // Test that the correct types were created
            Assert.That(instance, Is.InstanceOf(typeof(Baz)));

            var baz = instance as Baz;
            Assert.Multiple(() =>
            {
                Assert.That(baz.Bar, Is.InstanceOf(typeof(Bar)));
                Assert.That(baz.Foo, Is.InstanceOf(typeof(Foo)));
            });
        }

        [Test]
        public void SimpleFactoryConstruction()
        {
            var container = new DiContainer();
            container.Register<IFoo>(() => new Foo());

            object instance = container.Resolve<IFoo>();

            // Instance should be of the registered type 
            Assert.That(instance, Is.InstanceOf(typeof(Foo)));
        }

        [Test]
        public void MixedConstruction()
        {
            var container = new DiContainer();
            container.Register<IFoo>(() => new Foo());
            container.Register<IBar>(typeof(Bar));
            container.Register<IBaz>(typeof(Baz));

            IBaz instance = container.Resolve<IBaz>();

            // Test that the correct types were created
            Assert.That(instance, Is.InstanceOf(typeof(Baz)));

            var baz = instance as Baz;
            Assert.Multiple(() =>
            {
                Assert.That(baz.Bar, Is.InstanceOf(typeof(Bar)));
                Assert.That(baz.Foo, Is.InstanceOf(typeof(Foo)));
            });
        }

        [Test]
        public void InstanceResolution()
        {
            var container = new DiContainer();
            container.Register<IFoo>(typeof(Foo));

            object instance1 = container.Resolve<IFoo>();
            object instance2 = container.Resolve<IFoo>();

            // Instances should be different between calls to Resolve
            Assert.That(instance2, Is.Not.EqualTo(instance1));
        }

        [Test]
        public void SingletonResolution()
        {
            var container = new DiContainer();
            container.Register<IFoo>(typeof(Foo)).AsSingleton();

            object instance1 = container.Resolve<IFoo>();
            object instance2 = container.Resolve<IFoo>();

            // Instances should be identic between calls to Resolve
            Assert.That(instance2, Is.EqualTo(instance1));
        }

        [Test]
        public void PerScopeResolution()
        {
            var container = new DiContainer();
            container.Register<IFoo>(typeof(Foo)).PerScope();

            object instance1 = container.Resolve<IFoo>();
            object instance2 = container.Resolve<IFoo>();

            // Instances should be same as the container is itself a scope
            Assert.That(instance2, Is.EqualTo(instance1));

            using (var scope = container.CreateScope())
            {
                object instance3 = scope.Resolve<IFoo>();
                object instance4 = scope.Resolve<IFoo>();

                // Instances should be equal inside a scope
                Assert.That(instance4, Is.EqualTo(instance3));

                // Instances should not be equal between scopes
                Assert.That(instance3, Is.Not.EqualTo(instance1));
            }
        }

        [Test]
        public void MixedScopeResolution()
        {
            var container = new DiContainer();
            
            container.Register<IFoo>(typeof(Foo)).PerScope();
            container.Register<IBar>(typeof(Bar)).AsSingleton();
            container.Register<IBaz>(typeof(Baz));

            using (var scope = container.CreateScope())
            {
                Baz instance1 = scope.Resolve<IBaz>() as Baz;
                Baz instance2 = scope.Resolve<IBaz>() as Baz;

                // Ensure resolutions worked as expected
                Assert.That(instance2, Is.Not.EqualTo(instance1));

                Assert.Multiple(() =>
                {
                    // Singleton should be same
                    Assert.That(instance2.Bar, Is.EqualTo(instance1.Bar));
                    Assert.That((instance2.Bar as Bar).Foo, Is.EqualTo((instance1.Bar as Bar).Foo));
                    // Scoped types should be the same
                    Assert.That(instance2.Foo, Is.EqualTo(instance1.Foo));
                });

                Assert.Multiple(() =>
                {
                    // Singleton should not hold scoped object
                    Assert.That((instance1.Bar as Bar).Foo, Is.Not.EqualTo(instance1.Foo));
                    Assert.That((instance2.Bar as Bar).Foo, Is.Not.EqualTo(instance2.Foo));
                });
            }
        }

        [Test]
        public void SingletonScopedResolution()
        {
            var container = new DiContainer();
            
            container.Register<IFoo>(typeof(Foo)).AsSingleton();
            container.Register<IBar>(typeof(Bar)).PerScope();

            var instance1 = container.Resolve<IBar>();

            using (var scope = container.CreateScope())
            {
                var instance2 = container.Resolve<IBar>();

                // Singleton should resolve to the same instance
                Assert.That((instance2 as Bar).Foo, Is.EqualTo((instance1 as Bar).Foo));
            }
        }

        [Test]
        public void MixedNoScopeResolution()
        {
            var container = new DiContainer();

            container.Register<IFoo>(typeof(Foo)).PerScope();
            container.Register<IBar>(typeof(Bar)).AsSingleton();
            container.Register<IBaz>(typeof(Baz));

            Baz instance1 = container.Resolve<IBaz>() as Baz;
            Baz instance2 = container.Resolve<IBaz>() as Baz;

            // Ensure resolutions worked as expected
            Assert.That(instance2, Is.Not.EqualTo(instance1));

            // Singleton should be same
            Assert.That(instance2.Bar, Is.EqualTo(instance1.Bar));

            Assert.Multiple(() =>
            {
                // Scoped types should not be different outside a scope
                Assert.That(instance2.Foo, Is.EqualTo(instance1.Foo));
                Assert.That((instance1.Bar as Bar).Foo, Is.EqualTo(instance1.Foo));
                Assert.That((instance2.Bar as Bar).Foo, Is.EqualTo(instance2.Foo));
            });
        }

        [Test]
        public void MixedReversedRegistration()
        {
            var container = new DiContainer();
            
            container.Register<IBaz>(typeof(Baz));
            container.Register<IBar>(typeof(Bar));
            container.Register<IFoo>(() => new Foo());

            IBaz instance = container.Resolve<IBaz>();

            // Test that the correct types were created
            Assert.That(instance, Is.InstanceOf(typeof(Baz)));

            var baz = instance as Baz;
            Assert.That(baz.Bar, Is.InstanceOf(typeof(Bar)));
            Assert.That(baz.Foo, Is.InstanceOf(typeof(Foo)));
        }

        [Test]
        public void ScopeDisposesOfCachedInstances()
        {
            var container = new DiContainer();
            
            container.Register<SpyDisposable>(typeof(SpyDisposable)).PerScope();
            SpyDisposable spy;

            using (var scope = container.CreateScope())
            {
                spy = scope.Resolve<SpyDisposable>();
            }

            Assert.That(spy.Disposed, Is.True);
        }

        [Test]
        public void ContainerDisposesOfSingletons()
        {
            SpyDisposable spy;
            using (var container = new DiContainer())
            {
                container.Register<SpyDisposable>().AsSingleton();
                spy = container.Resolve<SpyDisposable>();
            }

            Assert.That(spy.Disposed, Is.True);
        }

        [Test]
        public void SingletonsAreDifferentAcrossContainers()
        {
            var container1 = new DiContainer();
            container1.Register<IFoo>(typeof(Foo)).AsSingleton();

            var container2 = new DiContainer();
            container2.Register<IFoo>(typeof(Foo)).AsSingleton();

            Assert.That(container2.Resolve<IFoo>(), Is.Not.EqualTo(container1.Resolve<IFoo>()));
        }

        [Test]
        public void GetServiceUnregisteredTypeReturnsNull()
        {
            using (var container = new DiContainer())
            {
                object value = container.GetService(typeof(Foo));

                Assert.That(value, Is.Null);
            }
        }

        [Test]
        public void GetServiceMissingDependencyThrows()
        {
            using (var container = new DiContainer())
            {
                container.Register<Bar>();

                Assert.Catch<KeyNotFoundException>(() => container.GetService(typeof(Bar)));
            }
        }

        #region Types used for tests
        interface IFoo
        {
        }

        class Foo : IFoo
        {
        }

        interface IBar
        {
        }

        class Bar : IBar
        {
            public IFoo Foo { get; set; }

            public Bar(IFoo foo)
            {
                Foo = foo;
            }
        }

        interface IBaz
        {
        }

        class Baz : IBaz
        {
            public IFoo Foo { get; set; }
            public IBar Bar { get; set; }

            public Baz(IFoo foo, IBar bar)
            {
                Foo = foo;
                Bar = bar;
            }
        }

        class SpyDisposable : IDisposable
        {
            public bool Disposed { get; private set; }

            public void Dispose() => Disposed = true;
        }
        #endregion
    }
    #pragma warning restore 1591
}
