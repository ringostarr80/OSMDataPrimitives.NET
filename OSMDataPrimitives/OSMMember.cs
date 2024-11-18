using System.Collections.Generic;

namespace OSMDataPrimitives
{
	/// <summary>
	/// OsmMember.
	/// </summary>
	public struct OsmMember
	{
		/// <summary>
		/// The member-type.
		/// </summary>
		public MemberType Type { get; set; }

		/// <summary>
		/// The member-reference-id.
		/// </summary>
		public ulong Ref { get; set; }

		/// <summary>
		/// The member-role (usually "inner" or "outer").
		/// </summary>
		public string Role { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:OSMDataPrimitives.OsmMember"/> struct.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="reference">Reference.</param>
		/// <param name="role">Role.</param>
		public OsmMember(MemberType type, ulong reference, string role)
		{
			this.Type = type;
			this.Ref = reference;
			this.Role = role;
		}

		/// <summary>
		/// Converts the OsmMember object to a dictionary.
		/// </summary>
		/// <returns>A dictionary containing the member's properties.</returns>
		public readonly Dictionary<string, string> ToDictionary()
		{
			return new Dictionary<string, string>
			{
				{ "ref", this.Ref.ToString() },
				{ "type", this.Type.ToString().ToLower() },
				{ "role", this.Role }
			};
		}
	}
}
