using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductService.API.Controllers.Product;

namespace ProductService.API.Controllers.Media
{

    public class MediaController : ApiControllerBase
    {
        private readonly ILogger<MediaController> _logger;

        public MediaController(ILogger<MediaController> logger)
        {
            _logger = logger;
        }
    }
}
