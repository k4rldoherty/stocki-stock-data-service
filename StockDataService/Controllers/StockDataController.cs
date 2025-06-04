using Microsoft.AspNetCore.Mvc;
using StockDataService.Services;

namespace StockDataService.Controllers;

[ApiController]
[Route("api/v1/")]
public class StockDataController(ApiService apiService) : ControllerBase
{
    [HttpGet("stock-price-data")]
    public async Task<IActionResult> GetStockPriceDataAsync(
        [FromQuery(Name = "ticker")] string ticker
    )
    {
        var res = await apiService.GetStockPriceData(ticker);
        return res.Data is null ? StatusCode((int)res.StatusCode, res.Message) : Ok(res);
    }

    [HttpGet("stock-overview")]
    public async Task<IActionResult> GetStockInfoAsync([FromQuery(Name = "ticker")] string ticker)
    {
        var res = await apiService.GetStockInfoAsync(ticker);
        return res.Data is null ? StatusCode((int)res.StatusCode, res.Message) : Ok(res);
    }

    [HttpGet("stock-news")]
    public async Task<IActionResult> GetStockNewsAsync([FromQuery(Name = "ticker")] string ticker)
    {
        var res = await apiService.GetStockNewsAsync(ticker);
        return res.Data is null ? StatusCode((int)res.StatusCode, res.Message) : Ok(res);
    }

    [HttpGet("check-ticker")]
    public async Task<IActionResult> CheckTickerAsync([FromQuery(Name = "ticker")] string ticker)
    {
        var res = await apiService.CheckTickerAsync(ticker);
        return res.Data is null ? StatusCode((int)res.StatusCode, res.Message) : Ok(res);
    }
}

