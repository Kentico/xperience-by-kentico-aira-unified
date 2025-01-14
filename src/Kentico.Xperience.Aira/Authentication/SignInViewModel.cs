using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Kentico.Xperience.Aira.Authentication;

public class SignInViewModel
{
    [Required(ErrorMessage = "Please enter your user name or email address")]
    [DisplayName("User name or email")]
    [MaxLength(100, ErrorMessage = "Maximum allowed length of the input text is {1}")]
    public string UserNameOrEmail { get; set; } = "";

    [DataType(DataType.Password)]
    [DisplayName("Password")]
    [MaxLength(100, ErrorMessage = "Maximum allowed length of the input text is {1}")]
    public string Password { get; set; } = "";
}
