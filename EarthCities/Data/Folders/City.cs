using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EarthCities.Data.Folders
{
	[Table("Cities")]
	public class City
	{
		[Key]
		[Required]
		public int Id { get; set; }
		public string Name { get; set; }
		public string Name_ASCII { get; set; }
		[Column(TypeName = "decimal(7,4)")]
		public decimal Latitude { get; set; }
		[Column(TypeName = "decimal(7,4)")]
		public decimal Longitude { get; set; }
		[ForeignKey(nameof(Country))]
		public int CountryId { get; set; }
		public virtual Country Country { get; set; } // parent
	}
}
