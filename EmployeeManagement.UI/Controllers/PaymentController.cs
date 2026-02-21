using EmployeeManagement.UI.Models;
using EmployeeManagement.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.UI.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int employeeId)
        {
            try
            {
                var payments = await _paymentService.GetEmployeePaymentsAsync(employeeId);
                var total = await _paymentService.GetTotalPaymentsAsync(employeeId);

                ViewBag.EmployeeId = employeeId;
                ViewBag.TotalPayments = total;

                return View(payments);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading payment history: {ex.Message}");
                ViewBag.Error = "Failed to load payment history";
                return View(new List<Payment>());
            }
        }

        [HttpGet]
        public IActionResult InitiatePayment(int? employeeId)
        {
            var model = new PaymentRequest();
            if (employeeId.HasValue)
            {
                model.EmployeeId = employeeId.Value;
            }
            return View(model);
        }

        [HttpPost]
         public async Task<IActionResult> InitiatePayment(PaymentRequest request)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Please fill all required fields correctly";
                _logger.LogWarning($"Invalid payment request for method: {request.PaymentMethod}");
                return View(request);
            }

            try
            {
                _logger.LogInformation($"Processing {request.PaymentMethod} payment - Amount: {request.Amount}, UPI: {request.UpiId}");

                var response = await _paymentService.InitiatePaymentAsync(request);

                if (!response.Success)
                {
                    ViewBag.Error = response.Message;
                    _logger.LogWarning($"Payment initiation failed: {response.Message}");
                    return View(request);
                }

                _logger.LogInformation($"Payment initiated successfully. Redirecting to: {response.PaymentUrl}");

                // Store OrderId in session for later verification
                HttpContext.Session.SetString("CurrentOrderId", response.OrderId);

                // Redirect to payment gateway
                return Redirect(response.PaymentUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error initiating payment: {ex.Message}");
                ViewBag.Error = "An error occurred while initiating payment";
                return View(request);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Success(string orderId)
        {
            try
            {
                if (string.IsNullOrEmpty(orderId))
                {
                    return RedirectToAction("Failure", new { orderId = "Unknown" });
                }

                var response = await _paymentService.GetPaymentStatusAsync(orderId);
                
                if (response.Success && response.PaymentStatus == "Completed")
                {
                    _logger.LogInformation($"Payment successful - OrderId: {orderId}");
                    return View(response);
                }

                return RedirectToAction("Failure", new { orderId = orderId });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Payment Success: {ex.Message}");
                return RedirectToAction("Failure", new { orderId = orderId });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Failure(string orderId)
        {
            ViewBag.OrderId = orderId;
            _logger.LogInformation($"Payment failed - OrderId: {orderId}");
            return View();
        }
    }
}