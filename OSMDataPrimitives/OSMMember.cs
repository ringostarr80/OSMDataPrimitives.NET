using System.Collections.Generic;

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
        /// Initializes a new instance of the <see cref="T:OSMDataPrimitives.OSMMember"/> struct.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="reference">Reference.</param>
        /// <param name="role">Role.</param>
        public OSMMember(MemberType type, ulong reference, string role)
		{
			this.Type = type;
			this.Ref = reference;
			this.Role = role;
		}

        public Dictionary<string, string> ToDictionary()
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
