using System.Collections.Generic;
using NJasmine;
using RavenDBMembership.IntegrationTests.ProviderFixtures;

namespace RavenDBMembership.IntegrationTests
{
    public abstract class MembershipSpecification : GivenWhenThenFixture
    {
        public virtual Dictionary<string, string> GetAdditionalConfiguration()
        {
            return new Dictionary<string, string>();
        }

        public void arrange_membership_provider(MembershipProviderOverride membership)
        {
            beforeAll(delegate
                {
                    membership.InjectMembershipImplementation(GetAdditionalConfiguration());
                });

            afterAll(() => membership.RestoreMembershipImplementation());
        }
    }
}