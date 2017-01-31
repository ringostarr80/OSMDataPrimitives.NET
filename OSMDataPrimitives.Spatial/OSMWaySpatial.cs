using System;
using System.Collections.Generic;

namespace OSMDataPrimitives.Spatial
{
	/// <summary>
	/// OSMWaySpatial.
	/// </summary>
	public class OSMWaySpatial : OSMWay
	{
		private List<OSMNodeSpatial> _nodes = new List<OSMNodeSpatial>();

		/// <summary>
		/// Gets or sets the nodes.
		/// </summary>
		/// <value>The nodes.</value>
		public List<OSMNodeSpatial> Nodes {
			get { return this._nodes; }
			set {
				if(value == null) {
					throw new NullReferenceException("Nodes can't be null.");
				}
				this._nodes = value;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:OSMDataPrimitives.Spatial.OSMWaySpatial"/> is closed.
		/// </summary>
		/// <value><c>true</c> if this way is a closed line (polygon); otherwise, <c>false</c>.</value>
		public bool IsClosed {
			get {
				if(this._nodes.Count < 3) {
					return false;
				}
				var lastNodeIndex = this._nodes.Count - 1;
				if(Math.Abs(this._nodes[0].Latitude - this._nodes[lastNodeIndex].Latitude) > double.Epsilon) {
					return false;
				}
				if(Math.Abs(this._nodes[0].Longitude - this._nodes[lastNodeIndex].Longitude) > double.Epsilon) {
					return false;
				}

				return true;
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
		/// Clone this instance.
		/// </summary>
		public new object Clone()
		{
			var clone = (OSMWaySpatial)base.Clone();
			clone._nodes = new List<OSMNodeSpatial>();
			foreach(var node in this._nodes) {
				clone._nodes.Add((OSMNodeSpatial)node.Clone());
			}

			return clone;
		}
	}
}
