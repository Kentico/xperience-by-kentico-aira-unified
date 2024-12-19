namespace Kentico.Xperience.Aira.Registration
{
    public class EmailConfirmationViewModel
    {
        public EmailConfirmationState State { get; set; } = EmailConfirmationState.Failure_NotYetConfirmed;
        public string Message { get; set; } = "";
        public string SendButtonText { get; set; } = "";
        public string Username { get; set; } = "";
    }
}
