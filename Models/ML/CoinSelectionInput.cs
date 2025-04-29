using Chrysalis.Cbor.Types.Cardano.Core.Common;
using Chrysalis.Tx.Models;
using Microsoft.ML.Data;

namespace CardanoCoinSelection.Models.ML;

    public class CoinSelectionInput
{
    [VectorType(1)]
    public List<ResolvedInput> AvailableUtxos { get; set; } = [];

    [VectorType(1)]
    public List<Value> RequestedAmount { get; set; } = [];

    public int MaxInputs { get; set; } = int.MaxValue;
}
