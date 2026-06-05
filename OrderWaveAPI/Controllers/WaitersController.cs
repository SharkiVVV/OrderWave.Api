using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using OrderWaveAPI.Data;
using OrderWaveAPI.Models;
using OrderWaveAPI.Transfer.Requests;
using OrderWaveAPI.Transfer.Responses;

namespace OrderWaveAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WaitersController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        
        public WaitersController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var waiters = await _dbContext.Waiters
                .Include(w => w.User)
                .Include(w => w.WaitersShifts)
                .Include(w => w.TableAssignments)
                .ThenInclude(a => a.Session)
                .ThenInclude(s => s.Table)
                .ToListAsync();

            var response = waiters.Select(w =>
            {
                var activeShift = w.WaitersShifts
                    .FirstOrDefault(s => s.ShiftEnd == null);

                var activeTable = w.TableAssignments
                    .Where(a => a.Session.IsActive)
                    .Select(a => a.Session.Table.TableNumber)
                    .ToList();

                return new WaiterResponse
                {
                    WaiterId = w.WaiterId,
                    UserId = w.UserId,
                    FirstName = w.User.FirstName ?? string.Empty,
                    LastName = w.User.LastName ?? string.Empty,
                    Phone = w.User.Phone,
                    IsActive = w.User.IsActive,
                    IsOnShift = activeShift != null,
                    ShiftStart = activeShift?.ShiftStart,
                    AssignedTables = activeTable
                };




            }).ToList();
            
            
            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var waiter = await _dbContext.Waiters
                .Include(w => w.User)
                .Include(w => w.WaitersShifts)
                .Include(w=> w.TableAssignments)
                .ThenInclude(a => a.Session)
                .ThenInclude(s => s.Table)
                .FirstOrDefaultAsync(w => w.WaiterId == id);
            if (waiter is null)
            {
                return NotFound(new {message = "Waiter not found"});
            }
            
            var activeShift = waiter.WaitersShifts
                .FirstOrDefault(s => s.ShiftEnd == null);
            var activeTables = waiter.TableAssignments
                .Where(a => a.Session.IsActive)
                .Select(a => a.Session.Table.TableNumber)
                .ToList();
            

            return Ok(new WaiterResponse
            {
                WaiterId = waiter.WaiterId,
                UserId = waiter.UserId,
                FirstName = waiter.User.FirstName ?? string.Empty,
                LastName = waiter.User.LastName ?? string.Empty,
                Phone = waiter.User.Phone,
                IsActive = waiter.User.IsActive,
                IsOnShift = activeShift != null,
                ShiftStart = activeShift?.ShiftStart,
                AssignedTables = activeTables
                
            });
        }

        [HttpGet("{id}/shifts/active")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetActiveShifts(int id)
        {
            var shift = await _dbContext.WaitersShifts
                .FirstOrDefaultAsync(s=> s.WaiterId == id && s.ShiftEnd == null);
            if (shift is null)
            {
                return  NotFound(new {message = "Active shift not found"});
            }
            
            return Ok(new
            {
                shiftId = shift.ShiftId,
                waiterId = shift.WaiterId,
                ShiftStart = shift.ShiftStart
            });

            
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateWaiterRequest request)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u=> u.UserId == request.UserId);
            if (user is null)
            {
                return NotFound(new {message = "User not found"});
            }

            var alreadyWaiter = await _dbContext.Waiters
                .AnyAsync(w => w.UserId == request.UserId);

            if (alreadyWaiter)
            {
                return Conflict(new {message = "Waiter already exists"});
            }

            var waiter = new Waiter
            {
                UserId = request.UserId,
                CreatedAt = DateTime.UtcNow,
            };
            
            _dbContext.Waiters.Add(waiter);
            await _dbContext.SaveChangesAsync();
            
            return Ok(new {message = "Waiter created", waiterId = waiter.WaiterId});
        }

        [HttpPost("{id}/shifts")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> StartShift(int id)
        {
            var waiter = await _dbContext.Waiters
                .Include(w => w.WaitersShifts)
                .FirstOrDefaultAsync(w => w.WaiterId == id );
            if (waiter is null)
            {
                return NotFound(new {message = "Waiter not found"});
                
            }
            var hasActiveShift = waiter.WaitersShifts
                .Any(s => s.ShiftEnd == null);
            if (hasActiveShift)
            {
                return Conflict(new {message = "Waiter has active shift"});
            }

            var shift = new WaitersShift
            {
                WaiterId = waiter.WaiterId,
                ShiftStart = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
            };
            
            _dbContext.WaitersShifts.Add(shift);
            await _dbContext.SaveChangesAsync();
            
            return Ok(new {message = "Waiters shift started", shiftId = shift.ShiftId});

        }
        
        [HttpPatch("{id}/shifts/{shiftId}/end")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EndShift(int id, int shiftId)
        {
            var shift = await _dbContext.WaitersShifts
                .FirstOrDefaultAsync(s=> s.ShiftId == shiftId && s.WaiterId == id);

            if (shift is null)
            {
                return NotFound(new {message = "Shift not found"});
                
            }

            if (shift.ShiftEnd != null)
            {
                return BadRequest(new {message = "Shift has already ended"});
            }
            
            var hasActiveTables = await _dbContext.TableAssignments
                .AnyAsync(a => a.WaiterId == id && a.Session.IsActive);
            if (hasActiveTables)
            {
                return BadRequest(new {message = "Waiter has active tables. Close the session"});
            }
            
            shift.ShiftEnd = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            
            return Ok(new {message = "Shift ended"});
        }
    }
}

