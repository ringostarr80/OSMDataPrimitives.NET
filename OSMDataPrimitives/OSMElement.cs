using System;
using System.Collections.Specialized;

namespace OSMDataPrimitives
{
	/// <summary>
	/// OSMElement is the base class for OSMNode, OSMWay and OSMRelation.
	/// </summary>
	public abstract class OSMElement : IOSMElement
	{
		private long _id = 0;
		private ulong _version = 0;
		private DateTime _timestamp = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		private long _changeset = 0;
		private ulong _userId = 0;
		private string _userName = string.Empty;
		private NameValueCollection _tags = new NameValueCollection();

		/// <summary>
		/// Gets the identifier.
		/// </summary>
		/// <value>The identifier.</value>
		public long Id { get { return this._id; } }
		/// <summary>
		/// Gets or sets the version.
		/// </summary>
		/// <value>The version.</value>
		public ulong Version { get { return this._version; } set { this._version = value; } }
		/// <summary>
		/// Gets or sets the timestamp.
		/// </summary>
		/// <value>The timestamp.</value>
		public DateTime Timestamp { get { return this._timestamp; } set { this._timestamp = value; } }
		/// <summary>
		/// Gets or sets the changeset.
		/// </summary>
		/// <value>The changeset.</value>
		public long Changeset { get { return this._changeset; } set { this._changeset = value; } }
		/// <summary>
		/// Gets or sets the user identifier.
		/// </summary>
		/// <value>The user identifier.</value>
		public ulong UserId { get { return this._userId; } set { this._userId = value; } }
		/// <summary>
		/// Gets or sets the name of the user.
		/// </summary>
		/// <value>The name of the user.</value>
		public string UserName { get { return this._userName; } set { this._userName = value; } }
		/// <summary>
		/// Gets the tags.
		/// </summary>
		/// <value>The tags.</value>
		public NameValueCollection Tags { get { return this._tags; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:OSMDataPrimitives.OSMElement"/> class.
		/// </summary>
		/// <param name="id">Identifier.</param>
		protected OSMElement(long id)
		{
			this._id = id;
		}
	}
}
