﻿using System;
using System.Collections.Generic;

namespace OSMDataPrimitives
{
	/// <summary>
	/// OSMElement is the base class for OSMNode, OSMWay and OSMRelation.
	/// </summary>
	public abstract class OsmElement : IOsmElement, ICloneable
	{
		private Dictionary<string, string> tags = new();

		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>The identifier.</value>
		public ulong Id { get; private set; } = 0;
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
        public Dictionary<string, string> Tags {
			get { return this.tags; }
			set {
				this.tags = value ?? throw new ArgumentNullException("Tags");
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:OSMDataPrimitives.OSMElement"/> class.
		/// </summary>
		/// <param name="id">Identifier.</param>
		protected OsmElement(ulong id)
		{
			this.Id = id;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj)) {
				return true;
			}
			if(obj == null || GetType() != obj.GetType()) {
				return false;
			}
			var other = (OsmElement)obj;

			if (this.Tags == null && other.Tags != null) {
				return false;
			}
			if (this.Tags != null && other.Tags == null) {
				return false;
			}
			if (this.Tags.Count != other.Tags.Count) {
				return false;
			}

			foreach (var kvp in this.Tags)
			{
				if (!other.Tags.TryGetValue(kvp.Key, out string valueInDict2)) {
					return false;
				}
				if (kvp.Value != valueInDict2) {
					return false;
				}
			}

			return (
				this.Id == other.Id &&
				this.UserId == other.UserId &&
				this.UserName == other.UserName &&
				this.Version == other.Version &&
				this.Timestamp == other.Timestamp &&
				this.Changeset == other.Changeset
			);
		}

		public override int GetHashCode()
		{
			unchecked {
				return (
					this.Id.GetHashCode() +
					this.UserId.GetHashCode() +
					this.UserName.GetHashCode() +
					this.Version.GetHashCode() +
					this.Timestamp.GetHashCode() +
					this.Changeset.GetHashCode() +
					this.Tags.GetHashCode()
				);
			}
		}

		/// <summary>
		/// Clone this instance.
		/// </summary>
		public object Clone()
		{
			var clone = (OsmElement)this.MemberwiseClone();
			clone.tags = new Dictionary<string, string>(this.tags);

			return clone;
		}

		/// <summary>
		/// Overrides the identifier.
		/// </summary>
		/// <param name="newId">New identifier.</param>
		public void OverrideId(ulong newId)
		{
			this.Id = newId;
		}
	}
}
