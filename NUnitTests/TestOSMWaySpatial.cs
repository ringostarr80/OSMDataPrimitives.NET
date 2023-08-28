using System;
using NUnit.Framework;
using OSMDataPrimitives;
using OSMDataPrimitives.Spatial;

namespace NUnit
{
	[TestFixture]
	public class TestOSMWaySpatial
	{
		private static OSMWay GetDefaultOSMWay()
		{
			var way = new OSMWay(2) {
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

		private static OSMWaySpatial GetDefaultOSMWaySpatial()
		{
			var way = new OSMWaySpatial(19410366) {
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
			way.Nodes.Add(new OSMNodeSpatial(201850689, 53.5510746, 9.9936292));
			way.Nodes.Add(new OSMNodeSpatial(2290487640, 53.5511904, 9.9937873));
			way.Nodes.Add(new OSMNodeSpatial(2587535233, 53.5515143, 9.9943703));
			way.Nodes.Add(new OSMNodeSpatial(201850776, 53.5516129, 9.9945670));

			return way;
		}

		private static OSMWaySpatial GetClosedOSMWaySpatial()
		{
			var way = new OSMWaySpatial(39857589) {
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
			way.Nodes.Add(new OSMNodeSpatial(477908592, 53.5511112, 9.9938190));
			way.Nodes.Add(new OSMNodeSpatial(732351378, 53.5512961, 9.9941492));
			way.Nodes.Add(new OSMNodeSpatial(477908594, 53.5514803, 9.9944793));
			way.Nodes.Add(new OSMNodeSpatial(732351355, 53.5514181, 9.9945542));
			way.Nodes.Add(new OSMNodeSpatial(1786096550, 53.5512897, 9.9947158));
			way.Nodes.Add(new OSMNodeSpatial(477908596, 53.5511379, 9.9949159));
			way.Nodes.Add(new OSMNodeSpatial(732351408, 53.5510464, 9.9947638));
			way.Nodes.Add(new OSMNodeSpatial(2292205046, 53.5508649, 9.9944565));
			way.Nodes.Add(new OSMNodeSpatial(477908600, 53.5507743, 9.9942999));
			way.Nodes.Add(new OSMNodeSpatial(1786096546, 53.5509325, 9.9940710));
			way.Nodes.Add(new OSMNodeSpatial(477908592, 53.5511112, 9.9938190));

			return way;
		}

		private static OSMWaySpatial GetClockwiseOSMWaySpatial()
		{
			var way = new OSMWaySpatial(90105666) {
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
			way.Nodes.Add(new OSMNodeSpatial(1044559491, 51.1878774, 6.8192821));
			way.Nodes.Add(new OSMNodeSpatial(1044559375, 51.1878761, 6.8191117));
			way.Nodes.Add(new OSMNodeSpatial(1044559424, 51.1879890, 6.8191095));
			way.Nodes.Add(new OSMNodeSpatial(1044559405, 51.1879903, 6.8192799));
			way.Nodes.Add(new OSMNodeSpatial(1044559535, 51.1879820, 6.8192801));
			way.Nodes.Add(new OSMNodeSpatial(1044559491, 51.1878774, 6.8192821));

			return way;
		}

		private static OSMWaySpatial GetCounterClockwiseOSMWaySpatial()
		{
			var way = new OSMWaySpatial(273579651) {
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
			way.Nodes.Add(new OSMNodeSpatial(2784120049, 51.1882391, 6.8179689));
			way.Nodes.Add(new OSMNodeSpatial(2784120048, 51.1881964, 6.8178443));
			way.Nodes.Add(new OSMNodeSpatial(2784120046, 51.1880588, 6.8179631));
			way.Nodes.Add(new OSMNodeSpatial(2784120047, 51.1880967, 6.8180917));
			way.Nodes.Add(new OSMNodeSpatial(2784120049, 51.1882391, 6.8179689));

			return way;
		}

		[Test]
		public void TestOSMWaySpatialClone()
		{
			var defaultWay = GetDefaultOSMWaySpatial();
			var clonedWay = (OSMWaySpatial)defaultWay.Clone();
			clonedWay.Nodes[0].Latitude += 0.02;
			clonedWay.Nodes[0].Longitude += 0.01;

			Assert.AreEqual(53.5710746, clonedWay.Nodes[0].Latitude);
			Assert.AreEqual(10.0036292, clonedWay.Nodes[0].Longitude);

			Assert.AreEqual(53.5510746, defaultWay.Nodes[0].Latitude);
			Assert.AreEqual(9.9936292, defaultWay.Nodes[0].Longitude);
		}

		[Test]
		public void TestOSMWaySpatialConstructorWithOSMWay()
		{
			var way = GetDefaultOSMWay();
			var waySpatial = new OSMWaySpatial(way);

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

			Assert.AreEqual(7, way.Changeset);
			Assert.AreEqual(8, waySpatial.Changeset);
			Assert.AreEqual(3, way.Version);
			Assert.AreEqual(4, waySpatial.Version);
			Assert.AreEqual(5, way.UserId);
			Assert.AreEqual(2, waySpatial.UserId);
			Assert.AreEqual("foo", way.UserName);
			Assert.AreEqual("bar", waySpatial.UserName);
			Assert.AreEqual(new DateTime(2017, 1, 20, 12, 03, 43, DateTimeKind.Utc), way.Timestamp);
			Assert.AreEqual(new DateTime(2017, 1, 25, 15, 20, 55, DateTimeKind.Utc), waySpatial.Timestamp);
			Assert.AreEqual("this road", way.Tags["name"]);
			Assert.AreEqual("that street", waySpatial.Tags["name"]);
			Assert.AreEqual("A1", way.Tags["ref"]);
			Assert.AreEqual("B2", waySpatial.Tags["ref"]);

			Assert.AreEqual(5, way.NodeRefs[0]);
			Assert.AreEqual(9, way.NodeRefs[1]);
			Assert.AreEqual(12, way.NodeRefs[2]);
			Assert.AreEqual(543, way.NodeRefs[3]);
			Assert.AreEqual(43, way.NodeRefs[4]);
			Assert.AreEqual(1234151, way.NodeRefs[5]);

			Assert.AreEqual(15, waySpatial.NodeRefs[0]);
			Assert.AreEqual(19, waySpatial.NodeRefs[1]);
			Assert.AreEqual(112, waySpatial.NodeRefs[2]);
			Assert.AreEqual(1543, waySpatial.NodeRefs[3]);
			Assert.AreEqual(143, waySpatial.NodeRefs[4]);
			Assert.AreEqual(11234151, waySpatial.NodeRefs[5]);
		}

		[Test]
		public void TestOSMWaySpatialIsNotClosed()
		{
			var defaultWay = GetDefaultOSMWaySpatial();
			Assert.AreEqual(false, defaultWay.IsClosed);
		}

		[Test]
		public void TestOSMWaySpatialIsClosed()
		{
			var closedWay = GetClosedOSMWaySpatial();
			Assert.AreEqual(true, closedWay.IsClosed);
		}

		[Test]
		public void TestOSMWaySpatialDirectionClockwise()
		{
			var counterClockwiseWay = GetCounterClockwiseOSMWaySpatial();
			Assert.AreEqual(PolygonDirection.CounterClockwise, counterClockwiseWay.Direction);
		}

		[Test]
		public void TestOSMWaySpatialDirectionCounterClockwise()
		{
			var clockwiseWay = GetClockwiseOSMWaySpatial();
			Assert.AreEqual(PolygonDirection.Clockwise, clockwiseWay.Direction);
		}

		[Test]
		public void TestOSMWaySpatialReverse()
		{
			var clockwiseWay = GetClockwiseOSMWaySpatial();
			var counterClockwiseWay = clockwiseWay.Reverse();
			Assert.AreEqual(PolygonDirection.CounterClockwise, counterClockwiseWay.Direction);
		}

		[Test]
		public void TestOSMWaySpatialToWkt()
		{
			var way = GetDefaultOSMWaySpatial();
			var wkt = way.ToWkt();
			var expectedWkt = "LINESTRING (9.9936292 53.5510746, 9.9937873 53.5511904, 9.9943703 53.5515143, 9.994567 53.5516129)";
			Assert.AreEqual(expectedWkt, wkt);
		}

		[Test]
		public void TestOSMWaySpatialPointIsInPolygon()
		{
			var polygon = GetClosedOSMWaySpatial();

			Assert.True(polygon.PointInPolygon(53.5514198, 9.9944637));
			Assert.True(polygon.PointInPolygon(53.5511411, 9.9948384));
			Assert.True(polygon.PointInPolygon(53.5509208, 9.9944416));
			Assert.True(polygon.PointInPolygon(53.5510938, 9.9940587));

			Assert.False(polygon.PointInPolygon(53.5512706, 9.9948439));
			Assert.False(polygon.PointInPolygon(53.5507803, 9.9945476));
			Assert.False(polygon.PointInPolygon(53.5509482, 9.9939608));
			Assert.False(polygon.PointInPolygon(53.5513296, 9.9938440));
		}
	}
}
