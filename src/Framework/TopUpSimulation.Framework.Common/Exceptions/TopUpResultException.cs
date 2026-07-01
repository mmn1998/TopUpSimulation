namespace TopUpSimulation.Framework.Common.Exceptions;
public record TopUpResultException(string code, string? message = "")
{
    public static TopUpResultException None = new("200");
    public static TopUpResultException ServerError = new("500");
}