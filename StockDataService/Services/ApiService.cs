using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using StockDataService.Models;

namespace StockDataService.Services;

public class ApiService(HttpClient httpClient, IConfiguration config, IMemoryCache cache)
{
    private readonly string? _fhApi = config["FINNHUB_API_KEY"] ?? throw new NullReferenceException("Cannot retrieve Api Key");
    private readonly string? _alphaApi = config["ALPHA_API_KEY"] ?? throw new NullReferenceException("Cannot retrieve Api Key");
    public async Task<ApiResponse> GetStockData(string symbol)
    {
        symbol = symbol.ToUpper();
        var url = $"https://www.finnhub.io/api/v1/quote?token={_fhApi}&symbol={symbol}";
        if (cache.TryGetValue(url, out JsonElement? cacheEntry))
        {
            return new ApiResponse(200, "Retrieved from cache", cacheEntry);
        }
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
        cache.Set(url, jsonDoc.RootElement.Clone(), DateTimeOffset.Now.AddMinutes(5));
        return new ApiResponse(((int)response.StatusCode), $"{DateTime.Now}", jsonDoc.RootElement.Clone());
    }

    public async Task<ApiResponse> GetStockInfoAsync(string symbol)
    {
        var url =
            $"https://www.alphavantage.co/query?function=OVERVIEW&symbol={symbol}&apikey={_alphaApi}";
        symbol = symbol.ToUpper();
        if (cache.TryGetValue(url, out JsonElement? cacheEntry))
        {
            return new ApiResponse(200, "Retrieved from cache", cacheEntry);
        }
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
        cache.Set(url, jsonDoc.RootElement.Clone(), DateTimeOffset.Now.AddDays(1));
        return new ApiResponse((int)response.StatusCode, "Request Successful", jsonDoc.RootElement.Clone());
    }

    public async Task<ApiResponse> GetStockNewsAsync(string symbol)
    {
        symbol = symbol.ToUpper();
        var url =
            $"https://www.finnhub.io/api/v1/company-news?token={_fhApi}&symbol={symbol}&from={DateTime.Now.AddDays(-7):yyyy-MM-dd}&to={DateTime.Now:yyyy-MM-dd}";
        if (cache.TryGetValue(url, out JsonElement? cacheEntry))
        {
            return new ApiResponse(200, "Retrieved from cache", cacheEntry);
        }
        var response = await httpClient.GetAsync(url);
        if (response.StatusCode != HttpStatusCode.OK)
            return new ApiResponse(
                (int)response.StatusCode,
                "Something went wrong retrieving the data",
                null
            );
        var responseStr = await response.Content.ReadAsStringAsync();
        if (responseStr == "[]")
            return new ApiResponse(
                (int)response.StatusCode,
                $"Ticker symbol incorrect, please check your input and try again",
                null
            );
        var jsonDoc = JsonDocument.Parse(responseStr);
        cache.Set(url, jsonDoc.RootElement.Clone(), DateTimeOffset.Now.AddHours(3));
        return new ApiResponse((int)response.StatusCode, "Company News",jsonDoc.RootElement.Clone()); 
    }
}