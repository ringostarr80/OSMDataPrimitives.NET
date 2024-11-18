using System;

namespace OSMDataPrimitives
{
	/// <summary>
	/// OsmNode.
	/// </summary>
	public class OsmNode : OsmElement
	{
		private double _latitude;
		private double _longitude;

		/// <summary>
		/// Gets or sets the latitude.
		/// </summary>
		/// <value>The latitude.</value>
		/// <exception cref="T:System.ArgumentOutOfRangeException"></exception>
		public double Latitude
		{
			get => this._latitude;
			set
			{
				if (value is < -90.0 or > 90.0)
				{
					throw new ArgumentOutOfRangeException(nameof(Latitude), value,
						"The value for Latitude must be between -90.0 and 90.0.");
				}

				this._latitude = value;
			}
		}

		/// <summary>
		/// Gets or sets the longitude.
		/// </summary>
		/// <value>The longitude.</value>
		public double Longitude
		{
			get => this._longitude;
			set
			{
				if (value is < -180.0 or > 180.0)
				{
					throw new ArgumentOutOfRangeException(nameof(Longitude), value,
						"The value for Longitude must be between -180.0 and 180.0.");
				}

				this._longitude = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:OSMDataPrimitives.OsmNode"/> class.
		/// </summary>
		/// <param name="id">Identifier.</param>
		public OsmNode(ulong id) : base(id)
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:OSMDataPrimitives.OsmNode"/> class.
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
			if (ReferenceEquals(this, obj))
			{
				return true;
			}

			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			var other = (OsmNode)obj;

			if (this.Tags == null && other.Tags != null)
			{
				return false;
			}

			if (this.Tags != null && other.Tags == null)
			{
				return false;
			}

			if (this.Tags is not null && other.Tags is not null)
			{
				if (this.Tags.Count != other.Tags.Count)
				{
					return false;
				}

				foreach (var kvp in this.Tags)
				{
					if (!other.Tags.TryGetValue(kvp.Key, out string valueInDict2))
					{
						return false;
					}

					if (kvp.Value != valueInDict2)
					{
						return false;
					}
				}
			}

			return (
				this.Id == other.Id &&
				this.UserId == other.UserId &&
				this.UserName == other.UserName &&
				this.Version == other.Version &&
				this.Timestamp == other.Timestamp &&
				this.Changeset == other.Changeset &&
				Math.Abs(this.Latitude - other.Latitude) < double.Epsilon &&
				Math.Abs(this.Longitude - other.Longitude) < double.Epsilon
			);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ this.Latitude.GetHashCode() ^ this.Longitude.GetHashCode();
		}
	}
}
