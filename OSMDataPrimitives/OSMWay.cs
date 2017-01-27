using System;
using System.Collections.Generic;

namespace OSMDataPrimitives
{
	/// <summary>
	/// OSMWay.
	/// </summary>
	public class OSMWay : OSMElement
	{
		private List<ulong> _nodeRefs = new List<ulong>();

		/// <summary>
		/// Gets or sets the node reference-ids.
		/// </summary>
		/// <value>The node reference-ids.</value>
		public List<ulong> NodeRefs {
			get { return this._nodeRefs; }
			set {
				if(value == null) {
					throw new NullReferenceException("NodeRefs can't be null.");
				}
				this._nodeRefs = value;
			}
		}

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
			clone._nodeRefs = new List<ulong>(this._nodeRefs);

			return clone;
		}
	}
}
