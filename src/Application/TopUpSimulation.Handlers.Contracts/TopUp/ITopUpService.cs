using TopUpSimulation.Handlers.Contracts.TopUp.Requests;
using TopUpSimulation.Handlers.Contracts.TopUp.Responses;

namespace TopUpSimulation.Handlers.Contracts.TopUp;

public interface ITopUpService
{
    Task<InstantChargeResponse> InstantCharge(InstantChargeRequest request);
}
