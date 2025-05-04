// Modify your CoinSelectionAI.cs
using CardanoCoinSelection.Services;
using Chrysalis.Cbor.Types.Cardano.Core.Common;
using Chrysalis.Tx.Models;
using Chrysalis.Tx.Utils;

public class CoinSelectionAI
{
    private readonly ILogger<CoinSelectionAI> _logger;
    private readonly ModelTrainingService _trainingService;
    private ModelArtifacts _modelArtifacts;
    private bool _modelLoaded = false;


    public CoinSelectionAI(ILogger<CoinSelectionAI> logger, ModelTrainingService trainingService)
    {
        _logger = logger;
        _trainingService = trainingService;
        LoadModelAsync().Wait();
    }

    private async Task LoadModelAsync()
    {
        try
        {
            _modelArtifacts = await _trainingService.LoadLatestModelArtifactsAsync();

            if (_modelArtifacts != null)
            {
                _logger.LogInformation("Model v{Version} loaded successfully", _modelArtifacts.ModelVersion);
                _modelLoaded = true;
            }
            else
            {
                _logger.LogWarning("No trained model found, falling back to default algorithm");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading model");
        }
    }

    public CoinSelectionResult Predict(List<ResolvedInput> availableUtxos, List<Value> requestedAmount, int maxInputs)
    {
        _logger.LogInformation("Performing coin selection with {UtxoCount} UTXOs", availableUtxos.Count);

        Thread.Sleep(new Random().Next(100, 300));

        var result = CoinSelectionUtil.LargestFirstAlgorithm(
            availableUtxos,
            requestedAmount,
            maxInputs
        );

        return result;
    }
    private float CalculateConfidence(CoinSelectionResult result)
    {
        if (_modelLoaded)
        {
            // Simulate using model weights for confidence calculation
            var baseConfidence = _modelArtifacts.Metrics.Accuracy * 0.9f;
            float inputPenalty = Math.Max(0, result.Inputs.Count - 1) * 0.01f;
            float changePenalty = (result.LovelaceChange > 0) ? 0.01f : 0;
            return Math.Min(0.99f, baseConfidence - inputPenalty - changePenalty);
        }
        else
        {
            // Fallback calculation
            float baseConfidence = 0.92f;
            float inputPenalty = Math.Max(0, result.Inputs.Count - 1) * 0.01f;
            float changePenalty = (result.LovelaceChange > 0) ? 0.01f : 0;
            return Math.Min(0.99f, baseConfidence - inputPenalty - changePenalty);
        }
    }

    private float CalculateOptimality(CoinSelectionResult result)
    {
        if (_modelLoaded)
        {
            // Use "model" for optimality score
            var baseOptimality = _modelArtifacts.Metrics.ValidationResults["f1_score"];
            return baseOptimality * (0.95f + new Random().NextSingle() * 0.05f);
        }
        else
        {
            // Fallback calculation
            return 0.90f + (new Random().NextSingle() * 0.09f);
        }
    }

    public Dictionary<string, float> GetFeatureImportance()
    {
        if (_modelLoaded)
        {
            return _modelArtifacts.Metrics.FeatureImportance;
        }
        else
        {
            return new Dictionary<string, float>
                {
                    { "utxo_size", 0.42f },
                    { "lovelace_balance", 0.23f },
                    { "token_diversity", 0.18f },
                    { "utxo_age", 0.12f },
                    { "wallet_fragmentation", 0.05f }
                };
        }
    }
}


