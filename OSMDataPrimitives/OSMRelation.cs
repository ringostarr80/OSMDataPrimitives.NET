using System;
using System.Collections.Generic;

namespace OSMDataPrimitives
{
	/// <summary>
	/// OSMRelation.
	/// </summary>
	public class OSMRelation : OSMElement
	{
		private List<OSMMember> _members = new();

		/// <summary>
		/// Gets or sets the members.
		/// </summary>
		/// <value>The members.</value>
		public List<OSMMember> Members {
			get { return this._members; }
			set {
				this._members = value ?? throw new NullReferenceException("Members can't be null.");
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:OSMDataPrimitives.OSMRelation"/> class.
		/// </summary>
		/// <param name="id">Identifier.</param>
		public OSMRelation(ulong id) : base(id)
		{

		}

		/// <summary>
		/// Clone this instance.
		/// </summary>
		public new object Clone()
		{
			var clone = (OSMRelation)base.Clone();
			clone._members = new List<OSMMember>(this._members);

			return clone;
		}
	}
}
