using Chrysalis.Cbor.Types.Cardano.Core.Common;
using Chrysalis.Tx.Models;
using Microsoft.ML.Data;

namespace CardanoCoinSelection.Models.ML;

public class CoinSelectionOutput
{
    [VectorType(1)]
    public List<ResolvedInput> SelectedUtxos { get; set; } = [];

    public ulong LovelaceChange { get; set; }

    public Dictionary<byte[], TokenBundleOutput> AssetsChange { get; set; } = [];

    public float ConfidenceScore { get; set; }
    public float OptimalityScore { get; set; }
    public Dictionary<string, float> FeatureImportances { get; set; } = [];
}
