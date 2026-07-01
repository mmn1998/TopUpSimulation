using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TopUpSimulation.Framework.Common.Exceptions;
using TopUpSimulation.Framework.Common.RestfulClient;
using TopUpSimulation.Framework.Common.Settings;
using TopUpSimulation.Handlers.Contracts.TopUp;
using TopUpSimulation.Handlers.Contracts.TopUp.Requests;
using TopUpSimulation.Handlers.Contracts.TopUp.Responses;

namespace TopUpSimulation.ExternalServices.TopUp;

public class MCITopUpService : ITopUpService
{
    private readonly IRestfulClient _restfulClient;
    private readonly ILogger<MCITopUpService> _logger;
    private readonly MCITopUpSetting _mCITopUpSetting;

    public MCITopUpService(IRestfulClient restfulClient,
        IOptions<MCITopUpSetting> mCITopUpSettingoption,
        ILogger<MCITopUpService> logger)
    {
        _restfulClient = restfulClient;
        _logger = logger;
        _mCITopUpSetting = mCITopUpSettingoption.Value ?? throw TopUpResultException.ConfigureErrorrror;
    }
    public async Task<InstantChargeResponse> InstantCharge(InstantChargeRequest request)
    {
        var url = $"{_mCITopUpSetting.BaseUrl}{_mCITopUpSetting.InstantCharge}";
        var headers = GetDefaultHeaders();
        return await _restfulClient.PostAsync<InstantChargeResponse, InstantChargeRequest>(actionUrl: url, request: request, headers: headers);
    }
    #region private methods
    private Dictionary<string, string> GetDefaultHeaders()
    {
        var headers = new Dictionary<string, string>();
        headers.Add("token", _mCITopUpSetting.SecretToken);
        return headers;
    }
    #endregion
}
