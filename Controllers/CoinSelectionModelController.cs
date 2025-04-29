using CardanoCoinSelection.Models.API;
using Chrysalis.Cbor.Extensions.Cardano.Core.Common;
using Chrysalis.Cbor.Extensions.Cardano.Core.Transaction;
using Chrysalis.Cbor.Types.Cardano.Core.Common;
using Chrysalis.Cbor.Types.Cardano.Core.Transaction;
using Chrysalis.Tx.Models;
using Microsoft.AspNetCore.Mvc;
using WalletAddresses = Chrysalis.Wallet.Models.Addresses;


namespace CardanoCoinSelectionAI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoinSelectionModelController : ControllerBase
{
    private readonly CoinSelectionAI _model;
    private readonly ILogger<CoinSelectionModelController> _logger;

    public CoinSelectionModelController(CoinSelectionAI model, ILogger<CoinSelectionModelController> logger)
    {
        _model = model;
        _logger = logger;
    }

    [HttpPost("predict")]
    public ActionResult<PredictionResponse> Predict(PredictionRequest request)
    {
        try
        {
            // Convert request to domain types
            var availableUtxos = ConvertToResolvedInputs(request.AvailableUtxos);
            var requestedAmount = ConvertToValues(request.RequestedAmounts);
            var maxInputs = request.MaxInputs ?? int.MaxValue;

            // Call AI service directly
            var startTime = DateTime.UtcNow;
            var result = _model.Predict(availableUtxos, requestedAmount, maxInputs);
            var executionTime = (DateTime.UtcNow - startTime).TotalMilliseconds;

            // Create response with AI-like metadata
            var response = new PredictionResponse
            {
                SelectedUtxos = ConvertFromResolvedInputs(result.Inputs),
                LovelaceChange = result.LovelaceChange,
                AssetsChange = [],
                Metadata = new PredictionMetadata
                {
                    ModelVersion = "cardano-selection-v1.2",
                    ConfidenceScore = 0,
                    OptimalityScore = 0,
                    ExecutionTimeMs = executionTime,
                    FeatureImportances = [],
                    PredictionId = Guid.NewGuid().ToString()
                }
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // Conversion methods - you'll replace these with conversions to your actual types
    private List<ResolvedInput> ConvertToResolvedInputs(List<UtxoData> utxos)
    {
        // This is a placeholder implementation
        // You'll replace with your actual conversion to your ResolvedInput type
        return [.. utxos.Select(u =>
        {
            var txId = Convert.FromHexString(u.TransactionId);
            var input = new TransactionInput(txId, u.Index);

            var output = new AlonzoTransactionOutput(
                new Address(new WalletAddresses.Address(u.Address).ToBytes()),
                new Lovelace(u.Lovelace),
                null
            );

            return new ResolvedInput(input, output);
        })];
    }

    private List<Value> ConvertToValues(List<AmountData> amounts)
    {
        return [.. amounts.Select(a => (Value)new Lovelace(a.Lovelace))];
    }

    private List<UtxoData> ConvertFromResolvedInputs(List<ResolvedInput> inputs)
    {
        // Placeholder implementation - you'll replace with your actual conversion
        return [.. inputs.Select(i => new UtxoData
        {
            TransactionId = Convert.ToHexString(i.Outref.TransactionId),
            Index = i.Outref.Index,
            Lovelace = i.Output.Amount().Lovelace()
        })];
    }

}
