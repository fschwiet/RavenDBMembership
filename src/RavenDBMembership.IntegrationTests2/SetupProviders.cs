using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Windsor;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;

namespace RavenDBMembership.IntegrationTests2
{
    [SetUpFixture]
    public class SetupProviders
    {
        [SetUp]
        public void PreprareProvider()
        {
            var store = new EmbeddableDocumentStore() {RunInMemory = true};
            store.Initialize();

            var container = new WindsorContainer();
            container.Register(Castle.MicroKernel.Registration.Component.For<IDocumentStore>().Instance(store));

            ServiceLocator.SetLocatorProvider(() => new WindsorServiceLocator(container));
        }
    }
}
