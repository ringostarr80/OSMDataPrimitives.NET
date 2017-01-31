﻿using System;
using NUnit.Framework;
using OSMDataPrimitives.Spatial;

namespace NUnit
{
	[TestFixture]
	public class TestOSMWaySpatial
	{
		private OSMWaySpatial GetDefaultOSMWaySpatial()
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

		private OSMWaySpatial GetClosedOSMWaySpatial()
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

		[Test]
		public void TestOSMWaySpatialClone()
		{
			var defaultWay = this.GetDefaultOSMWaySpatial();
			var clonedWay = (OSMWaySpatial)defaultWay.Clone();
			clonedWay.Nodes[0].Latitude += 0.02;
			clonedWay.Nodes[0].Longitude += 0.01;

			Assert.AreEqual(53.5710746, clonedWay.Nodes[0].Latitude);
			Assert.AreEqual(10.0036292, clonedWay.Nodes[0].Longitude);

			Assert.AreEqual(53.5510746, defaultWay.Nodes[0].Latitude);
			Assert.AreEqual(9.9936292, defaultWay.Nodes[0].Longitude);
		}

		[Test]
		public void TestOSMWaySpatialIsNotClosed()
		{
			var defaultWay = this.GetDefaultOSMWaySpatial();
			Assert.AreEqual(false, defaultWay.IsClosed);
		}

		[Test]
		public void TestOSMWaySpatialIsClosed()
		{
			var closedWay = this.GetClosedOSMWaySpatial();
			Assert.AreEqual(true, closedWay.IsClosed);
		}
	}
}