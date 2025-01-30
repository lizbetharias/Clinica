using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Clinica.Models;

namespace Clinica.Controllers
{
    public class HistorialController : Controller
    {
        private readonly BDContext _context;

        public HistorialController(BDContext context)
        {
            _context = context;
        }

        // GET: Historial
        public async Task<IActionResult> Index()
        {
            ViewData["EspecialidadId"] = new SelectList(_context.Especialidad, "EspecialidadId", "Nombre");
            ViewData["PacienteId"] = new SelectList(_context.Paciente, "PacienteId", "Nombre");
            var bDContext = _context.Historial.Include(h => h.Especialidad).Include(h => h.Paciente);
            return View(await bDContext.ToListAsync());
        }

        // GET: Historial/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var historial = await _context.Historial
                .Include(h => h.Especialidad)
                .Include(h => h.Paciente)
                .FirstOrDefaultAsync(m => m.HistorialId == id);
            if (historial == null)
            {
                return NotFound();
            }

            return View(historial);
        }

        // GET: Historial/Create
        public IActionResult Create(int id)
        {
            ViewData["EspecialidadId"] = new SelectList(_context.Especialidad, "EspecialidadId", "Nombre");
            var paciente = _context.Paciente
        .Where(p => p.PacienteId == id)
        .Select(p => new { p.Nombre, p.Apellido })
        .FirstOrDefault();

            if (paciente == null)
            {
                return NotFound(); // Si no se encuentra el paciente, devolver error 404
            }

            // Enviar los datos al ViewData

            ViewData["PacienteId"] = id;
            ViewData["PacienteNombre"] = $"{paciente.Nombre} {paciente.Apellido}"; // Concatenar Nombre y Apellido

            return View();
        }

        // POST: Historial/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HistorialId,Fecha,PacienteId,EspecialidadId")] Historial historial)
        {
            if (ModelState.IsValid)
            {
                _context.Add(historial);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EspecialidadId"] = new SelectList(_context.Especialidad, "EspecialidadId", "Nombre", historial.EspecialidadId);
            ViewData["PacienteId"] = new SelectList(_context.Paciente, "PacienteId", "Nombre", historial.PacienteId);
            return View(historial);
        }

        // GET: Historial/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var historial = await _context.Historial.FindAsync(id);
            if (historial == null)
            {
                return NotFound();
            }
            ViewData["EspecialidadId"] = new SelectList(_context.Especialidad, "EspecialidadId", "Nombre", historial.EspecialidadId);
            ViewData["PacienteId"] = new SelectList(_context.Paciente, "PacienteId", "Nombre", historial.PacienteId);
            return View(historial);
        }

        // POST: Historial/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HistorialId,Fecha,PacienteId,EspecialidadId")] Historial historial)
        {
            if (id != historial.HistorialId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(historial);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HistorialExists(historial.HistorialId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EspecialidadId"] = new SelectList(_context.Especialidad, "EspecialidadId", "Nombre", historial.EspecialidadId);
            ViewData["PacienteId"] = new SelectList(_context.Paciente, "PacienteId", "Nombre", historial.PacienteId);
            return View(historial);
        }

        // GET: Historial/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var historial = await _context.Historial
                .Include(h => h.Especialidad)
                .Include(h => h.Paciente)
                .FirstOrDefaultAsync(m => m.HistorialId == id);
            if (historial == null)
            {
                return NotFound();
            }

            return View(historial);
        }

        // POST: Historial/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var historial = await _context.Historial.FindAsync(id);
            if (historial != null)
            {
                _context.Historial.Remove(historial);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HistorialExists(int id)
        {
            return _context.Historial.Any(e => e.HistorialId == id);
        }
    }
}
