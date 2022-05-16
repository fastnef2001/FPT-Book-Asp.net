using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace important1.Models
{
    public partial class OrderDetail
    {
        public int OrderId { get; set; }
        public string Isbn { get; set; } = null!;
        [Display(Name = "Số lượng")]
        public int Quantity { get; set; }
        [Display(Name = "Sách")]
        public virtual Book? IsbnNavigation { get; set; }
        public virtual Order? Order { get; set; }
    }
}
