namespace AuthBox.Utils.ExtensionMethods;
public static class ExceptionExtensions
{
    public static string GetInnermostMessage(this Exception exception)
    {
        Exception innermostException = exception;

        while (innermostException.InnerException is not null)
        {
            innermostException = innermostException.InnerException;
        }

        return innermostException.Message;
    }
}
