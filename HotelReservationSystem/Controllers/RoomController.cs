using Microsoft.AspNetCore.Mvc;
using HotelReservation.Bll.DTOs;
using HotelReservation.Bll.Services;
using HotelReservation.Domain.Exceptions;

namespace HotelReservation.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly ILogger<RoomController> _logger;

        public RoomController(IRoomService roomService, ILogger<RoomController> logger)
        {
            _roomService = roomService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RoomDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetAllRooms(CancellationToken cancellationToken)
        {
            try
            {
                var rooms = await _roomService.GetAllAsync(cancellationToken);
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all rooms");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(RoomDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RoomDto>> GetRoom(int id, CancellationToken cancellationToken)
        {
            try
            {
                var room = await _roomService.GetByIdAsync(id, cancellationToken);

                if (room == null)
                {
                    return NotFound($"Room with ID {id} not found");
                }

                return Ok(room);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting room with ID {RoomId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

      
        [HttpPost]
        [ProducesResponseType(typeof(RoomDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<RoomDto>> CreateRoom([FromBody] CreateRoomDto createRoomDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var room = await _roomService.CreateAsync(createRoomDto, cancellationToken);
                return CreatedAtAction(nameof(GetRoom), new { id = room.RoomId }, room);
            }
            catch (BusinessConflictException ex)
            {
                return Conflict(ex.Message);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating room");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(RoomDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<RoomDto>> UpdateRoom(int id, [FromBody] RoomDto roomDto, CancellationToken cancellationToken)
        {
            if (id != roomDto.RoomId)
            {
                return BadRequest("Room ID in URL doesn't match the ID in request body");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var room = await _roomService.UpdateAsync(roomDto, cancellationToken);
                return Ok(room);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (BusinessConflictException ex)
            {
                return Conflict(ex.Message);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating room with ID {RoomId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteRoom(int id, CancellationToken cancellationToken)
        {
            try
            {
                var isDeleted = await _roomService.DeleteAsync(id, cancellationToken);

                if (isDeleted)
                {
                    return NoContent();
                }

                return NotFound($"Room with ID {id} not found");
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (BusinessConflictException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting room with ID {RoomId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpGet("available")]
        [ProducesResponseType(typeof(IEnumerable<AvailableRoomDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<AvailableRoomDto>>> GetAvailableRooms(
            [FromQuery] DateTime checkIn,
            [FromQuery] DateTime checkOut,
            CancellationToken cancellationToken)
        {
            try
            {
                var rooms = await _roomService.GetAvailableRoomsAsync(checkIn, checkOut, cancellationToken);
                return Ok(rooms);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available rooms for dates: {CheckIn} - {CheckOut}", checkIn, checkOut);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpGet("by-number/{roomNumber}")]
        [ProducesResponseType(typeof(RoomDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RoomDto>> GetRoomByNumber(string roomNumber, CancellationToken cancellationToken)
        {
            try
            {
                var room = await _roomService.GetByRoomNumberAsync(roomNumber, cancellationToken);

                if (room == null)
                {
                    return NotFound($"Room with number '{roomNumber}' not found");
                }

                return Ok(room);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting room by number: {RoomNumber}", roomNumber);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }
    }
}