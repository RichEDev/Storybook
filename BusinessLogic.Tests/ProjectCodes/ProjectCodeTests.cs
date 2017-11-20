namespace BusinessLogic.Tests.ProjectCodes
{
    using BusinessLogic.ProjectCodes;
    using Xunit;

    public class ProjectCodeTests
    {
        [Fact]
        public void InitializingDefaultConstructor_CreatesDefaultProjectCode()
        {
            ProjectCode sut = new ProjectCode();
            Assert.Equal(0, sut.Id);
            Assert.Null(sut.Name);
            Assert.Null(sut.Description);
            Assert.Equal(false, sut.Archived);
            Assert.Equal(false, sut.Rechargeable);
        }


        [Fact]
        public void Initializing_ProjectCode_SetsProperties()
        {
            ProjectCode sut = new ProjectCode(11, "ref", "desc", true, false);
            Assert.Equal(11, sut.Id);
            Assert.Equal("ref", sut.Name);
            Assert.Equal("desc", sut.Description);
            Assert.Equal(true, sut.Archived);
            Assert.Equal(false, sut.Rechargeable);

            // switch the bools just to make sure
            sut = new ProjectCode(11, "ref", "desc", false, true);
            Assert.Equal(false, sut.Archived);
            Assert.Equal(true, sut.Rechargeable);

            Assert.Equal(11, sut.Id);
            Assert.Equal("desc", sut.ToString(true));
            Assert.Equal("ref", sut.ToString(false));
        }

        [Fact]
        public void Set_ProjectCodeProperties_SetsProperties()
        {
            ProjectCode sut = new ProjectCode(11, "ref", "desc", true, false)
            {
                Id = 12,
                Name = "new ref",
                Description = "new desc",
                Archived = false
            };
            
            // Check that the properties setter does nothing fancy
            Assert.Equal(12, sut.Id);
            Assert.Equal("new ref", sut.Name);
            Assert.Equal("new desc", sut.Description);
            Assert.Equal(false, sut.Archived);
        }
    }
}
