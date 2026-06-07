using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderWaveAPI.Data;
using OrderWaveAPI.Transfer.Requests;
using OrderWaveAPI.Transfer.Responses;

namespace OrderWaveAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KitchenController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        
        public KitchenController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetQueue([FromQuery] string? status = "Cooking")
        {
            var query = _dbContext.KitchenQueues
                .Include(q =>q.OrderDetail)
                    .ThenInclude(od => od.Dish)
                .Include(q => q.OrderDetail)
                    .ThenInclude(od => od.Order)
                        .ThenInclude(o=> o.Guest)
                .Include(q => q.OrderDetail)
                    .ThenInclude(od => od.Order)
                        .ThenInclude(o => o.Session)
                            .ThenInclude(s=>s.Table)
                .AsQueryable();
            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(q=> q.DishStatus == status);
            }
            
            var items = await query
                .OrderBy(q=> q.CreatedAt)
                .Select(q=> new KitchenItemResponse
                {
                    QueueId =  q.QueueId,
                    OrderDetailId =   q.OrderDetailId,
                    OrderId = q.OrderDetail.OrderId,
                    DishName = q.OrderDetail.Dish.DishName,
                    DishAmount = q.OrderDetail.DishAmount,
                    TableNumber = q.OrderDetail.Order.Session.Table.TableNumber,
                    GuestName = q.OrderDetail.Order.Guest.GuestName,
                    DishStatus = q.DishStatus,
                    CreatedAt = q.CreatedAt
                })
                .ToListAsync();

            return Ok(items);
        }

        [HttpPatch("{queueId}/status")]
        public async Task<IActionResult> UpdateStatus(int queueId, [FromBody] UpdateKitchenStatusRequest request)
        {
            var item = await _dbContext.KitchenQueues
                .FirstOrDefaultAsync(q=>q.QueueId == queueId);
            if (item is null)
            {
                return NotFound(new {message = "Kitchen queue not found"});
            }

            if (item.DishStatus != "Cooking")
            {
                return BadRequest(new {message = $"Cannot update status. Current status is {item.DishStatus}"});
            }
            
            var allowedStatuses = new[] {"Ready", "Cancelled"};

            if (!allowedStatuses.Contains(request.DishStatus))
            {
                return BadRequest(new {message = "Status must be Ready or Cancelled"});
            }
            
            item.DishStatus = request.DishStatus;
            await _dbContext.SaveChangesAsync();
            
            return Ok(new {message = $"Kitchen status updated successfully {request.DishStatus}"});
            
        }
    }
}
