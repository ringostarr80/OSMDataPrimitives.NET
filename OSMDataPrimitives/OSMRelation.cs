using System;
using System.Collections.Generic;

namespace OSMDataPrimitives
{
	/// <summary>
	/// OsmRelation.
	/// </summary>
	public class OsmRelation : OsmElement
	{
		private List<OsmMember> _members = [];

		/// <summary>
		/// Gets or sets the members.
		/// </summary>
		/// <value>The members.</value>
		public List<OsmMember> Members
		{
			get => this._members;
			set => this._members = value ?? throw new ArgumentNullException(nameof(Members));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:OSMDataPrimitives.OsmRelation"/> class.
		/// </summary>
		/// <param name="id">Identifier.</param>
		public OsmRelation(ulong id) : base(id)
		{

		}

		/// <summary>
		/// Clone this instance.
		/// </summary>
		public new object Clone()
		{
			var clone = (OsmRelation)base.Clone();
			clone._members = [..this._members];

			return clone;
		}
	}
}
