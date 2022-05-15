using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using important1.Models;
using Microsoft.AspNetCore.Identity;

namespace important1.Areas.Identity.Data;

// Add profile data for application users by adding properties to the important1User class
public class important1User : IdentityUser
{
    [PersonalData]
    public string? Address { get; set; }
    [PersonalData]
    public DateTime? DoB { get; set; }
    public ICollection<Book>? Books { get; set; }
    public virtual ICollection<Order>? Orders { get; set; }
    public virtual ICollection<Cart>? Carts { get; set; }
}

