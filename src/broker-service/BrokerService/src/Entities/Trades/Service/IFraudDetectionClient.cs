public interface IFraudDetectionClient
{
    Task<float> ScoreTradeAsync(
        int accountId,
        int instrumentId,
        bool success,
        double quantity,
        double price,
        double totalValue,
        int hour,
        int day,
        bool isBuy
    );
}