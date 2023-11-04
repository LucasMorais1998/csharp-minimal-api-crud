using ApiCrud.Data;
using ApiCrud.Students;
using Microsoft.EntityFrameworkCore;

namespace ApiCrud.StudentsRoutes;

public static class StudentsRoutes
{
    public static void AddStudentsRoutes(this WebApplication app)
    {
        var studentsRoutes = app.MapGroup("students");

        studentsRoutes.MapGet("", async (
            AppDbContext context,
            CancellationToken ct) =>
       {
           var allStudents = await context
               .Students
               .Where(student => student.IsActive)
               .Select(student => new StudentDTO(student.Id, student.Name))
               .ToListAsync(ct);

           return allStudents;
       });

        studentsRoutes.MapPost("", async (
            AddStudentRequest request,
            AppDbContext context,
            CancellationToken ct) =>
        {
            var studentAlreadyExists = await context.Students
                .AnyAsync(student => student.Name == request.Name, ct);


            if (studentAlreadyExists)
                return Results.Conflict("Student already exists!");


            var newStudent = new Student(request.Name);

            await context.Students.AddAsync(newStudent, ct);
            await context.SaveChangesAsync(ct);

            return Results.Ok(new StudentDTO(newStudent.Id, newStudent.Name));
        });

        studentsRoutes.MapPut("{id}", async (
            Guid id,
            UpdateStudentRequest request,
            AppDbContext context,
            CancellationToken ct) =>
        {
            var student = await context.Students
                .SingleOrDefaultAsync(student => student.Id == id, ct);

            if (student == null) return Results.NotFound("Student not found!");

            student.UpdateName(request.Name);

            await context.SaveChangesAsync(ct);

            return Results.Ok(new StudentDTO(student.Id, student.Name));
        });

        studentsRoutes.MapDelete("{id}", async (Guid id,
            AppDbContext context,
            CancellationToken ct) =>
        {
            var student = await context.Students
                .SingleOrDefaultAsync(student => student.Id == id, ct);

            if (student == null) return Results.NotFound("Student not found!");

            student.DisableStudent();

            await context.SaveChangesAsync(ct);

            return Results.Ok();
        });
    }
}