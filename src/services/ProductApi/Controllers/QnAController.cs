using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ProductApi.Controllers
{
    /// <summary>
    /// Controller for handling product Q&A operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class QnAController : ControllerBase
    {
        private readonly ILogger<QnAController> _logger;
        // In-memory storage for demonstration purposes
        private static readonly Dictionary<int, string> _questions = new();
        private static int _nextId = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="QnAController"/> class.
        /// </summary>
        /// <param name="logger">Logger instance.</param>
        public QnAController(ILogger<QnAController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Simulates answering a product question via GET.
        /// </summary>
        /// <param name="question">The question to ask.</param>
        /// <returns>Simulated answer.</returns>
        [HttpGet("ask")]
        public IActionResult AskGet(string question)
        {
            var answer = $"You asked: {question}. This is a simulated answer about products.";
            return Ok(answer);
        }

        /// <summary>
        /// Simulates answering a product question via POST and stores it.
        /// </summary>
        /// <param name="question">The question to ask.</param>
        /// <returns>Simulated answer and question ID.</returns>
        [HttpPost("ask")]
        public IActionResult AskPost([FromBody] string question)
        {
            var id = _nextId++;
            _questions[id] = question;
            var answer = $"You asked: {question}. This is a simulated answer about products.";
            return Ok(new { id, answer });
        }

        /// <summary>
        /// Updates an existing question.
        /// </summary>
        /// <param name="id">The ID of the question to update.</param>
        /// <param name="question">The new question text.</param>
        /// <returns>Status of the update operation.</returns>
        [HttpPut("edit/{id}")]
        public IActionResult Edit(int id, [FromBody] string question)
        {
            if (!_questions.ContainsKey(id))
            {
                return NotFound($"Question with ID {id} not found.");
            }
            _questions[id] = question;
            return Ok($"Question with ID {id} updated.");
        }

        /// <summary>
        /// Deletes a question by ID.
        /// </summary>
        /// <param name="id">The ID of the question to delete.</param>
        /// <returns>Status of the delete operation.</returns>
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            if (!_questions.ContainsKey(id))
            {
                return NotFound($"Question with ID {id} not found.");
            }
            _questions.Remove(id);
            return Ok($"Question with ID {id} deleted.");
        }
     
    }
}
