﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;

namespace RavenDBMembership.IntegrationTests
{
    public abstract class AllProvidersSpecification : GivenWhenThenFixture
    {
        public abstract void SpecifyForEachProvider();

        public sealed override void Specify()
        {
            when("using RavenMembershipProvider in-memory", delegate
            {
                importNUnit<ProviderFixtures.FixtureForInMemoryRavenMembershipProvider>();

                SpecifyForEachProvider();
            });

            when("using SQLMembershipProvider", delegate
            {
                importNUnit<ProviderFixtures.FixtureForSqlMembershipProvider>();

                SpecifyForEachProvider();
            });

            when("using raven with munin on disk", delegate
            {
                importNUnit<ProviderFixtures.FixtureForMuninRavenMembershipProvider>();

                SpecifyForEachProvider();
            });

            when("using raven with esent on disk", delegate
            {
                importNUnit<ProviderFixtures.FixtureForEsentRavenMembershipProvider>();

                SpecifyForEachProvider();
            });
        }
    }
}