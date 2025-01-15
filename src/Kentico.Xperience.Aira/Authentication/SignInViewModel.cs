using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Kentico.Xperience.Aira.Authentication;

/// <summary>
/// View model for the signin page.
/// </summary>
public class SignInViewModel
{
    /// <summary>
    /// User name or email of the User.
    /// </summary>
    [Required(ErrorMessage = "Please enter your user name or email address")]
    [DisplayName("User name or email")]
    [MaxLength(100, ErrorMessage = "Maximum allowed length of the input text is {1}")]
    public string UserNameOrEmail { get; set; } = "";

    /// <summary>
    /// Password of the User.
    /// </summary>
    [DataType(DataType.Password)]
    [DisplayName("Password")]
    [MaxLength(100, ErrorMessage = "Maximum allowed length of the input text is {1}")]
    public string Password { get; set; } = "";
}
