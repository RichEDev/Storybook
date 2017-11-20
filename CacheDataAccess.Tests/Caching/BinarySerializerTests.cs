using BusinessLogic.ProjectCodes;

namespace CacheDataAccess.Tests.Caching
{
    using CacheDataAccess.Caching;

    using Xunit;

    public class BinarySerializerTests
    {
        public class Serialize
        {
            [Fact]
            public void SerializableObject_Serialize_Serializes()
            {
                ISerialize serializer = new BinarySerializer();

                IProjectCode projectCode = new ProjectCode(3, "myName", "myDescription", true, true);

                byte[] serialized = serializer.Serialize(projectCode);

                Assert.NotNull(serialized);
                Assert.True(serialized.Length > 0);
            }

            [Fact]
            public void NullObject_Serialize_ReturnsNull()
            {
                ISerialize serializer = new BinarySerializer();

                Assert.Null(serializer.Serialize(null));
            }
        }

        public class Deserialize
        {

            [Fact]
            public void NullComplexType_Deserialize_ReturnsDefault()
            {
                ISerialize serializer = new JsonSerializer();

                Assert.Equal(default(ProjectCode), serializer.Deserialize<ProjectCode>(null));
            }

            [Fact]
            public void NullSimpleType_Deserialize_ReturnsDefault()
            {
                ISerialize serializer = new JsonSerializer();

                Assert.Equal(default(int), serializer.Deserialize<int>(null));
            }

            [Fact]
            public void SerializedProjectCode_Deserialize_ReturnsValidProjectCode()
            {
                ISerialize serializer = new BinarySerializer();

                IProjectCode projectCode = new ProjectCode(3, "myName", "myDescription", true, true);
                byte[] serializedProjectCode = serializer.Serialize(projectCode);

                IProjectCode deserilizedProjectCode = serializer.Deserialize<IProjectCode>(serializedProjectCode);

                Assert.Equal(projectCode.Id, deserilizedProjectCode.Id);
                Assert.Equal(projectCode.Name, deserilizedProjectCode.Name);
                Assert.Equal(projectCode.Description, deserilizedProjectCode.Description);
                Assert.Equal(projectCode.Rechargeable, deserilizedProjectCode.Rechargeable);
                Assert.Equal(projectCode.Archived, deserilizedProjectCode.Archived);
            }
        }
    }
}
