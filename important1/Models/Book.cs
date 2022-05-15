using important1.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace important1.Models
{
    public partial class Book
    {
        [Key]
        public string Isbn { get; set; } = null!;
        [Display(Name = "Tên sách")]
        public string Title { get; set; } = null!;
        [Display(Name = "Miêu tả")]
        public string? Descr { get; set; }
        [Display(Name = "Tác giả")]
        public string? Author { get; set; }
        [Display(Name = "Giá")]
        [Range(1, double.MaxValue, ErrorMessage = "The price must be positive.")]
        public int Price { get; set; }
        public string? Thumb { get; set; }

        [Display(Name = "Số trang")]
        [Range(1, double.MaxValue, ErrorMessage = "The page must be positive.")]
        public int? Page { get; set; }
        [Display(Name = "User name")]
        public string? Id { get; set; }
        [Display(Name = "Danh mục")]
        public int CategoryId { get; set; }

        [Display(Name = "Danh mục")]
        public virtual Category? Category { get; set; }
        [Display(Name = "Seller name")]
        public virtual important1User? IdNavigation { get; set; }
        public virtual ICollection<OrderDetail>? OrderDetails { get; set; }
        public virtual ICollection<Cart>? Carts { get; set; }
    }
}
