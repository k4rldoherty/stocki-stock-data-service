using Microsoft.AspNetCore.Mvc;
using StockDataService.Services;

namespace StockDataService.Controllers;

[ApiController]
[Route("api/")]
public class StockDataController(ApiService apiService) : ControllerBase
{
    [HttpGet("get-stock-price-data/{ticker}")]
    public async Task<IActionResult> GetStockPriceDataAsync(string ticker)
    {
        var res = await apiService.GetStockData(ticker);
        return res.Data is null
            ? StatusCode((int)res.StatusCode, res.Message)
            : Ok(res);
    }

    [HttpGet("get-stock-info/{ticker}")]
    public async Task<IActionResult> GetStockInfoAsync(string ticker)
    {
        var res = await apiService.GetStockInfoAsync(ticker);
        return res.Data is null
            ? StatusCode((int)res.StatusCode, res.Message)
            : Ok(res);
    }

    [HttpGet("get-stock-news/{ticker}")]
    public async Task<IActionResult> GetStockNewsAsync(string ticker)
    {
        var res = await apiService.GetStockNewsAsync(ticker);
        return res.Data is null
            ? StatusCode((int)res.StatusCode, res.Message)
            : Ok(res);
    }

    [HttpGet("check-ticker/{ticker}")]
    public async Task<IActionResult> CheckTickerAsync(string ticker)
    {
        var res = await apiService.CheckTickerAsync(ticker);
        return res.Data is null
            ? StatusCode((int)res.StatusCode, res.Message)
            : Ok(res);
    }
}