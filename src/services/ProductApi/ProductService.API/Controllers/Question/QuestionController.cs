using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductService.API.Controllers.Product;

namespace ProductService.API.Controllers.Question
{

    public class QuestionController : ApiControllerBase
    {
        private readonly ILogger<QuestionController> _logger;

        public QuestionController(ILogger<QuestionController> logger)
        {
            _logger = logger;
        }
    }
}
