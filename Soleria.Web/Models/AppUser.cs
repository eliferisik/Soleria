using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Soleria.Web.Models;

public class AppUser : IdentityUser
{
    [MaxLength(60)]
    public string Ad { get; set; } = "";

    [MaxLength(60)]
    public string Soyad { get; set; } = "";
}
