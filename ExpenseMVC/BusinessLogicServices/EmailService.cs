using SendGrid;
using SendGrid.Helpers.Mail;

namespace ExpenseMVC.BusinessLogicServices;

public class EmailService
{
    private static SendGridClient _client;
    public static EmailAddress _from;
    public static EmailAddress _to;
    public static string _htmlMsg;
    
    public static async Task SendEmail(string apiKey, string from, string to, string message)
    {
        SetUpEmailService( from, to, message);
        var mailMsg = MailHelper.CreateSingleEmail(_from, _to, "Sending with SendGrid is Fun",
            " and easy to do anywhere, even with c#", _htmlMsg);
        var client = new SendGridClient(apiKey);
        var response = await client.SendEmailAsync(mailMsg).ConfigureAwait(false);
    }

    public static void SetUpEmailService( string from, string to, string message)
    {
        // if (_client is not null)
        // {
        //     _client = new SendGridClient(apiKey);
        // }
        _from = new EmailAddress(from, from);
        _to = new EmailAddress(to, to);
        _htmlMsg = $"<strong>{message}</strong>";
    }
}