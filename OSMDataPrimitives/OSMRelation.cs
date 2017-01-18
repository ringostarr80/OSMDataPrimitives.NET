using System.Collections.Generic;

namespace OSMDataPrimitives
{
	/// <summary>
	/// OSMRelation.
	/// </summary>
	public class OSMRelation : OSMElement
	{
		private List<OSMMember> _members = new List<OSMMember>();

		/// <summary>
		/// Gets the members.
		/// </summary>
		/// <value>The members.</value>
		public List<OSMMember> Members { get { return this._members; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:OSMDataPrimitives.OSMRelation"/> class.
		/// </summary>
		/// <param name="id">Identifier.</param>
		public OSMRelation(long id) : base(id)
		{

		}
	}
}
