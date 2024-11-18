using System;
using System.Collections.Generic;

namespace OSMDataPrimitives
{
	/// <summary>
	/// OsmWay.
	/// </summary>
	public class OsmWay : OsmElement
	{
		private List<long> _nodeRefs = [];

		/// <summary>
		/// Gets or sets the node reference-ids.
		/// </summary>
		/// <value>The node reference-ids.</value>
		public List<long> NodeRefs
		{
			get => this._nodeRefs;
			set => this._nodeRefs = value ?? throw new ArgumentNullException(nameof(NodeRefs));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:OSMDataPrimitives.OsmWay"/> class.
		/// </summary>
		/// <param name="id">Identifier.</param>
		public OsmWay(ulong id) : base(id)
		{

		}

		/// <summary>
		/// Clone this instance.
		/// </summary>
		public new object Clone()
		{
			var clone = (OsmWay)base.Clone();
			clone._nodeRefs = [..this._nodeRefs];

			return clone;
		}
	}
}
