using Microsoft.AspNetCore.Mvc;
using HotelCatalog.Bll.DTOs;
using HotelCatalog.Bll.Interfaces;

namespace HotelCatalog.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class DiscountCategoryController : ControllerBase
    {
        private readonly IDiscountCategoryService _discountCategoryService;
        private readonly ILogger<DiscountCategoryController> _logger;

        public DiscountCategoryController(IDiscountCategoryService discountCategoryService, ILogger<DiscountCategoryController> logger)
        {
            _discountCategoryService = discountCategoryService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DiscountCategoryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DiscountCategoryDto>>> GetAllDiscounts(CancellationToken cancellationToken)
        {
            try
            {
                var discounts = await _discountCategoryService.GetAllAsync(cancellationToken);
                return Ok(discounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all discount categories");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(DiscountCategoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DiscountCategoryDto>> GetDiscount(int id, CancellationToken cancellationToken)
        {
            try
            {
                var discount = await _discountCategoryService.GetByIdAsync(id, cancellationToken);

                if (discount == null)
                {
                    return NotFound($"Discount category with ID {id} not found");
                }

                return Ok(discount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting discount category with ID {DiscountId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpGet("above-percent")]
        [ProducesResponseType(typeof(IEnumerable<DiscountCategoryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<DiscountCategoryDto>>> GetDiscountsAbovePercent(
            [FromQuery] decimal percent,
            CancellationToken cancellationToken)
        {
            try
            {
                var discounts = await _discountCategoryService.GetDiscountsAbovePercentAsync(percent, cancellationToken);
                return Ok(discounts);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting discounts above percent");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(DiscountCategoryDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<DiscountCategoryDto>> CreateDiscount([FromBody] CreateDiscountCategoryDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var discount = await _discountCategoryService.CreateAsync(dto, cancellationToken);
                return CreatedAtAction(nameof(GetDiscount), new { id = discount.DiscountId }, discount);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating discount category");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(DiscountCategoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<DiscountCategoryDto>> UpdateDiscount(int id, [FromBody] UpdateDiscountCategoryDto dto, CancellationToken cancellationToken)
        {
            if (id != dto.DiscountId)
            {
                return BadRequest("Discount ID in URL doesn't match the ID in request body");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var discount = await _discountCategoryService.UpdateAsync(dto, cancellationToken);
                return Ok(discount);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating discount category with ID {DiscountId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteDiscount(int id, CancellationToken cancellationToken)
        {
            try
            {
                var isDeleted = await _discountCategoryService.DeleteAsync(id, cancellationToken);

                if (isDeleted)
                {
                    return NoContent();
                }

                return NotFound($"Discount category with ID {id} not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting discount category with ID {DiscountId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }
    }
}