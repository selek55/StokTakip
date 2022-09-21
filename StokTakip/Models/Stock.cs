using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace StokTakip.Models
{
    public class Stock
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; }

        public bool Status { get; set; }

        public decimal? BuyingPrice { get; set; }

        public decimal? SellingPrice { get; set; }

        public decimal? Quantity { get; set; }

        [NotMapped]
        public IFormFile? Image  { get; set; }

        public Image? ImageData { get; set; }
    }

    public class Image 
    {
        public byte[] Data { get; set; }

        public string ContentType { get; set; }

        public long Length { get; set; }

        public string FileName { get; set; }

    }
}
