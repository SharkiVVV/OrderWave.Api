using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OrderWaveAPI.Data;
using OrderWaveAPI.Models;
using OrderWaveAPI.Services;
using OrderWaveAPI.Transfer.Requests;
using OrderWaveAPI.Transfer.Responses;

namespace OrderWaveAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TablesController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        
        public TablesController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tables = await _dbContext.RestaurantTables
                .Include(t => t.TableSession)
                .ThenInclude(s => s!.Orders)
                .ThenInclude(o => o.OrderDetails)
                .OrderBy(t => t.TableNumber)
                .ToListAsync();
            var response = tables.Select(t =>
            {
                var session = t.TableSession?.IsActive == true ? t.TableSession : null;
                var total = session?.Orders
                    .Where(o => o.CurrentStatus != "Cancelled")
                    .SelectMany(o => o.OrderDetails)
                    .Sum(od => od.DishPrice * od.DishAmount) ?? 0;
                return new TableResponse
                {
                    TableId = t.TableId,
                    TableNumber = t.TableNumber,
                    TableCapacity = t.TableCapacity,
                    IsActive = t.IsActive,
                    IsOccupied = session != null,
                    SessionId = session?.SessionId,
                    GuestsAmount = session?.GuestsAmount ?? 0,
                    TotalAmount = total,
                };
            }).ToList();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var table = await _dbContext.RestaurantTables
                .Include(t => t.TableSession)
                .ThenInclude(s => s!.TableAssignments)
                .ThenInclude(a => a.Waiter)
                .ThenInclude(w => w.User)
                .FirstOrDefaultAsync(t => t.TableId == id);

            if (table is null)
            {
                return NotFound(new {message = "Table not found"});
            }
            var session = table.TableSession;

            return Ok(new TableResponse
            {
                TableId = table.TableId,
                TableNumber = table.TableNumber,
                TableCapacity = table.TableCapacity,
                IsActive = table.IsActive,
                IsOccupied = session != null,
                SessionId = session?.SessionId,
                GuestsAmount = session?.GuestsAmount ?? 0
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateTableRequest request)
        {
            var exists = await _dbContext.RestaurantTables
                .AnyAsync(t=>t.TableNumber == request.TableNumber);
            if (exists)
            {
                return Conflict(new{message = "Table already exists"} );
            }

         

            var table = new RestaurantTable
            {
                TableNumber = request.TableNumber,
                TableCapacity = request.TableCapacity,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            
            _dbContext.RestaurantTables.Add(table);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = table.TableId }, 
                new {message = "Table crated", table.TableId});
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTableRequest request)
        {
            var table = await _dbContext.RestaurantTables
                .FirstOrDefaultAsync(t=>t.TableId == id);
            if (table is null)
            {
                return NotFound(new {message = "Table not found"});
            }
            
            var numberTaken = await _dbContext.RestaurantTables
                .AnyAsync(t=> t.TableNumber == request.TableNumber && t.TableId != id );
            if (numberTaken)
            {
                return Conflict(new{message = "Table already exists"});
            }
            
            table.TableNumber = request.TableNumber;
            table.TableCapacity = request.TableCapacity;
            table.IsActive = request.IsActive;
            
            await _dbContext.SaveChangesAsync();
            return Ok(new { message = "Table successfully updated" });
        }

        [HttpPost("{id}/session")]
        public async Task<IActionResult> OpenSession(int id, [FromBody] OpenSessionRequest request)
        {
            var table = await _dbContext.RestaurantTables
                .Include(t => t.TableSession)
                .FirstOrDefaultAsync(t => t.TableId == id);

            if (table is null)
            {
                return NotFound(new {message = "Table not found"});
            }

            if (!table.IsActive)
            {
                return BadRequest(new {message = "Table is not active"});
            }
            
            var activeSession = await _dbContext.TableSessions
                .AnyAsync(s => s.TableId == id && s.IsActive);

            if (activeSession)
            {
                return Conflict(new{message = "Table is already occupied"});
            }
            
            
            var waiter = await _dbContext.Waiters
                .FirstOrDefaultAsync(w=>w.WaiterId == request.WaiterId);

            if (waiter is null)
            {
                return NotFound(new {message = "Waiter not found"});
            }
            
            var session= new TableSession
            {
                TableId = id,
                IsActive = true,
                GuestsAmount = request.GuestAmount,
                OpenedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
            _dbContext.TableSessions.Add(session);
            await _dbContext.SaveChangesAsync();

            _dbContext.TableAssignments.Add(new TableAssignment
            {
                SessionId = session.SessionId,
                WaiterId = request.WaiterId,
                AssignedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            });
            
            await _dbContext.SaveChangesAsync();
            
            return Ok(new {message = "Session successfully opened", sessionId = session.SessionId});
        }

        [HttpGet("session/{sessionId}")]
        public async Task<IActionResult> GetSession(int sessionId)
        {
            var session = await _dbContext.TableSessions
                .Include(s => s.Table)
                .Include(s => s.TableAssignments)
                .ThenInclude(a => a.Waiter)
                .ThenInclude(w => w.User)
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);

            if (session is null)
            {
                return NotFound(new {message = "Session not found"});
                
            }

            return Ok( new SessionResponse
            {
                SessionId = session.SessionId,
                TableId =  session.Table.TableId,
                IsActive = session.Table.IsActive,
                GuestsAmout = session.GuestsAmount,
                OpenedAt = session.OpenedAt,
                ClosedAt = session.ClosedAt,
                Waiters = session.TableAssignments.Select(a => new AssignedWaiterResponse
                {
                    WaiterId = a.WaiterId,
                    FirstName = a.Waiter.User.FirstName?? string.Empty,
                    LastName = a.Waiter.User.LastName
                }).ToList(),
            });
        }

        [HttpPatch("session/{sessionId}/close")]
        public async Task<IActionResult> CloseSession(int sessionId)
        {
            var session = await _dbContext.TableSessions
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);

            if (session is null)
            {
                return NotFound(new {message = "Session not found"});
            }

            if (!session.IsActive)
            {
                return BadRequest(new {message = "Session is not active or already closed" });
                
            }
            
            var hasActiveOrder = await _dbContext.Orders
                .AnyAsync(o => o.SessionId == sessionId && 
                               (o.CurrentStatus == "Pending"|| 
                                o.CurrentStatus == "In_Progress"));
            if (!hasActiveOrder)
            {
                return BadRequest(new {message = "Session with this order not found"});
            }

            if (hasActiveOrder)
            {
                return BadRequest(new {message = "Session has active order"});
                
                
            }
            
            session.IsActive = false;
            session.ClosedAt = DateTime.UtcNow;
            
            await _dbContext.SaveChangesAsync();
            return Ok(new { message = "Session successfully closed" });

        }




    }
}










