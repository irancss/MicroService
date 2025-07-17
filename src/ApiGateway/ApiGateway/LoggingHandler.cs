using System.Diagnostics;

namespace ApiGateway
/// <summary>
/// A <see cref="DelegatingHandler"/> that logs information about HTTP requests and their responses.
/// It records details at the start of processing a request, and upon its completion or failure.
/// </summary>

/// <summary>
/// Initializes a new instance of the <see cref="LoggingHandler"/> class.
/// </summary>
/// <param name="logger">The logger instance to be used for logging request and response information.</param>

/// <summary>
/// Sends an HTTP request to the inner handler to send to the server as an asynchronous operation,
/// while logging details about the request and the outcome (response or exception).
/// </summary>
/// <param name="request">The HTTP request message to send to the server.</param>
/// <param name="cancellationToken">A cancellation token to cancel operation.</param>
/// <returns>A task representing the asynchronous operation, which will yield an <see cref="HttpResponseMessage"/>.</returns>
/// <remarks>
/// This method logs the following information:
/// <list type="bullet">
/// <item><description>Start of the request: HTTP method and URI.</description></item>
/// <item><description>Completion of the request: HTTP method, URI, response status code, and elapsed time in milliseconds.</description></item>
/// <item><description>Failure of the request: HTTP method, URI, and exception details if an error occurs.</description></item>
/// </list>
/// Any exceptions encountered during the request processing are logged and then re-thrown.
/// </remarks>
{
    public class LoggingHandler : DelegatingHandler
    {
        private readonly ILogger<LoggingHandler> _logger;

        public LoggingHandler(ILogger<LoggingHandler> logger)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();

            _logger.LogInformation($"Start: {request.Method} {request.RequestUri}");

            try
            {
                var response = await base.SendAsync(request, cancellationToken);

                _logger.LogInformation($"Completed: {request.Method} {request.RequestUri} " +
                                       $"=> {response.StatusCode} in {stopwatch.ElapsedMilliseconds}ms");

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed: {request.Method} {request.RequestUri}");
                throw;
            }
        }
    }
}
