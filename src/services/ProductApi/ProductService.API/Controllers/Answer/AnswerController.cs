using System.Net;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.CQRS.Answer.Commands;
using ProductService.Application.CQRS.Answer.Queries;
using ProductService.Application.DTOs.Answer;

namespace ProductService.API.Controllers.Answer
{
    public class AnswerController(ILogger<AnswerController> logger) : ApiControllerBase
    {
        private readonly ILogger<AnswerController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        /// <summary>
        /// Creates a new answer for a specific question.
        /// </summary>
        /// <param name="questionId">The ID of the question to answer.</param>
        /// <param name="request">The request DTO containing answer details.</param>
        /// <returns>The created answer.</returns>
        /// <response code="201">Returns the newly created answer.</response>
        /// <response code="400">If the request is invalid or the question ID is not provided.</response>
        /// <response code="404">If the specified question is not found.</response>
        [HttpPost]
        [ProducesResponseType(typeof(AnswerDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> CreateAnswer(string questionId, [FromBody] AnswerDto request)
        {
            if (string.IsNullOrEmpty(questionId))
            {
                _logger.LogWarning("Question ID not provided in CreateAnswer request.");
                return BadRequest(new ProblemDetails { Title = "Question ID is required." });
            }

            _logger.LogInformation("Attempting to create answer for question ID: {QuestionId}", questionId);
            var command = new CreateAnswerCommand
            {
                QuestionId = questionId,
                AnswerText = request.AnswerText
                // Add other properties from request as needed
            };
            var result = await Mediator.Send(command);

            // Assuming your command handler returns the created AnswerDto or its Id
            // And you have a GetAnswerByIdQuery to fetch it
            // For simplicity, returning the result directly if it's AnswerDto
            // Or use CreatedAtAction if your command returns the ID
            _logger.LogInformation("Successfully created answer with ID: {AnswerId} for question ID: {QuestionId}", result, questionId);
            return CreatedAtAction(nameof(GetAnswerById), new { questionId = questionId, answerId = result }, result);
        }

        /// <summary>
        /// Retrieves a specific answer by its ID for a given question.
        /// </summary>
        /// <param name="questionId">The ID of the question.</param>
        /// <param name="answerId">The ID of the answer to retrieve.</param>
        /// <returns>The requested answer.</returns>
        /// <response code="200">Returns the requested answer.</response>
        /// <response code="404">If the question or answer is not found.</response>
        [HttpGet("{answerId}")]
        [ProducesResponseType(typeof(AnswerDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAnswerById(string questionId, string answerId)
        {
            _logger.LogInformation("Attempting to retrieve answer with ID: {AnswerId} for question ID: {QuestionId}", answerId, questionId);
            var query = new GetAnswerByIdQuery { QuestionId = questionId, AnswerId = answerId };
            var answer = await Mediator.Send(query);

            if (answer == null)
            {
                _logger.LogWarning("Answer with ID: {AnswerId} for question ID: {QuestionId} not found.", answerId, questionId);
                return NotFound(new ProblemDetails { Title = "Answer not found." });
            }
            _logger.LogInformation("Successfully retrieved answer with ID: {AnswerId}", answerId);
            return Ok(answer);
        }


        /// <summary>
        /// Retrieves all answers for a specific question.
        /// </summary>
        /// <param name="questionId">The ID of the question.</param>
        /// <returns>A list of answers for the specified question.</returns>
        /// <response code="200">Returns a list of answers.</response>
        /// <response code="404">If the question is not found.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AnswerDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAnswersByQuestionId(string questionId)
        {
            _logger.LogInformation("Attempting to retrieve all answers for question ID: {QuestionId}", questionId);
            var query = new GetAnswersByQuestionIdQuery { QuestionId = questionId };
            var answers = await Mediator.Send(query);

            // The handler for GetAnswersByQuestionIdQuery should handle the case where the question itself is not found
            // and potentially throw an exception that gets converted to a 404 by an exception middleware.
            // Or, it could return an empty list if the question exists but has no answers.
            _logger.LogInformation("Successfully retrieved {AnswerCount} answers for question ID: {QuestionId}", answers.Count(), questionId);
            return Ok(answers);
        }


        /// <summary>
        /// Updates an existing answer.
        /// </summary>
        /// <param name="questionId">The ID of the question to which the answer belongs.</param>
        /// <param name="answerId">The ID of the answer to update.</param>
        /// <param name="request">The request DTO containing updated answer details.</param>
        /// <returns>No content if successful.</returns>
        /// <response code="204">If the answer was updated successfully.</response>
        /// <response code="400">If the request is invalid or IDs do not match.</response>
        /// <response code="404">If the answer or question is not found.</response>
        [HttpPut("{answerId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateAnswer(string questionId, string answerId, [FromBody] AnswerDto request)
        {
            if (answerId != request.Id) // Assuming UpdateAnswerRequestDto has an Id property
            {
                _logger.LogWarning("Mismatched Answer ID in URL ({UrlAnswerId}) and body ({BodyAnswerId}) for UpdateAnswer request.", answerId, request.Id);
                return BadRequest(new ProblemDetails { Title = "Mismatched answer ID in URL and body." });
            }
            if (string.IsNullOrEmpty(questionId))
            {
                _logger.LogWarning("Question ID not provided in UpdateAnswer request for answer ID: {AnswerId}", answerId);
                return BadRequest(new ProblemDetails { Title = "Question ID is required." });
            }

            _logger.LogInformation("Attempting to update answer with ID: {AnswerId} for question ID: {QuestionId}", answerId, questionId);
            var command = new UpdateAnswerCommand
            {
                Id = answerId,
                QuestionId = questionId, // Important for context, though the answer itself knows its questionId
                AnswerText = request.AnswerText,
                // Include other updatable fields from UpdateAnswerRequestDto
            };
            await Mediator.Send(command);
            _logger.LogInformation("Successfully updated answer with ID: {AnswerId}", answerId);
            return NoContent();
        }


        /// <summary>
        /// Deletes a specific answer.
        /// </summary>
        /// <param name="questionId">The ID of the question to which the answer belongs.</param>
        /// <param name="answerId">The ID of the answer to delete.</param>
        /// <returns>No content if successful.</returns>
        /// <response code="204">If the answer was deleted successfully.</response>
        /// <response code="404">If the answer or question is not found.</response>
        [HttpDelete("{answerId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteAnswer(string questionId, string answerId)
        {
            _logger.LogInformation("Attempting to delete answer with ID: {AnswerId} for question ID: {QuestionId}", answerId, questionId);
            var command = new DeleteAnswerCommand { AnswerId = answerId, QuestionId = questionId }; // Pass QuestionId for context if needed by handler
            await Mediator.Send(command);
            _logger.LogInformation("Successfully deleted answer with ID: {AnswerId}", answerId);
            return NoContent();
        }



        /// <summary>
        /// Approves an answer. (Admin action)
        /// </summary>
        /// <param name="questionId">The ID of the question to which the answer belongs.</param>
        /// <param name="answerId">The ID of the answer to approve.</param>
        /// <returns>No content if successful.</returns>
        /// <response code="204">If the answer was approved successfully.</response>
        /// <response code="404">If the answer or question is not found.</response>
        /// <response code="403">If the user is not authorized to approve answers.</response>
        [HttpPatch("{answerId}/approve")] // Using PATCH as it's a partial update of the 'IsApproved' status
        // [Authorize(Roles = "Admin")] // Example authorization
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        public async Task<IActionResult> ApproveAnswer(string questionId, string answerId)
        {
            _logger.LogInformation("Attempting to approve answer with ID: {AnswerId} for question ID: {QuestionId}", answerId, questionId);
            var command = new ApproveAnswerCommand { AnswerId = answerId, QuestionId = questionId };
            await Mediator.Send(command);
            _logger.LogInformation("Successfully approved answer with ID: {AnswerId}", answerId);
            return NoContent();
        }
    }
}
