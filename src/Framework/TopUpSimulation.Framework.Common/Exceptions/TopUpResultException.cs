namespace TopUpSimulation.Framework.Common.Exceptions;
public record TopUpResultException(string code, string? message = "")
{
    public static TopUpResultException None = new("200");
    public static TopUpResultException ServerError = new("500");
    public static TopUpResultException ConfigureErrorrror = new("500", "check appSettings file you may forget something dude");
    public static implicit operator TopUpException(TopUpResultException exception)
    {
        return new TopUpException(exception);
    }
}