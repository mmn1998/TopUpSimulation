namespace TopUpSimulation.Framework.Common.Exceptions;
public record TopUpResultException(string code, string? message = "")
{
    public static TopUpResultException None = new("200");
    public static TopUpResultException ServerError = new("500");
    public static TopUpResultException ConfigureError = new("500", "check appSettings file you may forget something dude");
    public static TopUpResultException DeserializeError = new("500", "Event was sended in a wrong format");
    public static TopUpResultException TopUpChargeError = new("500", "MCI service was not available");
    public static TopUpResultException BrokerError = new("500", "Message broker is not available");
    public static implicit operator TopUpException(TopUpResultException exception)
    {
        return new TopUpException(exception);
    }
}