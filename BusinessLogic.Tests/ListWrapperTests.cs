namespace BusinessLogic.Tests
{
    using System.Collections.Generic;

    using Xunit;

    public class ListWrapperTests
    {
        [Fact]
        public void List_Ctor_InitializesEmptyCollection()
        {
            ListWrapper<int> sut = new ListWrapperStub();

            Assert.Equal(0, sut.Count);
        }

        [Fact]
        public void List_Ctor_MaintainsOrder()
        {
            ListWrapper<int> sut = this.Create();

            this.AssertCreate(sut);
        }

        [Fact]
        public void List_Count_ReturnsCorrectNumber()
        {
            ListWrapper<int> sut = this.Create();

            Assert.Equal(5, sut.Count);
        }

        [Fact]
        public void List_Add_ValueAdded()
        {
            ListWrapper<int> sut = this.Create();

            sut.Add(6);

            Assert.Equal(6, sut.Count);
            Assert.Equal(6, sut[5]);
        }

        [Fact]
        public void List_Contains_ReturnsCorrectly()
        {
            ListWrapper<int> sut = this.Create();

            Assert.True(sut.Contains(1));
            Assert.True(sut.Contains(2));
            Assert.True(sut.Contains(3));
            Assert.True(sut.Contains(4));
            Assert.True(sut.Contains(5));
            Assert.False(sut.Contains(6));
        }

        [Fact]
        public void List_ContainsType_ReturnsCorrectly()
        {
            ListWrapper<int> sut = this.Create();

            Assert.True(sut.Contains(typeof(int)));
            Assert.False(sut.Contains(typeof(string)));
        }

        [Fact]
        public void PopulatedList_Enumerator_ReturnsCorrectSequence()
        {
            ListWrapper<int> sut = this.Create();

            Assert.Equal(new List<int> { 1, 2, 3, 5, 4 }, sut);
        }

        private ListWrapper<int> Create()
        {
            return new ListWrapperStub(new[] { 1, 2, 3, 5, 4 });
        }

        private void AssertCreate(ListWrapper<int> sut)
        {
            Assert.Equal(1, sut[0]);
            Assert.Equal(2, sut[1]);
            Assert.Equal(3, sut[2]);
            Assert.Equal(5, sut[3]);
            Assert.Equal(4, sut[4]);
        }


        public class ListWrapperStub : ListWrapper<int>
        {
            public ListWrapperStub()
            {
                
            }

            public ListWrapperStub(IEnumerable<int> collection)
                : base(collection)
            {
                
            }
        }
    }
}
