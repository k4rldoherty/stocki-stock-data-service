using System.Text.Json;

namespace StockDataService.Models;

public class ApiResponse(int statusCode, string message, JsonElement? data)
{
    public int StatusCode { get; } = statusCode;
    public string Message { get; } = message;
    public JsonElement? Data { get; } = data;
}
