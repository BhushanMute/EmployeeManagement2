using EmployeeManagement.API.Models;
using EmployeeManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.API.Repositories;

namespace EmployeeManagement.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentGatewayService _paymentGatewayService;
        private readonly IPaymentRepository _paymentRepository;

        public PaymentController(IPaymentGatewayService paymentGatewayService, IPaymentRepository paymentRepository)
        {
            _paymentGatewayService = paymentGatewayService;
            _paymentRepository = paymentRepository;
        }

        [HttpPost("initiate")]
        public async Task<IActionResult> InitiatePayment([FromBody] PaymentRequest request)
        {
            if (request == null || request.Amount <= 0)
            {
                return BadRequest(new { message = "Invalid payment request" });
            }

            var response = await _paymentGatewayService.InitiatePaymentAsync(request);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("status/{orderId}")]
        public async Task<IActionResult> GetPaymentStatus(string orderId)
        {
            var response = await _paymentGatewayService.GetPaymentStatusAsync(orderId);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("callback")]
        public async Task<IActionResult> PaymentCallback([FromBody] PaymentCallbackRequest callback)
        {
            try
            {
                // Verify signature
                bool isValid = await _paymentGatewayService.VerifyPaymentSignatureAsync(callback);

                if (!isValid)
                {
                    return BadRequest(new { message = "Invalid signature" });
                }

                // Get payment and update status
                var payment = await _paymentRepository.GetPaymentByOrderIdAsync(callback.OrderId);

                if (payment != null)
                {
                    await _paymentRepository.UpdatePaymentStatusAsync(payment.Id, callback.Status.ToUpper(), callback.TransactionId);
                }

                return Ok(new { message = "Callback processed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("refund/{transactionId}")]
        public async Task<IActionResult> ProcessRefund(string transactionId, [FromBody] dynamic request)
        {
            decimal amount = request.amount;
            var response = await _paymentGatewayService.ProcessRefundAsync(transactionId, amount);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetEmployeePayments(int employeeId)
        {
            var payments = await _paymentRepository.GetPaymentsByEmployeeAsync(employeeId);
            return Ok(payments);
        }

        [HttpGet("total/{employeeId}")]
        public async Task<IActionResult> GetTotalPayments(int employeeId)
        {
            var total = await _paymentRepository.GetTotalPaymentsByEmployeeAsync(employeeId);
            return Ok(new { employeeId, totalPayments = total });
        }
    }
}