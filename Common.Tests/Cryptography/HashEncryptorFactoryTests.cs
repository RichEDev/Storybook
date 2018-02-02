using System;
using Common.Cryptography;
using Xunit;

namespace Common.Tests.Cryptography
{
    public class HashEncryptorFactoryTests
    {
        public class Create
        {
            [Fact]
            public void Create_ReturnsNonNullObject()
            {
                var sut = new HashEncryptorFactory();
                var result = sut.Create();
                Assert.NotNull(result);
            }

        }
    }
}
