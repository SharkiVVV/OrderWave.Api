using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderWaveAPI.Data;
using OrderWaveAPI.Models;
using OrderWaveAPI.Transfer.Requests;
using OrderWaveAPI.Transfer.Responses;

namespace OrderWaveAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public OrdersController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("guest/{guestId}")]
        public async Task<IActionResult> GetByGuest(int guestId)
        {
            var guestExists = await _dbContext.Guests
                .AnyAsync(g => g.GuestId == guestId);
            if (!guestExists)
            {
                return NotFound(new { message = "Guest not found" });
            }

            var orders = await _dbContext.Orders
                .Include(o => o.Guest)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Dish)
                .Where(o => o.GuestId == guestId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            var response = orders.Select(o => new OrderResponse
            {
                OrderId = o.OrderId,
                GuestId = o.GuestId,
                GuestName = o.Guest.GuestName,
                WaiterId = o.WaiterId,
                SessionId = o.SessionId,
                CurrentStatus = o.CurrentStatus,
                OrderDate = o.OrderDate,
                Items = o.OrderDetails.Select(od => new OrderDetailResponse
                {
                    OrderDetailId = od.OrderDetailId,
                    DishId = od.DishId,
                    DishName = od.Dish.DishName,
                    DishAmount = od.DishAmount,
                    DishPrice = od.DishPrice,
                    Subtotal = od.DishPrice * od.DishAmount

                }).ToList(),
                TotalAmount = o.OrderDetails.Sum(od => od.DishPrice * od.DishAmount)
            });

            return Ok(response);
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetById(int orderId)
        {
            var order = await _dbContext.Orders
                .Include(o => o.Guest)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Dish)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order is null)
            {
                return NotFound(new { message = "Order not found" });
            }

            return Ok(new OrderResponse
            {
                OrderId = order.OrderId,
                GuestId = order.GuestId,
                GuestName = order.Guest.GuestName,
                WaiterId = order.WaiterId,
                SessionId = order.SessionId,
                CurrentStatus = order.CurrentStatus,
                OrderDate = order.OrderDate,
                Items = order.OrderDetails.Select(od => new OrderDetailResponse
                {
                    OrderDetailId = od.OrderDetailId,
                    DishId = od.DishId,
                    DishName = od.Dish.DishName,
                    DishAmount = od.DishAmount,
                    DishPrice = od.DishPrice,
                    Subtotal = od.DishPrice * od.DishAmount

                }).ToList(),
                TotalAmount = order.OrderDetails.Sum(od => od.DishPrice * od.DishAmount)
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim is null)
            {
                return Unauthorized(new { message = "User not found or Invalid token" });

            }

            var userId = int.Parse(userIdClaim);

            if (!request.Items.Any())
            {
                return BadRequest(new { message = "Order must contain at least one item" });
            }

            var session = await _dbContext.TableSessions
                .FirstOrDefaultAsync(s => s.SessionId == request.SessionId);
            if (session is null)
            {
                return NotFound(new { message = "Session not found" });
            }

            if (!session.IsActive)
            {
                return BadRequest(new { message = "Session is not active or already closed" });

            }

            var guest = await _dbContext.Guests
                .FirstOrDefaultAsync(g => g.GuestId == request.GuestId);
            if (guest is null)
            {
                return NotFound(new { message = "Guest not found" });
            }

            if (guest.SessionId != request.SessionId)
            {
                return BadRequest(new { message = "Guest not belong to this session" });
            }

            var waiterExists = await _dbContext.Waiters
                .AnyAsync(w => w.WaiterId == request.WaiterId);
            if (!waiterExists)
            {
                return NotFound(new { message = "Waiter not found" });
            }

            var dishIds = request.Items.Select(i => i.DishId).ToList();
            var dishes = await _dbContext.Menus
                .Where(m => dishIds.Contains(m.DishId) && m.IsActive)
                .ToListAsync();

            if (dishes.Count != dishIds.Count)
            {
                return BadRequest(new { message = "One or more dishes mismatch" });
            }

            var order = new Order
            {
                GuestId = request.GuestId,
                WaiterId = request.WaiterId,
                SessionId = request.SessionId,
                CurrentStatus = "Pending",
                OrderDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
            };

            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();

            var orderDetails = request.Items.Select(item =>
            {
                var dish = dishes.First(d => d.DishId == item.DishId);
                return new OrderDetail
                {
                    OrderId = order.OrderId,
                    DishId = item.DishId,
                    DishAmount = item.Amount,
                    DishPrice = dish.DishPrice,
                    CreatedAt = DateTime.UtcNow,
                };
            }).ToList();
            _dbContext.OrderDetails.AddRange(orderDetails);
            await _dbContext.SaveChangesAsync();

            var kitchenItems = orderDetails.Select(od => new KitchenQueue
            {
                OrderDetailId = od.OrderDetailId,
                DishStatus = "Cooking",
                CreatedAt = DateTime.UtcNow,


            }).ToList();
            _dbContext.KitchenQueues.AddRange(kitchenItems);

            _dbContext.OrderStatusHistories.Add(new OrderStatusHistory
            {
                OrderId = order.OrderId,
                ChangedBy = userId,
                Status = "Pending",
                ChangedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
            });
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Order created", orderId = order.OrderId });

        }

        [HttpPatch("{orderId}/status")]
        public async Task<IActionResult> UpdateStatus(int orderId, [FromBody] UpdateOrdersStatusRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim is null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var userId = int.Parse(userIdClaim);

            var order = await _dbContext.Orders
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order is null)
            {
                return NotFound(new { message = "Order not found" });

            }

            if (order.CurrentStatus is "Completed" or "Cancelled")
            {
                return BadRequest(new { message = $"Can not change status. Order is already {order.CurrentStatus}" });
            }
            
            var allowedStatuses = new[]{"Pending", "In_Progress", "Completed", "Cancelled"};

            if (!allowedStatuses.Contains(request.Status))
            {
                return BadRequest(new { message = $"Invalid status {request.Status}" });
            }

            order.CurrentStatus = request.Status;

            if (request.Status == "Cancelled")
            {
                var kitchenItems = await _dbContext.KitchenQueues
                    .Include(q=>q.OrderDetail)
                    .Where(q=> q.OrderDetail.OrderId == orderId && q.DishStatus == "Cooking")
                    .ToListAsync();
                foreach (var item in kitchenItems)
                {
                    item.DishStatus = "Cancelled";
                }
                
            }

            _dbContext.OrderStatusHistories.Add(new OrderStatusHistory
            {
                OrderId = order.OrderId,
                ChangedBy = userId,
                Status = request.Status,
                ChangedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,

            });
            
            await _dbContext.SaveChangesAsync();
            
            return Ok(new { message = $"Status updated to {request.Status}" });
        }




















    }
}
