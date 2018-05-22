using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests
{
    using Xunit;

    public class GuardTests
    {
        public class ThrowIfNullOrWhiteSpace
        {
            [Theory]
            [InlineData(null)]
            [InlineData("")]
            [InlineData(" ")]
            public void InvalidArgument_ThrowIfNullOrWhiteSpace_ThrowsArgumentNull(string value)
            {
                Assert.Throws<ArgumentNullException>(() => Guard.ThrowIfNullOrWhiteSpace(value, "someArgument"));
            }
        }

        public class ThrowIfNull
        {
            [Fact]
            public void NullArgument_ThrowIfNull_ThrowsArgumentNull()
            {
                Assert.Throws<ArgumentNullException>(() => Guard.ThrowIfNull(null, "someArgument"));
            }
        }

    }
}
