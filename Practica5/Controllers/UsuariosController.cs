using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Practica5.Data;
using Practica5.Models;
using Practica5.Data;
using Practica5.Models;

namespace Practica5.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly AppDbContext _db;

    public UsuariosController(AppDbContext db) => _db = db;

    // GET /api/usuarios
    [HttpGet]
    public async Task<ActionResult<List<Usuario>>> GetAll()
        => await _db.Usuarios.AsNoTracking().ToListAsync();

    // GET /api/usuarios/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Usuario>> GetById(int id)
    {
        var usuario = await _db.Usuarios.FindAsync(id);
        if (usuario is null) return NotFound(new { message = "Usuario no encontrado." });
        return usuario;
    }

    // POST /api/usuarios
    [HttpPost]
    public async Task<ActionResult<Usuario>> Create(Usuario usuario)
    {
        // Validate duplicate email (app-level)
        var correo = usuario.Correo.Trim().ToLowerInvariant();
        var exists = await _db.Usuarios.AnyAsync(u => u.Correo.ToLower() == correo);
        if (exists)
            return BadRequest(new { message = "El correo electrónico ya está en uso." });

        usuario.Correo = correo;

        _db.Usuarios.Add(usuario);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            // In case DB unique index catches something (race condition)
            return BadRequest(new { message = "El correo electrónico ya está en uso." });
        }

        return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuario);
    }

    // PUT /api/usuarios/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Usuario input)
    {
        var usuario = await _db.Usuarios.FindAsync(id);
        if (usuario is null) return NotFound(new { message = "Usuario no encontrado." });

        var newCorreo = input.Correo.Trim().ToLowerInvariant();

        // If email changes, validate uniqueness
        if (!string.Equals(usuario.Correo, newCorreo, StringComparison.OrdinalIgnoreCase))
        {
            var exists = await _db.Usuarios.AnyAsync(u => u.Correo.ToLower() == newCorreo && u.Id != id);
            if (exists)
                return BadRequest(new { message = "El correo electrónico ya está en uso." });

            usuario.Correo = newCorreo;
        }

        usuario.Nombre = input.Nombre;
        usuario.FechaDeNacimiento = input.FechaDeNacimiento;

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return BadRequest(new { message = "El correo electrónico ya está en uso." });
        }

        return NoContent();
    }

    // DELETE /api/usuarios/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var usuario = await _db.Usuarios.FindAsync(id);
        if (usuario is null) return NotFound(new { message = "Usuario no encontrado." });

        _db.Usuarios.Remove(usuario);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}