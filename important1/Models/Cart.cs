using important1.Areas.Identity.Data;

namespace important1.Models
{
    public class Cart
    {
     
        public string? UId { get; set; }
        public string? BookIsbn { get; set; }
        public int Quantity { get; set; }
        public int TotalPerCart { get; set; }
        public important1User? User { get; set; }
        public Book? Book { get; set; }

    }
}
