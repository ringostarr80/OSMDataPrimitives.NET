using System.Collections.Generic;

namespace OSMDataPrimitives
{
	/// <summary>
	/// OSMWay.
	/// </summary>
	public class OSMWay : OSMElement
	{
		private List<long> _nodeRefs = new List<long>();

		/// <summary>
		/// Gets the node reference-ids.
		/// </summary>
		/// <value>The node reference-ids.</value>
		public List<long> NodeRefs { get { return this._nodeRefs; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:OSMDataPrimitives.OSMWay"/> class.
		/// </summary>
		/// <param name="id">Identifier.</param>
		public OSMWay(ulong id) : base(id)
		{

		}

		/// <summary>
		/// Clone this instance.
		/// </summary>
		public new object Clone()
		{
			var clone = (OSMWay)base.Clone();
			clone._nodeRefs = new List<long>(this._nodeRefs);

			return clone;
		}
	}
}
