using important1.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace important1.Models
{
    public partial class Order
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Mã đơn hàng")]
        public int OrderId { get; set; }
        public int Total { get; set; }
        [Display(Name = "Ngày mua hàng")]
        public DateTime? OrderDate { get; set; }
        [Display(Name = "Người mua")]
        public string? Id { get; set; }

        public virtual ICollection<OrderDetail>? OrderDetails { get; set; }
        public virtual important1User? IdNavigation { get; set; }
    }
}
