using System;
using System.Collections.Generic;

namespace OSMDataPrimitives
{
	/// <summary>
	/// OSMRelation.
	/// </summary>
	public class OsmRelation : OsmElement
	{
		private List<OsmMember> members = new();

		/// <summary>
		/// Gets or sets the members.
		/// </summary>
		/// <value>The members.</value>
		public List<OsmMember> Members {
			get { return this.members; }
			set {
				this.members = value ?? throw new ArgumentNullException("Members");
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:OSMDataPrimitives.OSMRelation"/> class.
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
			clone.members = new List<OsmMember>(this.members);

			return clone;
		}
	}
}
