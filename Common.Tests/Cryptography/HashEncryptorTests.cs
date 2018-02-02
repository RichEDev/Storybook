using System;
using Common.Cryptography;
using Xunit;

namespace Common.Tests.Cryptography
{
    public class HashEncryptorTests
    {
        public class Encrypt
        {
            [Fact]
            public void NullValue_Encrypt_ThrowsArgumentNullException()
            {
                var sut = new Pbkdf2Encryptor();
                Assert.Throws<ArgumentNullException>(() => sut.Encrypt(null));
            }

            [Fact]
            public void NonBlankString_Encrypt_ReturnsValue()
            {
                var sut = new Pbkdf2Encryptor();
                var result = sut.Encrypt("HelloWorld");
                Assert.False(string.IsNullOrEmpty(result));
            }
        }

        public class Verify
        {
            [Fact]
            public void KNownHashAndOriginalValue_Verify_ReturnsTrue()
            {
                var sut = new Pbkdf2Encryptor();
                var result = sut.Verify("HelloWorld",
                    "1000:LnaZoaXCyRe96ZitiGKIE8Yo+8xeBr2A:EvoFmvnSQwigpNVlrNoxN0r3h/64g1zM");
                Assert.True(result);
            }

            [Fact]
            public void ParametersNull_Verify_ThrowsNullReferenceException()
            {
                var sut = new Pbkdf2Encryptor();
                Assert.Throws<NullReferenceException>(() => sut.Verify(null, null));
            }

            [Fact]
            public void HashInvalid_Verify_ReturnsFalse()
            {
                var sut = new Pbkdf2Encryptor();
                Assert.Throws<HashException>(() =>  sut.Verify(null, "testing"));
            }
        }
    }
}
