namespace BusinessLogic.Tests.Identity
{
    using System;

    using BusinessLogic.Identity;

    using NSubstitute;
    using NSubstitute.ReturnsExtensions;

    using Xunit;

    public class IdentityProviderTests
    {
        public class Ctor
        {
            [Fact]
            public void InvalidUserIdentityContext_Ctor_ThrowsNullArgument()
            {
                Assert.Throws<ArgumentNullException>(() => new IdentityProvider(null));
            }
        }

        public class GetUserIdentity
        {
            [Fact]
            public void ValidCall_GetUserIdentity_CallsIdentityContextGet()
            {
                IIdentityContextProvider contextProvider = Substitute.For<IIdentityContextProvider>();
                contextProvider.Get().Returns(new UserIdentity(262, 22));

                IdentityProvider sut = new IdentityProvider(contextProvider);
                UserIdentity userIdentity = sut.GetUserIdentity();

                Assert.Equal(262, userIdentity.AccountId);
                Assert.Equal(22, userIdentity.EmployeeId);
                Assert.True(userIdentity.IsAuthenticated);
                contextProvider.Received(1).Get();
            }

            [Fact]
            public void NullResponseFromContext_GetUserIdentity_IsAuthenticatedIsFalse()
            {
                IIdentityContextProvider contextProvider = Substitute.For<IIdentityContextProvider>();
                contextProvider.Get().ReturnsNull();

                IdentityProvider sut = new IdentityProvider(contextProvider);
                UserIdentity userIdentity = sut.GetUserIdentity();

                Assert.Null(userIdentity);
            }

            [Fact]
            public void NonPositiveAccountIdValueFromContext_GetUserIdentity_IsAuthenticatedIsFalse()
            {
                IIdentityContextProvider contextProvider = Substitute.For<IIdentityContextProvider>();
                contextProvider.Get().Returns(new UserIdentity(0, 22));

                IdentityProvider sut = new IdentityProvider(contextProvider);
                UserIdentity userIdentity = sut.GetUserIdentity();

                Assert.Equal(0, userIdentity.AccountId);
                Assert.Equal(22, userIdentity.EmployeeId);
                Assert.False(userIdentity.IsAuthenticated);
                contextProvider.Received(1).Get();
            }

            [Fact]
            public void NonPositiveEmployeeIdValueFromContext_GetUserIdentity_IsAuthenticatedIsFalse()
            {
                IIdentityContextProvider contextProvider = Substitute.For<IIdentityContextProvider>();
                contextProvider.Get().Returns(new UserIdentity(252, 0));

                IdentityProvider sut = new IdentityProvider(contextProvider);
                UserIdentity userIdentity = sut.GetUserIdentity();

                Assert.Equal(252, userIdentity.AccountId);
                Assert.Equal(0, userIdentity.EmployeeId);
                Assert.False(userIdentity.IsAuthenticated);
                contextProvider.Received(1).Get();
            }

            public class IdentityContextProviderStub : IIdentityContextProvider
            {
                private readonly int _accountId;

                private readonly int _employeeId;

                public IdentityContextProviderStub(int accountId, int employeeId)
                {
                    this._accountId = accountId;
                    this._employeeId = employeeId;
                }

                public UserIdentity Get()
                {
                    return new UserIdentity(this._accountId, this._employeeId);
                }
            }
        }
    }
}
