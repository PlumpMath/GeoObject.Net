using GeoObject.Net.CoordinateReferenceSystem;
using GeoObject.Net.Feature;
using GeoObject.Net.Geometry;
using ServiceStack.Text;
using NUnit.Framework;

namespace GeoObject.Net.Tests.CoordinateReferenceSystem
{
    [TestFixture]
    public class DefaultCrsTests : TestBase
    {
        [Test]
        public void Can_Serialize_Does_Not_Output_Crs_Property()
        {
            var collection = new FeatureCollection();

            var json = JsonSerializer.SerializeToString(collection);

            Assert.IsTrue(!json.Contains("\"crs\""));
        }

        [Test]
        public void Can_Deserialize_When_Json_Does_Not_Contain_Crs_Property()
        {
            var json = "{\"coordinates\":[90.65464646,53.2455662,200.4567],\"type\":\"Point\"}";

            var point = JsonSerializer.DeserializeFromString<GeoPoint>(json);

            Assert.IsNull(point.CRS);
            // Assert.IsInstanceOf<DefaultCRS>(point.CRS);
        }
    }
}