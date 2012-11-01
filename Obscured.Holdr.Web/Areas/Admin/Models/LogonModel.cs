using System.ComponentModel.DataAnnotations;

namespace Obscured.Holdr.Web.Areas.Admin.Models
{
    public class LogonModel
    {
        [Required(ErrorMessage = "Var god ange användarnamn"), Display(Name = "Användarnamn")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Var god ange lösenord"), Display(Name = "Lösenord")]
        public string Password { get; set; }
    }
}