using System;
using System.Linq;
using System.Text;
using RavenDBMembership.IntegrationTests.ProviderFixtures;

namespace RavenDBMembership.IntegrationTests
{
    public abstract class SpecificationForAllMembershipProviders : MembershipSpecification
    {
        public abstract void SpecifyForEach(bool testingOriginalMembershipProvider);

        public sealed override void Specify()
        {
            when("using RavenMembershipProvider in-memory", delegate
            {
                withCategory("RavenMembershipProvider");

                var provider = new OverrideForInMemoryRavenMembershipProvider();

                arrange_membership_provider(provider);

                SpecifyForEach(false);
            });

            when("using SQLMembershipProvider", delegate
            {
                withCategory("SqlMembershipProvider");

                var provider = new OverrideForSqlMembershipProvider();

                arrange_membership_provider(provider);

                SpecifyForEach(true);
            });

            when("using ASP.NET Universal membership provider", delegate
            {
                withCategory("AspnetUniversalMembershipProvider");

                var provider = new OverrideForUniversalASPNETMembershipProvider();

                arrange_membership_provider(provider);

                SpecifyForEach(false);
            });

            when("using raven with munin on disk", delegate
            {
                withCategory("RavenMembershipProvider");

                var provider = new OverrideForMuninRavenMembershipProvider();

                arrange_membership_provider(provider);

                SpecifyForEach(false);
            });

            when("using raven with esent on disk", delegate
            {
                withCategory("RavenMembershipProvider");

                var provider = new OverrideForEsentRavenMembershipProvider();

                arrange_membership_provider(provider);

                SpecifyForEach(false);
            });
        }
    }
}
