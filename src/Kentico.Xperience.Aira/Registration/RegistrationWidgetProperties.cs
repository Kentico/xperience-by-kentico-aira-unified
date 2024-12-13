using Kentico.PageBuilder.Web.Mvc;

namespace Kentico.Xperience.Aira.Registration;

public class RegistrationWidgetProperties : IWidgetProperties
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public RegistrationViewModel Model { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
}
