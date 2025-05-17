using Dapr.Client;
using Dapr.Workflow;
using OrderApi.Models.Entities;

namespace OrderApi.Services.Activities
{
    public class ReserveInventoryActivity : WorkflowActivity<List<OrderItem>, bool>
    {
        private readonly DaprClient _dapr;
        public ReserveInventoryActivity(DaprClient dapr) => _dapr = dapr;
        public override async Task<bool> RunAsync(WorkflowActivityContext context, List<OrderItem> input)
        {
            var request = _dapr.CreateInvokeMethodRequest(
                HttpMethod.Post,
                "inventory-service",
                "api/inventory/reserve",
                items);

            var response = await _dapr.InvokeMethodAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}
