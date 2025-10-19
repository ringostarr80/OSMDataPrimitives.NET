using System;
using NUnit.Framework;
using OSMDataPrimitives;
using OSMDataPrimitives.Spatial;

namespace NUnitTests
{
	[TestFixture]
	public class TestOsmWaySpatial
	{
		private static OsmWay GetDefaultOsmWay()
		{
			var way = new OsmWay(2) {
				UserId = 5,
				UserName = "foo",
				Version = 3,
				Changeset = 7,
				Timestamp = new DateTime(2017, 1, 20, 12, 03, 43, DateTimeKind.Utc)
			};
			way.NodeRefs.Add(5);
			way.NodeRefs.Add(9);
			way.NodeRefs.Add(12);
			way.NodeRefs.Add(543);
			way.NodeRefs.Add(43);
			way.NodeRefs.Add(1234151);
			way.Tags.Add("name", "this road");
			way.Tags.Add("ref", "A1");

			return way;
		}

		private static OsmWaySpatial GetDefaultOsmWaySpatial()
		{
			var way = new OsmWaySpatial(19410366) {
				Version = 11,
				Changeset = 29145968,
				UserId = 35935,
				UserName = "13 digits",
				Timestamp = new DateTime(2015, 2, 27, 22, 23, 16, DateTimeKind.Utc)
			};
			way.Tags.Add("cycleway", "street");
			way.Tags.Add("highway", "unclassified");
			way.Tags.Add("maxspeed", "20");
			way.Tags.Add("name", "Plan");
			way.Tags.Add("oneway", "yes");
			way.Tags.Add("postal_code", "20095");
			way.Tags.Add("source:maxspeed", "DE:zone:20");
			way.Tags.Add("surface", "cobblestone");
			way.Nodes.Add(new OsmNodeSpatial(201850689, 53.5510746, 9.9936292));
			way.Nodes.Add(new OsmNodeSpatial(2290487640, 53.5511904, 9.9937873));
			way.Nodes.Add(new OsmNodeSpatial(2587535233, 53.5515143, 9.9943703));
			way.Nodes.Add(new OsmNodeSpatial(201850776, 53.5516129, 9.9945670));

			return way;
		}

		private static OsmWaySpatial GetClosedOsmWaySpatial()
		{
			var way = new OsmWaySpatial(39857589) {
				Version = 6,
				Changeset = 34133920,
				UserId = 5359,
				UserName = "user_5359",
				Timestamp = new DateTime(2015, 9, 20, 6, 45, 35, DateTimeKind.Utc)
			};
			way.Tags.Add("addr:city", "Hamburg");
			way.Tags.Add("addr:country", "DE");
			way.Tags.Add("addr:housename", "Fölsch-Block");
			way.Tags.Add("addr:housenumber", "7");
			way.Tags.Add("addr:postcode", "20095");
			way.Tags.Add("addr:street", "Rathausmarkt");
			way.Tags.Add("building", "yes");
			way.Tags.Add("roof:shape", "gabled");
			way.Nodes.Add(new OsmNodeSpatial(477908592, 53.5511112, 9.9938190));
			way.Nodes.Add(new OsmNodeSpatial(732351378, 53.5512961, 9.9941492));
			way.Nodes.Add(new OsmNodeSpatial(477908594, 53.5514803, 9.9944793));
			way.Nodes.Add(new OsmNodeSpatial(732351355, 53.5514181, 9.9945542));
			way.Nodes.Add(new OsmNodeSpatial(1786096550, 53.5512897, 9.9947158));
			way.Nodes.Add(new OsmNodeSpatial(477908596, 53.5511379, 9.9949159));
			way.Nodes.Add(new OsmNodeSpatial(732351408, 53.5510464, 9.9947638));
			way.Nodes.Add(new OsmNodeSpatial(2292205046, 53.5508649, 9.9944565));
			way.Nodes.Add(new OsmNodeSpatial(477908600, 53.5507743, 9.9942999));
			way.Nodes.Add(new OsmNodeSpatial(1786096546, 53.5509325, 9.9940710));
			way.Nodes.Add(new OsmNodeSpatial(477908592, 53.5511112, 9.9938190));

			return way;
		}

		private static OsmWaySpatial GetClockwiseOsmWaySpatial()
		{
			var way = new OsmWaySpatial(90105666) {
				Version = 3,
				Changeset = 15554642,
				UserId = 173844,
				UserName = "Antikalk",
				Timestamp = new DateTime(2013, 3, 30, 18, 25, 11, DateTimeKind.Utc)
			};
			way.Tags.Add("addr:city", "Düsseldorf");
			way.Tags.Add("addr:country", "DE");
			way.Tags.Add("addr:housenumber", "6");
			way.Tags.Add("addr:postcode", "40591");
			way.Tags.Add("addr:street", "Opladener Straße");
			way.Tags.Add("building", "yes");
			way.Nodes.Add(new OsmNodeSpatial(1044559491, 51.1878774, 6.8192821));
			way.Nodes.Add(new OsmNodeSpatial(1044559375, 51.1878761, 6.8191117));
			way.Nodes.Add(new OsmNodeSpatial(1044559424, 51.1879890, 6.8191095));
			way.Nodes.Add(new OsmNodeSpatial(1044559405, 51.1879903, 6.8192799));
			way.Nodes.Add(new OsmNodeSpatial(1044559535, 51.1879820, 6.8192801));
			way.Nodes.Add(new OsmNodeSpatial(1044559491, 51.1878774, 6.8192821));

			return way;
		}

		private static OsmWaySpatial GetCounterClockwiseOsmWaySpatial()
		{
			var way = new OsmWaySpatial(273579651) {
				Version = 2,
				Changeset = 21634385,
				UserId = 24644,
				UserName = "Athemis",
				Timestamp = new DateTime(2014, 4, 11, 20, 29, 49, DateTimeKind.Utc)
			};
			way.Tags.Add("addr:city", "Düsseldorf");
			way.Tags.Add("addr:country", "DE");
			way.Tags.Add("addr:housenumber", "185");
			way.Tags.Add("addr:postcode", "40591");
			way.Tags.Add("addr:street", "Kölner Landstraße");
			way.Tags.Add("building", "yes");
			way.Nodes.Add(new OsmNodeSpatial(2784120049, 51.1882391, 6.8179689));
			way.Nodes.Add(new OsmNodeSpatial(2784120048, 51.1881964, 6.8178443));
			way.Nodes.Add(new OsmNodeSpatial(2784120046, 51.1880588, 6.8179631));
			way.Nodes.Add(new OsmNodeSpatial(2784120047, 51.1880967, 6.8180917));
			way.Nodes.Add(new OsmNodeSpatial(2784120049, 51.1882391, 6.8179689));

			return way;
		}

		[Test]
		public void TestOsmWaySpatialClone()
		{
			var defaultWay = GetDefaultOsmWaySpatial();
			var clonedWay = (OsmWaySpatial)defaultWay.Clone();
			clonedWay.Nodes[0].Latitude += 0.02;
			clonedWay.Nodes[0].Longitude += 0.01;

			Assert.That(clonedWay.Nodes[0].Latitude, Is.EqualTo(53.5710746));
			Assert.That(clonedWay.Nodes[0].Longitude, Is.EqualTo(10.0036292));

			Assert.That(defaultWay.Nodes[0].Latitude, Is.EqualTo(53.5510746));
			Assert.That(defaultWay.Nodes[0].Longitude, Is.EqualTo(9.9936292));
		}

		[Test]
		public void TestOsmWaySpatialConstructorWithOsmWay()
		{
			var way = GetDefaultOsmWay();
			var waySpatial = new OsmWaySpatial(way);

			waySpatial.Changeset += 1;
			waySpatial.Version += 1;
			waySpatial.UserId = 2;
			waySpatial.UserName = "bar";
			waySpatial.Timestamp = new DateTime(2017, 1, 25, 15, 20, 55, DateTimeKind.Utc);
			waySpatial.Tags["name"] = "that street";
			waySpatial.Tags["ref"] = "B2";
			waySpatial.NodeRefs.Clear();
			waySpatial.NodeRefs.Add(15);
			waySpatial.NodeRefs.Add(19);
			waySpatial.NodeRefs.Add(112);
			waySpatial.NodeRefs.Add(1543);
			waySpatial.NodeRefs.Add(143);
			waySpatial.NodeRefs.Add(11234151);

			Assert.That(way.Changeset, Is.EqualTo(7));
			Assert.That(waySpatial.Changeset, Is.EqualTo(8));
			Assert.That(way.Version, Is.EqualTo(3));
			Assert.That(waySpatial.Version, Is.EqualTo(4));
			Assert.That(way.UserId, Is.EqualTo(5));
			Assert.That(waySpatial.UserId, Is.EqualTo(2));
			Assert.That(way.UserName, Is.EqualTo("foo"));
			Assert.That(waySpatial.UserName, Is.EqualTo("bar"));
			Assert.That(way.Timestamp, Is.EqualTo(new DateTime(2017, 1, 20, 12, 03, 43, DateTimeKind.Utc)));
			Assert.That(waySpatial.Timestamp, Is.EqualTo(new DateTime(2017, 1, 25, 15, 20, 55, DateTimeKind.Utc)));
			Assert.That(way.Tags["name"], Is.EqualTo("this road"));
			Assert.That(waySpatial.Tags["name"], Is.EqualTo("that street"));
			Assert.That(way.Tags["ref"], Is.EqualTo("A1"));
			Assert.That(waySpatial.Tags["ref"], Is.EqualTo("B2"));

			Assert.That(way.NodeRefs[0], Is.EqualTo(5));
			Assert.That(way.NodeRefs[1], Is.EqualTo(9));
			Assert.That(way.NodeRefs[2], Is.EqualTo(12));
			Assert.That(way.NodeRefs[3], Is.EqualTo(543));
			Assert.That(way.NodeRefs[4], Is.EqualTo(43));
			Assert.That(way.NodeRefs[5], Is.EqualTo(1234151));

			Assert.That(waySpatial.NodeRefs[0], Is.EqualTo(15));
			Assert.That(waySpatial.NodeRefs[1], Is.EqualTo(19));
			Assert.That(waySpatial.NodeRefs[2], Is.EqualTo(112));
			Assert.That(waySpatial.NodeRefs[3], Is.EqualTo(1543));
			Assert.That(waySpatial.NodeRefs[4], Is.EqualTo(143));
			Assert.That(waySpatial.NodeRefs[5], Is.EqualTo(11234151));
		}

		[Test]
		public void TestOsmWaySpatialIsNotClosed()
		{
			var defaultWay = GetDefaultOsmWaySpatial();
			Assert.That(defaultWay.IsClosed, Is.False);
		}

		[Test]
		public void TestOsmWaySpatialIsClosed()
		{
			var closedWay = GetClosedOsmWaySpatial();
			Assert.That(closedWay.IsClosed, Is.True);
		}

		[Test]
		public void TestOsmWaySpatialDirectionClockwise()
		{
			var counterClockwiseWay = GetCounterClockwiseOsmWaySpatial();
			Assert.That(counterClockwiseWay.Direction, Is.EqualTo(PolygonDirection.CounterClockwise));
		}

		[Test]
		public void TestOsmWaySpatialDirectionCounterClockwise()
		{
			var clockwiseWay = GetClockwiseOsmWaySpatial();
			Assert.That(clockwiseWay.Direction, Is.EqualTo(PolygonDirection.Clockwise));
		}

		[Test]
		public void TestOsmWaySpatialReverse()
		{
			var clockwiseWay = GetClockwiseOsmWaySpatial();
			var counterClockwiseWay = clockwiseWay.Reverse();
			Assert.That(counterClockwiseWay.Direction, Is.EqualTo(PolygonDirection.CounterClockwise));
		}

		[Test]
		public void TestOsmWaySpatialToWkt()
		{
			var way = GetDefaultOsmWaySpatial();
			var wkt = way.ToWkt();
			const string expectedWkt = "LINESTRING (9.9936292 53.5510746, 9.9937873 53.5511904, 9.9943703 53.5515143, 9.994567 53.5516129)";
			Assert.That(wkt, Is.EqualTo(expectedWkt));
		}

		[Test]
		public void TestOsmWaySpatialPointIsInPolygon()
		{
			var polygon = GetClosedOsmWaySpatial();

			Assert.That(polygon.PointInPolygon(53.5514198, 9.9944637), Is.True);
			Assert.That(polygon.PointInPolygon(53.5511411, 9.9948384), Is.True);
			Assert.That(polygon.PointInPolygon(53.5509208, 9.9944416), Is.True);
			Assert.That(polygon.PointInPolygon(53.5510938, 9.9940587), Is.True);

			Assert.That(polygon.PointInPolygon(53.5512706, 9.9948439), Is.False);
			Assert.That(polygon.PointInPolygon(53.5507803, 9.9945476), Is.False);
			Assert.That(polygon.PointInPolygon(53.5509482, 9.9939608), Is.False);
			Assert.That(polygon.PointInPolygon(53.5513296, 9.9938440), Is.False);
		}
	}
}
