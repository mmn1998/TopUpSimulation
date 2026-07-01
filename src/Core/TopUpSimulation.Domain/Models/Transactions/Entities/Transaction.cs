using TopUpSimulation.Domain.Models.Transactions.Args;
using TopUpSimulation.Framework.Core.Entities;

namespace TopUpSimulation.Domain.Models.Transactions.Entities;

public class Transaction : Entity
{
    private Transaction()
    {

    }
    private Transaction(CreateTopUpArg arg)
    {
        TopUpRequest = arg.request;
        TopUpResponse = arg.response;
        Successful = arg.successfull;
    }
    public static Transaction Create(CreateTopUpArg arg)
    {
        return new Transaction(arg);
    }
    public string TopUpResponse { get; private set; }
    public string TopUpRequest { get; private set; }
    public bool Successful { get; private set; }
}
