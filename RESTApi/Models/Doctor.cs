using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using RESTApi.Models;

namespace RESTApi.Models
{
    public class Doctor : BaseModel
    {
        [Comment("Identyfikator")]
        public int Id { get; set; }

        [Comment("Imię")]
        public string FirstName { get; set; }

        [Comment("Nazwisko")]
        public string LastName { get; set; }

        [Comment("Specjalizacja")]
        public string Specialization { get; set; }
    }


public static class DoctorEndpoints
{
	public static void MapDoctorEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Doctor").WithTags(nameof(Doctor));

        group.MapGet("/", async (ApplicationDbContext db) =>
        {
            return await db.Doctors.ToListAsync();
        })
        .WithName("GetAllDoctors")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<Doctor>, NotFound>> (int id, ApplicationDbContext db) =>
        {
            return await db.Doctors.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is Doctor model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetDoctorById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, Doctor doctor, ApplicationDbContext db) =>
        {
            var affected = await db.Doctors
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                  .SetProperty(m => m.Id, doctor.Id)
                  .SetProperty(m => m.FirstName, doctor.FirstName)
                  .SetProperty(m => m.LastName, doctor.LastName)
                  .SetProperty(m => m.Specialization, doctor.Specialization)
                  .SetProperty(m => m.CreatedDate, doctor.CreatedDate)
                  .SetProperty(m => m.UpdatedDate, doctor.UpdatedDate)
                  .SetProperty(m => m.CreatedUserId, doctor.CreatedUserId)
                  .SetProperty(m => m.UpdatedUserId, doctor.UpdatedUserId)
                  );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateDoctor")
        .WithOpenApi();

        group.MapPost("/", async (Doctor doctor, ApplicationDbContext db) =>
        {
            db.Doctors.Add(doctor);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Doctor/{doctor.Id}",doctor);
        })
        .WithName("CreateDoctor")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, ApplicationDbContext db) =>
        {
            var affected = await db.Doctors
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteDoctor")
        .WithOpenApi();
    }
}}
