using System.Collections.Generic;
using GeoJSON.Net.Geometry;
using ServiceStack.Text;
using NUnit.Framework;

namespace GeoJSON.Net.Tests.Geometry
{
    [TestFixture]
    public class LineStringTests : TestBase
    {
        [Test]
        public void Is_Closed()
        {
            var coordinates = new List<GeoEntity>
            {
                new GeoEntity(4.889259338378906, 52.370725881211314),
                new GeoEntity(4.895267486572266, 52.3711451105601),
                new GeoEntity(4.892091751098633, 52.36931095278263),
                new GeoEntity(4.889259338378906, 52.370725881211314)
            };

            var lineString = new GeoLineString(coordinates);

            Assert.IsTrue(lineString.IsClosed());
        }

        [Test]
        public void Is_Not_Closed()
        {
            var coordinates = new List<GeoEntity>
            {
                new GeoEntity(4.889259338378906, 52.370725881211314),
                new GeoEntity(4.895267486572266, 52.3711451105601),
                new GeoEntity(4.892091751098633, 52.36931095278263),
                new GeoEntity(4.889259338378955, 52.370725881211592)
            };

            var lineString = new GeoLineString(coordinates);

            Assert.IsFalse(lineString.IsClosed());
        }

        [Test]
        public void Can_Serialize()
        {
            var coordinates = new List<GeoEntity>
            {
                new GeoEntity(4.889259338378906, 52.370725881211314),
                new GeoEntity(4.895267486572266, 52.3711451105601),
                new GeoEntity(4.892091751098633, 52.36931095278263),
                new GeoEntity(4.889259338378906, 52.370725881211314)
            };

            var lineString = new GeoLineString(coordinates);

            var actualJson = JsonSerializer.SerializeToString(lineString);

            JsonAssert.AreEqual(GetExpectedJson(), actualJson);
        }

        [Test]
        public void Can_Deserialize()
        {
            var coordinates = new List<GeoEntity>
            {
                new GeoEntity(4.889259338378906, 52.370725881211314),
                new GeoEntity(4.895267486572266, 52.3711451105601),
                new GeoEntity(4.892091751098633, 52.36931095278263),
                new GeoEntity(4.889259338378906, 52.370725881211314)
            };

            var expectedLineString = new GeoLineString(coordinates);

            var json = GetExpectedJson();
            var actualLineString = JsonSerializer.DeserializeFromString<GeoLineString>(json);

            Assert.AreEqual(expectedLineString, actualLineString);
        }
    }
}