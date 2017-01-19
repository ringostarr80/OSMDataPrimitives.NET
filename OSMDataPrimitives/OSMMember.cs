using System;

namespace OSMDataPrimitives
{
	/// <summary>
	/// OSMMember.
	/// </summary>
	public struct OSMMember
	{
		/// <summary>
		/// The member-type.
		/// </summary>
		public MemberType Type;
		/// <summary>
		/// The member-reference-id.
		/// </summary>
		public long Ref;
		/// <summary>
		/// The member-role (usually "inner" or "outer").
		/// </summary>
		public string Role;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:OSMDataPrimitives.OSMMember"/> struct.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="reference">Reference.</param>
		/// <param name="role">Role.</param>
		public OSMMember(MemberType type, long reference, string role)
		{
			this.Type = type;
			this.Ref = reference;
			this.Role = role;
		}
	}
}
