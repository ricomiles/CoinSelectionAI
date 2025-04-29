
namespace CardanoCoinSelection.Models.API;

public class PredictionRequest
{
    public List<UtxoData> AvailableUtxos { get; set; } = [];
    public List<AmountData> RequestedAmounts { get; set; } = [];
    public int? MaxInputs { get; set; }
}

public class UtxoData
{
    public string TransactionId { get; set; } = string.Empty;
    public ulong Index { get; set; }
    public string Address { get; set; } = string.Empty;
    public ulong Lovelace { get; set; }
    public List<TokenData> Tokens { get; set; } = [];
}

public class TokenData
{
    public string PolicyId { get; set; } = string.Empty; // Hex-encoded byte[]
    public string AssetName { get; set; } = string.Empty; // Hex-encoded byte[]
    public ulong Quantity { get; set; }
}

public class AmountData
{
    public ulong Lovelace { get; set; }
    public List<TokenData> Tokens { get; set; } = [];
}
