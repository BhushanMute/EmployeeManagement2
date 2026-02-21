using EmployeeManagement.API.Models;
using EmployeeManagement.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DummyUpiPaymentController : ControllerBase
    {
        private readonly IDummyUpiPaymentService _dummyUpiService;
        private readonly ILogger<DummyUpiPaymentController> _logger;

        public DummyUpiPaymentController(
            IDummyUpiPaymentService dummyUpiService,
            ILogger<DummyUpiPaymentController> logger)
        {
            _dummyUpiService = dummyUpiService;
            _logger = logger;
        }

        /// <summary>
        /// Dummy UPI Payment Form Page
        /// </summary>
        [HttpGet("payment-form")]
        public IActionResult PaymentForm(string orderId, decimal amount, string upiId, string txnId)
        {
            try
            {
                var model = new DummyUpiPaymentViewModel
                {
                    OrderId = orderId,
                    Amount = amount,
                    UpiId = upiId,
                    TransactionId = txnId,
                    Timestamp = DateTime.UtcNow
                };

                return Ok(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in PaymentForm: {ex.Message}");
                return BadRequest(new { message = "Invalid payment details" });
            }
        }

        /// <summary>
        /// Simulate UPI Payment Success
        /// </summary>
        [HttpPost("simulate-success")]
        public async Task<IActionResult> SimulateSuccess(string orderId)
        {
            try
            {
                if (string.IsNullOrEmpty(orderId))
                {
                    return BadRequest(new { message = "OrderId is required" });
                }

                var response = await _dummyUpiService.SimulateUpiPaymentAsync(orderId, success: true);

                if (response.Success)
                {
                    _logger.LogInformation($"Simulated successful payment for OrderId: {orderId}");
                    return Ok(response);
                }

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error simulating success: {ex.Message}");
                return StatusCode(500, new { message = "Error processing payment" });
            }
        }

        /// <summary>
        /// Simulate UPI Payment Failure
        /// </summary>
        [HttpPost("simulate-failure")]
        public async Task<IActionResult> SimulateFailure(string orderId)
        {
            try
            {
                if (string.IsNullOrEmpty(orderId))
                {
                    return BadRequest(new { message = "OrderId is required" });
                }

                var response = await _dummyUpiService.SimulateUpiPaymentAsync(orderId, success: false);

                if (!response.Success)
                {
                    _logger.LogInformation($"Simulated failed payment for OrderId: {orderId}");
                    return Ok(response);
                }

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error simulating failure: {ex.Message}");
                return StatusCode(500, new { message = "Error processing payment" });
            }
        }

        /// <summary>
        /// Verify UPI Payment
        /// </summary>
        [HttpPost("verify")]
        public async Task<IActionResult> VerifyPayment([FromBody] VerifyUpiPaymentRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.OrderId))
                {
                    return BadRequest(new { message = "OrderId is required" });
                }

                var response = await _dummyUpiService.VerifyUpiPaymentAsync(request.OrderId, request.UpiTransactionId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error verifying payment: {ex.Message}");
                return StatusCode(500, new { message = "Error verifying payment" });
            }
        }
    }

    public class VerifyUpiPaymentRequest
    {
        public string OrderId { get; set; }
        public string UpiTransactionId { get; set; }
    }

    public class DummyUpiPaymentViewModel
    {
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public string UpiId { get; set; }
        public string TransactionId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}