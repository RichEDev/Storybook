namespace BusinessLogic.Tests.DataConnections
{
    using Identity;

    using System;
    using System.Collections.Generic;
    using BusinessLogic.DataConnections;

    using Xunit;

    public class DataParametersTests
    {
        [Fact]
        public void DefaultCtor_Initialized_SetsParameters()
        {
            DataParameterStub sut = new DataParameterStub();

            Assert.Equal(0, sut.Count);
            Assert.Null(sut.ReturnValue);
        }

        [Fact]
        public void ItemsPresent_Clear_BackingCollectionCleared()
        {
            DataParameterStub sut = new DataParameterStub {"test"};

            // Asserting just to make sure the stub works
            Assert.Equal(1, sut.Count);

            sut.Clear();


            Assert.Equal(0, sut.Count);
        }

        [Fact]
        public void MultipleEntries_GetEnumerator_IteratesEntries()
        {
            DataParameterStub sut = new DataParameterStub {"test1", "test2", "test3"};

            string output = string.Empty;

            foreach (string s in sut)
            {
                output += $"{s},";
            }
            
            Assert.Equal("test1,test2,test3,", output);
        }

        public class DataParameterStub : DataParameters<string>
        {
            public override void Add(string value)
            {
                this.ParametersCollection.Add(value);
            }

            public override void Add(IEnumerable<string> values)
            {
                throw new InvalidOperationException();
            }

            public override string this[string key] => throw new InvalidOperationException();

            public override void AddAuditing(UserIdentity currentUser)
            {
                throw new InvalidOperationException();
            }

            public int Count => this.ParametersCollection.Count;
        }
    }
}
