using System;

namespace OSMDataPrimitives
{
	/// <summary>
	/// OSMNode.
	/// </summary>
	public class OsmNode : OsmElement
	{
		private double latitude = 0.0;
		private double longitude = 0.0;

		/// <summary>
		/// Gets or sets the latitude.
		/// </summary>
		/// <value>The latitude.</value>
		/// <exception cref="T:System.ArgumentOutOfRangeException"></exception>
		public double Latitude {
			get { return this.latitude; }
			set {
				if(value < -90.0 || value > 90.0) {
					throw new ArgumentOutOfRangeException("Latitude", value, "The value for Latitude must be between -90.0 and 90.0.");
				}
				this.latitude = value;
			}
		}
		/// <summary>
		/// Gets or sets the longitude.
		/// </summary>
		/// <value>The longitude.</value>
		public double Longitude {
			get { return this.longitude; }
			set {
				if(value < -180.0 || value > 180.0) {
					throw new ArgumentOutOfRangeException("Longitude", value, "The value for Longitude must be between -180.0 and 180.0.");
				}
				this.longitude = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:OSMDataPrimitives.OSMNode"/> class.
		/// </summary>
		/// <param name="id">Identifier.</param>
		public OsmNode(ulong id) : base(id)
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:OSMDataPrimitives.OSMNode"/> class.
		/// </summary>
		/// <param name="id">Identifier.</param>
		/// <param name="latitude">Latitude.</param>
		/// <param name="longitude">Longitude.</param>
		public OsmNode(ulong id, double latitude, double longitude) : base(id)
		{
			this.Latitude = latitude;
			this.Longitude = longitude;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj)) {
				return true;
			}
			if(obj == null || GetType() != obj.GetType()) {
				return false;
			}
			var other = (OsmNode)obj;

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
				this.Changeset == other.Changeset &&
				this.Latitude == other.Latitude &&
				this.Longitude == other.Longitude
			);
		}

		public override int GetHashCode()
		{
			unchecked {
				return base.GetHashCode() ^ this.Latitude.GetHashCode() ^ this.Longitude.GetHashCode();
			}
		}
	}
}
