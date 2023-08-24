using System;
using System.Collections.Specialized;

namespace OSMDataPrimitives
{
	/// <summary>
	/// OSMElement is the base class for OSMNode, OSMWay and OSMRelation.
	/// </summary>
	public abstract class OSMElement : IOSMElement, ICloneable
	{
		private ulong _id = 0;
		private NameValueCollection _tags = new();

		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>The identifier.</value>
		public ulong Id { get { return this._id; } }
        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        public ulong Version { get; set; } = 0;
        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        /// <value>The timestamp.</value>
        public DateTime Timestamp { get; set; } = DateTime.UnixEpoch;
        /// <summary>
        /// Gets or sets the changeset.
        /// </summary>
        /// <value>The changeset.</value>
        public ulong Changeset { get; set; } = 0;
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>The user identifier.</value>
        public ulong UserId { get; set; } = 0;
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        public string UserName { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        /// <value>The tags.</value>
        public NameValueCollection Tags {
			get { return this._tags; }
			set {
				this._tags = value ?? throw new NullReferenceException("Tags can't be null.");
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:OSMDataPrimitives.OSMElement"/> class.
		/// </summary>
		/// <param name="id">Identifier.</param>
		protected OSMElement(ulong id)
		{
			this._id = id;
		}

		/// <summary>
		/// Clone this instance.
		/// </summary>
		public object Clone()
		{
			var clone = (OSMElement)this.MemberwiseClone();
			clone._tags = new NameValueCollection(this._tags);

			return clone;
		}

		/// <summary>
		/// Overrides the identifier.
		/// </summary>
		/// <param name="newId">New identifier.</param>
		public void OverrideId(ulong newId)
		{
			this._id = newId;
		}
	}
}
