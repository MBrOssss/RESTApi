using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RESTApi.Data;
using RESTApi.Data.Models;

namespace RESTApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DoctorsController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoctorsController"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public DoctorsController(ApplicationDbContext context, ILogger<DoctorsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Gets the list of all doctors.
        /// </summary>
        /// <returns>A list of doctors.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Doctor>>> GetDoctors()
        {
            try
            {
                return await _context.Doctors.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting doctors.");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets a specific doctor by ID.
        /// </summary>
        /// <param name="id">The doctor ID.</param>
        /// <returns>The doctor with the specified ID.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Doctor>> GetDoctor(int id)
        {
            try
            {
                var doctor = await _context.Doctors.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);

                if (doctor == null)
                {
                    return NotFound();
                }

                return doctor;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting doctor with id {id}.");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Updates a specific doctor.
        /// </summary>
        /// <param name="id">The doctor ID.</param>
        /// <param name="doctor">The doctor entity with updated information.</param>
        /// <returns>No content if the update is successful.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDoctor(int id, Doctor doctor)
        {
            if (id != doctor.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(doctor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DoctorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating doctor with id {id}.");
                return StatusCode(500, "Internal server error");
            }

            return NoContent();
        }

        /// <summary>
        /// Creates a new doctor.
        /// </summary>
        /// <param name="doctor">The doctor entity to create.</param>
        /// <returns>The created doctor entity.</returns>
        [HttpPost]
        public async Task<ActionResult<Doctor>> PostDoctor(Doctor doctor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Doctors.Add(doctor);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetDoctor", new { id = doctor.Id }, doctor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new doctor.");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Deletes a specific doctor by ID.
        /// </summary>
        /// <param name="id">The doctor ID.</param>
        /// <returns>No content if the deletion is successful.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            try
            {
                var doctor = await _context.Doctors.FindAsync(id);
                if (doctor == null)
                {
                    return NotFound();
                }

                _context.Doctors.Remove(doctor);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting doctor with id {id}.");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Checks if a doctor exists by ID.
        /// </summary>
        /// <param name="id">The doctor ID.</param>
        /// <returns>True if the doctor exists, otherwise false.</returns>
        private bool DoctorExists(int id)
        {
            return _context.Doctors.Any(e => e.Id == id);
        }
    }
}
