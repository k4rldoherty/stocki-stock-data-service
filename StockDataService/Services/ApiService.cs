using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using StockDataService.Models;

namespace StockDataService.Services;

public class ApiService(HttpClient httpClient, IMemoryCache cache, IConfiguration config)
{
    private readonly string? _fhApi =
        Environment.GetEnvironmentVariable("FHAPI")
        ?? throw new NullReferenceException("Cannot retrieve fh api key");
    private readonly string? _alphaApi =
        Environment.GetEnvironmentVariable("ALPHAAPI")
        ?? throw new NullReferenceException("Cannot retrieve alpha api key");
    private readonly string? _fhBaseUrl =
        config["FinnhubBaseURL"]
        ?? throw new NullReferenceException("Cannot find finnhub base url");
    private readonly string? _alphaBaseUrl =
        config["AlphaBaseUrl"]
        ?? throw new NullReferenceException("Cannot find alpha vantage base url");

    public async Task<ApiResponse> GetStockPriceData(string symbol)
    {
        symbol = symbol.ToUpper();
        var url = $"{_fhBaseUrl}quote?token={_fhApi}&symbol={symbol}";
        if (cache.TryGetValue(url, out JsonElement? cacheEntry))
        {
            return new ApiResponse(200, "Retrieved from cache", cacheEntry);
        }
        var response = await httpClient.GetAsync(url);
        if (response.StatusCode != HttpStatusCode.OK)
            return new ApiResponse(
                ((int)response.StatusCode),
                $"Error retrieving data for ticker {symbol}",
                null
            );
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
        return new ApiResponse(
            ((int)response.StatusCode),
            $"Stock Data for {symbol} retrieved",
            jsonDoc.RootElement.Clone()
        );
    }

    public async Task<ApiResponse> GetStockInfoAsync(string symbol)
    {
        var url = $"{_alphaBaseUrl}query?function=OVERVIEW&symbol={symbol}&apikey={_alphaApi}";
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
            return new ApiResponse((int)response.StatusCode, "Symbol not found", null);
        }
        cache.Set(url, jsonDoc.RootElement.Clone(), DateTimeOffset.Now.AddDays(1));
        return new ApiResponse(
            (int)response.StatusCode,
            "Request Successful",
            jsonDoc.RootElement.Clone()
        );
    }

    public async Task<ApiResponse> GetStockNewsAsync(string symbol)
    {
        symbol = symbol.ToUpper();
        var url =
            $"{_fhBaseUrl}company-news?token={_fhApi}&symbol={symbol}&from={DateTime.Now.AddDays(-7):yyyy-MM-dd}&to={DateTime.Now:yyyy-MM-dd}";
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
        return new ApiResponse(
            (int)response.StatusCode,
            "Company News",
            jsonDoc.RootElement.Clone()
        );
    }

    public async Task<ApiResponse> CheckTickerAsync(string symbol)
    {
        symbol = symbol.ToUpper();
        var url = $"{_fhBaseUrl}stock/profile2?symbol={symbol}&token={_fhApi}";
        if (cache.TryGetValue(url, out JsonElement? cacheEntry))
        {
            return new ApiResponse(200, "Retrieved from cache", cacheEntry);
        }
        var response = await httpClient.GetAsync(url);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            return new ApiResponse(
                (int)response.StatusCode,
                $"Something went wrong retrieving the data",
                null
            );
        }
        var responseStr = await response.Content.ReadAsStringAsync();
        if (responseStr == "{}")
        {
            return new ApiResponse(
                (int)response.StatusCode,
                $"Ticker Symbol invalid, check input and try again",
                null
            );
        }
        var jsonDoc = JsonDocument.Parse(responseStr);
        cache.Set(url, jsonDoc.RootElement.Clone(), DateTimeOffset.Now.AddDays(1));
        return new ApiResponse(
            (int)response.StatusCode,
            "Ticker is valid",
            jsonDoc.RootElement.Clone()
        );
    }
}
