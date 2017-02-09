using System;
using System.Collections.Generic;

namespace OSMDataPrimitives.Spatial
{
	/// <summary>
	/// OSMWaySpatialCollection.
	/// </summary>
	public class OSMWaySpatialCollection : List<OSMWaySpatial>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:OSMDataPrimitives.Spatial.OSMWaySpatialCollection"/> class.
		/// </summary>
		public OSMWaySpatialCollection()
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:OSMDataPrimitives.Spatial.OSMWaySpatialCollection"/> class.
		/// </summary>
		/// <param name="ways">Ways.</param>
		public OSMWaySpatialCollection(IEnumerable<OSMWaySpatial> ways)
		{
			this.AddRange(ways);
		}

		/// <summary>
		/// This merges all the containing ways to one way for those, that can be merged.
		/// </summary>
		public OSMWaySpatialCollection Merge()
		{
			var mergedWays = new OSMWaySpatialCollection();
			foreach(var way in this) {
				if(way.Nodes.Count > 1) {
					mergedWays.Add((OSMWaySpatial)way.Clone());
				}
			}

			var merged = false;
			var mergedWaysCount = 0;
			do {
				merged = false;
				mergedWaysCount = mergedWays.Count;
				for(var i = 0; i < mergedWaysCount - 1; i++) {
					var currentWay = mergedWays[i];
					for(var innerI = i + 1; innerI < mergedWaysCount; innerI++) {
						var nextWay = mergedWays[innerI];
						var currentLastNode = currentWay.Nodes[currentWay.Nodes.Count - 1];
						var nextFirstNode = nextWay.Nodes[0];
						if(Math.Abs(currentLastNode.Latitude - nextFirstNode.Latitude) < double.Epsilon && Math.Abs(currentLastNode.Longitude - nextFirstNode.Longitude) < double.Epsilon) {
							for(var j = 1; j < nextWay.Nodes.Count; j++) {
								mergedWays[i].Nodes.Add(nextWay.Nodes[j]);
							}
							merged = true;
							mergedWays.RemoveAt(innerI);
							break;
						}
						var nextLastNode = nextWay.Nodes[nextWay.Nodes.Count - 1];
						if(Math.Abs(currentLastNode.Latitude - nextLastNode.Latitude) < double.Epsilon && Math.Abs(currentLastNode.Longitude - nextLastNode.Longitude) < double.Epsilon) {
							for(var j = nextWay.Nodes.Count - 2; j >= 0; j--) {
								mergedWays[i].Nodes.Add(nextWay.Nodes[j]);
							}
							merged = true;
							mergedWays.RemoveAt(innerI);
							break;
						}
						var currentFirstNode = currentWay.Nodes[0];
						if(Math.Abs(currentFirstNode.Latitude - nextFirstNode.Latitude) < double.Epsilon && Math.Abs(currentFirstNode.Longitude - nextFirstNode.Longitude) < double.Epsilon) {
							for(var j = 1; j < nextWay.Nodes.Count; j++) {
								mergedWays[i].Nodes.Insert(0, nextWay.Nodes[j]);
							}
							merged = true;
							mergedWays.RemoveAt(innerI);
							break;
						}
						if(Math.Abs(currentFirstNode.Latitude - nextLastNode.Latitude) < double.Epsilon && Math.Abs(currentFirstNode.Longitude - nextLastNode.Longitude) < double.Epsilon) {
							for(var j = nextWay.Nodes.Count - 2; j >= 0; j--) {
								mergedWays[i].Nodes.Insert(0, nextWay.Nodes[j]);
							}
							merged = true;
							mergedWays.RemoveAt(innerI);
							break;
						}
					}

					if(merged) {
						mergedWaysCount = mergedWays.Count;
					}
				}
			} while(merged);

			return mergedWays;
		}

		/// <summary>
		/// Ensures the polygon direction.
		/// Infos: http://wiki.openstreetmap.org/wiki/Relation:multipolygon/Algorithm
		/// </summary>
		/// <param name="role">Role.</param>
		public void EnsurePolygonDirection(string role = null)
		{
			for(var i = 0; i < this.Count; i++) {
				if(!this[i].IsClosed) {
					continue;
				}

				if(string.IsNullOrEmpty(role)) {
					role = this[i].Role;
				}
				var polygonDirection = this[i].Direction;
				if(role == "inner") {
					if(polygonDirection != PolygonDirection.CounterClockwise) {
						this[i] = this[i].Reverse();
					}
				} else if(role == "outer") {
					if(polygonDirection != PolygonDirection.Clockwise) {
						this[i] = this[i].Reverse();
					}
				}
			}
		}

		/// <summary>
		/// Removes the invalid polygons (not closed ways).
		/// </summary>
		public void RemoveInvalidPolygons()
		{
			for(var i = this.Count - 1; i >= 0; i--) {
				if(!this[i].IsClosed) {
					this.RemoveAt(i);
					continue;
				}
				if(this[i].Nodes.Count == 3 && Math.Abs(this[i].Nodes[0].Latitude - this[i].Nodes[2].Latitude) < double.Epsilon && Math.Abs(this[i].Nodes[0].Longitude - this[i].Nodes[2].Longitude) < double.Epsilon) {
					this.RemoveAt(i);
					continue;
				}
			}
		}
	}
}
