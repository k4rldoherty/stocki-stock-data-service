using System.Net;
using System.Text.Json;
using StockDataService.Models;

namespace StockDataService.Services;

public class ApiService(HttpClient httpClient, IConfiguration config)
{
    private readonly string? _fhApi = config["FINNHUB_API_KEY"];
    private readonly string? _alphaApi = config["ALPHA_API_KEY"];
    public async Task<ApiResponse> GetStockData(string symbol)
    {

        if (_fhApi is null || _alphaApi is null) return new ApiResponse(500, "Cannot find API Keys", null);
        symbol = symbol.ToUpper();
        var url = $"https://www.finnhub.io/api/v1/quote?token={_fhApi}&symbol={symbol}";
        var response = await httpClient.GetAsync(url);
        if (response.StatusCode != HttpStatusCode.OK)
            return new ApiResponse(((int)response.StatusCode), $"Error retrieving data for ticker {symbol}", null);
        var responseStr = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(responseStr);
        if (!jsonDoc.RootElement.TryGetProperty("c", out var currPrice))
        {
            return new ApiResponse(404, "Cannot get stock data for this ticker", null);
        }
        if (decimal.Parse(currPrice.ToString()).Equals(0))
        {
            return new ApiResponse(404, "Cannot get stock data for this ticker", null);
        }
        return new ApiResponse(((int)response.StatusCode), $"{DateTime.Now}", jsonDoc.RootElement.Clone());
    }

    public async Task<ApiResponse> GetStockInfoAsync(string symbol)
    {
        var url =
            $"https://www.alphavantage.co/query?function=OVERVIEW&symbol={symbol}&apikey={_alphaApi}";
        var response = await httpClient.GetAsync(url);
        if (response.StatusCode != HttpStatusCode.OK)
            return new ApiResponse(
                (int)response.StatusCode, 
                "Error retrieving data for this ticker",
                null
            );
        var responseStr = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(responseStr);
        if (jsonDoc.RootElement.TryGetProperty("Information", out _))
            return new ApiResponse(
                500,
                "My owner is too cheap to pay for the premium API.. I'm going to sleep until tomorrow.",
                null
            );
        if (!jsonDoc.RootElement.TryGetProperty("Symbol", out _))
        {
            return new ApiResponse((int)response.StatusCode,"Symbol not found", null);
        }
        return new ApiResponse((int)response.StatusCode, "Request Successful", jsonDoc.RootElement.Clone());
    }
}