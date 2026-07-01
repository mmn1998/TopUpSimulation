namespace TopUpSimulation.Handlers.Contracts.TopUp.Requests;

public record InstantChargeRequest(string mobileNumber, decimal amount);
