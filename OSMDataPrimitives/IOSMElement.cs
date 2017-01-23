using System;
using System.Collections.Specialized;

namespace OSMDataPrimitives
{
	/// <summary>
	/// IOSMElement is the interface for OSMNode, OSMWay and OSMRelation
	/// </summary>
	public interface IOSMElement
	{
		/// <summary>
		/// Gets the identifier.
		/// </summary>
		/// <value>The identifier.</value>
		ulong Id { get; }
		/// <summary>
		/// Gets or sets the version.
		/// </summary>
		/// <value>The version.</value>
		ulong Version { get; set; }
		/// <summary>
		/// Gets or sets the timestamp.
		/// </summary>
		/// <value>The timestamp.</value>
		DateTime Timestamp { get; set; }
		/// <summary>
		/// Gets or sets the changeset.
		/// </summary>
		/// <value>The changeset.</value>
		ulong Changeset { get; set; }
		/// <summary>
		/// Gets or sets the user identifier.
		/// </summary>
		/// <value>The user identifier.</value>
		ulong UserId { get; set; }
		/// <summary>
		/// Gets or sets the name of the user.
		/// </summary>
		/// <value>The name of the user.</value>
		string UserName { get; set; }
		/// <summary>
		/// Gets the tags.
		/// </summary>
		/// <value>The tags.</value>
		NameValueCollection Tags { get; }
	}
}
