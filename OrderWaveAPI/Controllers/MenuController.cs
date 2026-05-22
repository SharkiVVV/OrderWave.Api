using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    
    public class MenuController : ControllerBase
    {
        public readonly AppDbContext _dbContext;
        public MenuController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var dishes = await _dbContext.Menus
                .Include(m => m.Category)
                .Include(m => m.MenuPhoto)
                .Where(m => m.IsActive)
                .OrderBy(m => m.Category.CategoryName)
                .ThenBy(m => m.DishName)
                .Select(m => new MenuItemResponse
                {
                    DishId = m.DishId,
                    DishName = m.DishName,
                    DishPrice = m.DishPrice,
                    IsActive = m.IsActive,
                    CategoryId = m.CategoryId,
                    CategoryName = m.Category.CategoryName,
                    PhotoUrl = m.MenuPhoto != null ? m.MenuPhoto.PhotoUrl : null
                })
                .ToListAsync();
            return Ok(dishes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var dish = await _dbContext.Menus
                .Include(m => m.Category)
                .Include(m => m.MenuPhoto)
                .Include(m => m.DishDetail)
                .FirstOrDefaultAsync(m => m.DishId == id);

            if (dish is null)
            {
                return NotFound(new {message = "Dish not found."});
            }

            return Ok(new DishDetailtResponse
            {
                DishId = dish.DishId,
                DishName = dish.DishName,
                DishPrice = dish.DishPrice,
                IsActive = dish.IsActive,
                CategoryId = dish.CategoryId,
                CategoryName = dish.Category.CategoryName,
                PhotoUrl = dish.MenuPhoto?.PhotoUrl,
                Ingredients = dish.DishDetail?.DishIngredients,
                Description = dish.DishDetail?.DishDescription,
                
            });
            
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateDishRequest request)
        {
            var categoryExists = await _dbContext.MenuCategories
                .AnyAsync(c => c.CategoryId == request.CategoryId);
            if (!categoryExists)
            {
                return BadRequest(new {message = "Category not found."});
            }

            var dish = new Menu
            {
                CategoryId = request.CategoryId,
                DishName = request.DishName,
                DishPrice = request.DishPrice,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
            };
            _dbContext.Menus.Add(dish);
            await _dbContext.SaveChangesAsync();

            if (!string.IsNullOrEmpty(request.PhotoUrl))
            {
                    _dbContext.MenuPhotos.Add(new MenuPhoto
                        {
                            DishId = dish.DishId,
                            PhotoUrl = request.PhotoUrl,
                            IsMain = true,
                            SortOrder = 0,
                            CreatedAt = DateTime.UtcNow,
                        }
                    );
            }

            if (!string.IsNullOrEmpty(request.Ingredients) || !string.IsNullOrEmpty(request.Description))
            {
              
                    _dbContext.DishDetails.Add(
                        new DishDetail
                        {
                            DishId = dish.DishId,
                            DishIngredients = request.Ingredients ?? string.Empty,
                            DishDescription = request.Description ?? string.Empty,
                            CreatedAt = DateTime.UtcNow
                        }
                    );
            }
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = dish.DishId }, 
                new {message = " Dish created successfully", dishId = dish.DishId});
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDishRequest request)
        {
            var dish = await  _dbContext.Menus
                .Include(m => m.MenuPhoto)
                .Include(m => m.DishDetail)
                .FirstOrDefaultAsync(m => m.DishId == id);
                
            
            if (dish is null)
                return NotFound(new {message = "Dish not found."});
            dish.CategoryId = request.CategoryId;
            dish.DishName = request.DishName;
            dish.DishPrice = request.DishPrice;
            dish.IsActive = request.IsActive;
            
            if (!string.IsNullOrEmpty(request.PhotoUrl))
                
            {
                if (dish.MenuPhoto is null)
                {
                    _dbContext.MenuPhotos.Add(
                        new MenuPhoto
                        {
                            DishId = dish.DishId,
                            PhotoUrl = request.PhotoUrl,
                            IsMain = true,
                            SortOrder = 0,
                            CreatedAt = DateTime.UtcNow,
                        }
                    );
                }
                else
                {
                    dish.MenuPhoto.PhotoUrl = request.PhotoUrl;
                }
                
            }

            if (!string.IsNullOrEmpty(request.Ingredients) || !string.IsNullOrEmpty(request.Description))
            {
                if (dish.DishDetail is null)
                {
                    _dbContext.DishDetails.Add(
                        new DishDetail
                        {
                            DishId = dish.DishId,
                            DishIngredients = request.Ingredients ?? string.Empty,
                            DishDescription = request.Description ?? string.Empty,
                            CreatedAt = DateTime.UtcNow
                        }
                    );

                }

                else
                {
                    dish.DishDetail.DishIngredients = request.Ingredients ?? string.Empty;
                    dish.DishDetail.DishDescription = request.Description ?? string.Empty;
                }
                
            }
            await _dbContext.SaveChangesAsync();
            return Ok(new {message = " Dish updated successfully"});
            
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Deactivate(int id)
        {
            var dish = await  _dbContext.Menus
                .FirstOrDefaultAsync(m => m.DishId == id);

            if (dish is null)
            {
                return NotFound(new {message = "Dish not found."});
            }
            if (dish.IsActive == false)
            {
                return BadRequest(new {message = "Dish is not active."});
                
            }
            dish.IsActive = false;
            await _dbContext.SaveChangesAsync();
            return Ok(new {message = " Dish deleted successfully"});
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _dbContext.MenuCategories
                .OrderBy(c => c.CategoryName)
                .Select(c =>new CategoryResponse
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                })
                .ToListAsync();
            return Ok(categories);
        }

        [HttpPost("categories")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategories([FromBody] CreateCategoryRequest request)
        {
            var exists = await _dbContext.MenuCategories
                .AnyAsync(c=>c.CategoryName == request.CategoryName);
            if (exists)
            {
                return Conflict(new {message = "Category already exists."});
            }

            var category = new MenuCategory
            {
                CategoryName = request.CategoryName,
                CreatedAt = DateTime.UtcNow,
            };
                _dbContext.MenuCategories.Add(category);
                await _dbContext.SaveChangesAsync();
                
                return Ok(new {message = " Category created successfully"});
        }


    }
}
