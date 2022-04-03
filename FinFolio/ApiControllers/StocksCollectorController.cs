using FinFolio.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FinFolio.Web.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
    public class StocksCollectorController : ControllerBase
    {
        private readonly IStockService _stockService;
        private readonly ILogger<GoldCollectorController> _logger;

        public StocksCollectorController(IStockService stockService, ILogger<GoldCollectorController> logger)
        {
            _stockService = stockService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<string>> GetRemoteStock(string ticker)
        {
            return await _stockService.LoadRemoteStockInfo(ticker);
        }
    }
}
