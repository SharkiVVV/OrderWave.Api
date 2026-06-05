using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using OrderWaveAPI.Models;
using OrderWaveAPI.Data;
using OrderWaveAPI.Transfer.Requests;
using OrderWaveAPI.Transfer.Responses;

namespace OrderWaveAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GuestsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        
        public GuestsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("session/{sessionId}")]
        public async Task<IActionResult> GetBySession( int sessionId)
        {
            var sessionExists = await _dbContext.TableSessions
                .AnyAsync(s=>s.SessionId == sessionId);
            if (!sessionExists)
            {
                return NotFound(new{message = "Session not found"});
            }
            
            var guests = await _dbContext.Guests
                .Include(g=>g.Orders)
                .ThenInclude(o=>o.OrderDetails)
                .Where(g => g.SessionId == sessionId)
                .ToListAsync();
            
            var response = guests.Select(
                g => new GuestResponse
                {
                    GuestId = g.GuestId,
                    GuestName = g.GuestName,
                    GuestSurname = g.GuestSurname,
                    
                    TotalAmount = g.Orders
                        .Where( o => o.CurrentStatus != "Cancelled")
                        .SelectMany(o => o.OrderDetails)
                        .Sum(od => od.DishPrice * od.DishAmount)
                }).ToList();

            return Ok(response);
            
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var guest = await _dbContext.Guests
                .Include(g => g.Orders)
                .ThenInclude(o => o.OrderDetails)
                .FirstOrDefaultAsync(g => g.GuestId == id);

            if (guest is null)
            {
                return NotFound(new{message = "Guest not found"});
                
            }
            
            return Ok(new GuestResponse
            {
                GuestId = guest.GuestId,
                GuestName = guest.GuestName,
                GuestSurname = guest.GuestSurname,
                TotalAmount = guest.Orders
                    .Where(o => o.CurrentStatus != "Cancelled")
                    .SelectMany(o=> o.OrderDetails)
                    .Sum(od => od.DishPrice * od.DishAmount)
                
            });
        }

        [HttpPost("session/{sessionId}")]
        public async Task<IActionResult> Create(int sessionId, [FromBody] CreateGuestRequest request)
        {
            var session = await _dbContext.TableSessions
                .FirstOrDefaultAsync(s=> s.SessionId == sessionId);
            if (session is null)
            {
                return NotFound(new{message = "Session not found"});
            }

            if (!session.IsActive)
            {
                return BadRequest(new{message = "Session is already  disactive"});
            }

            var guest = new Guest
            {
                SessionId = sessionId,
                GuestName = request.GuestName,
                GuestSurname = request.GuestSurname,
                CreatedAt = DateTime.UtcNow,
            };
            _dbContext.Guests.Add(guest);
            await _dbContext.SaveChangesAsync();
            
            return Ok(new {message = "Guest successfully created", guestId = guest.GuestId});

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var guest = await _dbContext.Guests
                .Include(g => g.Orders)
                .FirstOrDefaultAsync(g => g.GuestId == id);

            if (guest is null)
            {
                return NotFound(new{message = "Guest not found"});
            }

            if (guest.Orders.Any())
            {
                return BadRequest(new{message = "Cannot delete guest with orders"});
                
            }
            
            _dbContext.Guests.Remove(guest);
            await _dbContext.SaveChangesAsync();
            return Ok(new {message = "Guest successfully deleted"});
        }


    }
}
