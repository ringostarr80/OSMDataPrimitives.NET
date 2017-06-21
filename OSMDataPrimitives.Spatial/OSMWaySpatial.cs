using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace OSMDataPrimitives.Spatial
{
	/// <summary>
	/// OSMWaySpatial.
	/// </summary>
	public class OSMWaySpatial : OSMWay
	{
		private List<OSMNodeSpatial> _nodes = new List<OSMNodeSpatial>();
		private string _role = "outer";

		/// <summary>
		/// Gets or sets the nodes.
		/// </summary>
		/// <value>The nodes.</value>
		public List<OSMNodeSpatial> Nodes {
			get { return this._nodes; }
			set {
				this._nodes = value ?? throw new NullReferenceException("Nodes can't be null.");
			}
		}

		/// <summary>
		/// Gets or sets the role.
		/// </summary>
		/// <value>The role.</value>
		public string Role { get { return this._role; } set { this._role = value; } }

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:OSMDataPrimitives.Spatial.OSMWaySpatial"/> is closed.
		/// </summary>
		/// <value><c>true</c> if this way is a closed line (polygon); otherwise, <c>false</c>.</value>
		public bool IsClosed {
			get {
				if (this._nodes.Count < 3) {
					return false;
				}
				var lastNodeIndex = this._nodes.Count - 1;
				if (Math.Abs(this._nodes[0].Latitude - this._nodes[lastNodeIndex].Latitude) > double.Epsilon) {
					return false;
				}
				if (Math.Abs(this._nodes[0].Longitude - this._nodes[lastNodeIndex].Longitude) > double.Epsilon) {
					return false;
				}

				return true;
			}
		}

		/// <summary>
		/// Gets the direction (Clockwise or CounterClockwise).
		/// </summary>
		/// <value>The direction.</value>
		public PolygonDirection Direction {
			get {
				var nodesCount = this._nodes.Count;
				var sum = 0.0;
				for (var i = 0; i < nodesCount; i++) {
					var nextNodeIndex = i + 1;
					if (i == nodesCount - 1) {
						nextNodeIndex = 0;
					}
					var longitudeDiff = this._nodes[nextNodeIndex].Longitude - this._nodes[i].Longitude;
					var latitudeDiff = this._nodes[nextNodeIndex].Latitude + this._nodes[i].Latitude;
					sum += longitudeDiff * latitudeDiff;
				}

				return (sum >= 0.0) ? PolygonDirection.Clockwise : PolygonDirection.CounterClockwise;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:OSMDataPrimitives.Spatial.OSMWaySpatial"/> class.
		/// </summary>
		/// <param name="id">Identifier.</param>
		public OSMWaySpatial(ulong id) : base(id)
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:OSMDataPrimitives.Spatial.OSMWaySpatial"/> class.
		/// </summary>
		/// <param name="way">Way.</param>
		public OSMWaySpatial(OSMWay way) : base(way.Id)
		{
			this.Changeset = way.Changeset;
			this.Timestamp = way.Timestamp;
			this.UserId = way.UserId;
			this.UserName = way.UserName;
			this.Version = way.Version;
			this.Tags = new NameValueCollection(way.Tags);
			this.NodeRefs = new List<ulong>(way.NodeRefs);
		}

		/// <summary>
		/// Clone this instance.
		/// </summary>
		public new object Clone()
		{
			var clone = (OSMWaySpatial)base.Clone();
			clone._nodes = new List<OSMNodeSpatial>();
			foreach (var node in this._nodes) {
				clone._nodes.Add((OSMNodeSpatial)node.Clone());
			}

			return clone;
		}

		/// <summary>
		/// Reverse the node-order of this instance.
		/// </summary>
		public OSMWaySpatial Reverse()
		{
			var reversedWay = (OSMWaySpatial)this.Clone();
			reversedWay.Nodes.Reverse();
			return reversedWay;
		}

		/// <summary>
		/// Checks, if the given Point is in this polygon.
		/// </summary>
		/// <returns><c>true</c>, if this Point is in this polygon, <c>false</c> otherwise.</returns>
		/// <param name="latitude">Latitude.</param>
		/// <param name="longitude">Longitude.</param>
		public bool PointInPolygon(double latitude, double longitude)
		{
			if (!this.IsClosed) {
				return false;
			}

			var result = false;
			var nodesCount = this.Nodes.Count;
			var j = nodesCount - 1;
			for (var i = 0; i < nodesCount; j = i++) {
				var iNode = this._nodes[i];
				var jNode = this._nodes[j];
				if (((iNode.Latitude > latitude) != (jNode.Latitude > latitude)) &&
				   (longitude < (jNode.Longitude - iNode.Longitude) * (latitude - iNode.Latitude) / (jNode.Latitude - iNode.Latitude) + iNode.Longitude)) {
					result = !result;
				}
			}

			return result;
		}
	}
}
