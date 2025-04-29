
namespace CardanoCoinSelection.Models.API;

public class PredictionResponse
{
    public List<UtxoData> SelectedUtxos { get; set; } = [];
    public ulong LovelaceChange { get; set; }
    public List<TokenData> AssetsChange { get; set; } = [];
    public PredictionMetadata Metadata { get; set; } = new PredictionMetadata();
}

public class PredictionMetadata
{
    public string ModelVersion { get; set; } = "cardano-selection-v1.0";
    public float ConfidenceScore { get; set; }
    public float OptimalityScore { get; set; }
    public double ExecutionTimeMs { get; set; }
    public Dictionary<string, float> FeatureImportances { get; set; } = [];
    public string PredictionId { get; set; } = string.Empty;
}
