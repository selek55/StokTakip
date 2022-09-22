using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace StokTakip.Models
{
    public class Stock
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Code { get; set; }

        public string Name { get; set; }

        public bool Status { get; set; }

        public int? BuyingPrice { get; set; }

        public int? SellingPrice { get; set; }

        public int? Quantity { get; set; }

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
