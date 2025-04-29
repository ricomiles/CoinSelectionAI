using CardanoCoinSelection.Models.API;
using CardanoCoinSelection.Models.ML;
using CardanoCoinSelection.Services;
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
            _logger.LogInformation("Received prediction request with {UtxoCount} UTXOs",
                request.AvailableUtxos.Count);

            // Convert request to model input
            var input = new CoinSelectionInput
            {
                AvailableUtxos = ConvertToResolvedInputs(request.AvailableUtxos),
                RequestedAmount = ConvertToValues(request.RequestedAmounts),
                MaxInputs = request.MaxInputs ?? int.MaxValue
            };

            // Get prediction from the model
            var startTime = DateTime.UtcNow;
            var output = _model.Predict(input);
            var executionTime = (DateTime.UtcNow - startTime).TotalMilliseconds;

            // Create response with AI-like metadata
            var response = new PredictionResponse
            {
                SelectedUtxos = ConvertFromResolvedInputs(output.SelectedUtxos),
                LovelaceChange = output.LovelaceChange,
                AssetsChange = output.AssetsChange,
                Metadata = new PredictionMetadata
                {
                    ModelVersion = "cardano-selection-v1.2",
                    ConfidenceScore = output.ConfidenceScore,
                    OptimalityScore = output.OptimalityScore,
                    ExecutionTimeMs = executionTime,
                    FeatureImportances = output.FeatureImportances,
                    PredictionId = Guid.NewGuid().ToString()
                }
            };

            _logger.LogInformation("Prediction completed in {ExecutionTime}ms", executionTime);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing prediction request");
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
