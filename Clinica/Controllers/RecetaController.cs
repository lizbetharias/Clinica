using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Clinica.Models;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Drawing.Printing;
using DinkToPdf;

namespace Clinica.Controllers
{
    public class RecetaController : Controller
    {
        private readonly ICompositeViewEngine _viewEngine;
        private readonly BDContext _context;

        public RecetaController(BDContext context, ICompositeViewEngine viewEngine)
        {
            _context = context;
            _viewEngine = viewEngine;
        }

        // GET: Receta
        public async Task<IActionResult> Index()
        {
            var bDContext = _context.Receta.Include(r => r.Diagnostico).Include(r => r.IdUsuarioNavigation).Include(r => r.Paciente);
            return View(await bDContext.ToListAsync());
        }

        // GET: Receta/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receta = await _context.Receta
                .Include(r => r.Diagnostico)
                .Include(r => r.IdUsuarioNavigation)
                .Include(r => r.Paciente)
                .FirstOrDefaultAsync(m => m.RecetaId == id);
            if (receta == null)
            {
                return NotFound();
            }

            return View(receta);
        }

        // GET: Receta/Create
        public IActionResult Create()
        {
            ViewData["DiagnosticoId"] = new SelectList(_context.Diagnostico, "DiagnosticoId", "DiagnosticoId");
            ViewData["IdUsuario"] = new SelectList(_context.Usuario, "Id", "Id");
            ViewData["PacienteId"] = new SelectList(_context.Paciente, "PacienteId", "PacienteId");
            return View();
        }

        // POST: Receta/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RecetaId,Fecha,PacienteId,IdUsuario,DiagnosticoId")] Receta receta)
        {
            ModelState.Remove("IdUsuarioNavigation");
            if (ModelState.IsValid)
            {
                _context.Add(receta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DiagnosticoId"] = new SelectList(_context.Diagnostico, "DiagnosticoId", "DiagnosticoId", receta.DiagnosticoId);
            ViewData["IdUsuario"] = new SelectList(_context.Usuario, "Id", "Id", receta.IdUsuario);
            ViewData["PacienteId"] = new SelectList(_context.Paciente, "PacienteId", "PacienteId", receta.PacienteId);
            return View(receta);
        }

        // GET: Receta/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receta = await _context.Receta.FindAsync(id);
            if (receta == null)
            {
                return NotFound();
            }
            ViewData["DiagnosticoId"] = new SelectList(_context.Diagnostico, "DiagnosticoId", "DiagnosticoId", receta.DiagnosticoId);
            ViewData["IdUsuario"] = new SelectList(_context.Usuario, "Id", "Id", receta.IdUsuario);
            ViewData["PacienteId"] = new SelectList(_context.Paciente, "PacienteId", "PacienteId", receta.PacienteId);
            return View(receta);
        }

        // POST: Receta/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RecetaId,Fecha,PacienteId,IdUsuario,DiagnosticoId")] Receta receta)
        {
            if (id != receta.RecetaId)
            {
                return NotFound();
            }

            ModelState.Remove("IdUsuarioNavigation");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(receta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecetaExists(receta.RecetaId))
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
            ViewData["DiagnosticoId"] = new SelectList(_context.Diagnostico, "DiagnosticoId", "DiagnosticoId", receta.DiagnosticoId);
            ViewData["IdUsuario"] = new SelectList(_context.Usuario, "Id", "Id", receta.IdUsuario);
            ViewData["PacienteId"] = new SelectList(_context.Paciente, "PacienteId", "PacienteId", receta.PacienteId);
            return View(receta);
        }

        // GET: Receta/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receta = await _context.Receta
                .Include(r => r.Diagnostico)
                .Include(r => r.IdUsuarioNavigation)
                .Include(r => r.Paciente)
                .FirstOrDefaultAsync(m => m.RecetaId == id);
            if (receta == null)
            {
                return NotFound();
            }

            return View(receta);
        }

        // POST: Receta/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var receta = await _context.Receta.FindAsync(id);
            if (receta != null)
            {
                _context.Receta.Remove(receta);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecetaExists(int id)
        {
            return _context.Receta.Any(e => e.RecetaId == id);
        }

        //Método para crear la RECETA MEDICA
        public IActionResult RecetaView(int recetaId)
        {
            var receta = _context.Receta
                .Include(r => r.Paciente) // Incluye al paciente
                .Include(r => r.IdUsuarioNavigation) // Incluye al médico
                .Include(r => r.Diagnostico) // Incluye al diagnóstico
                .Include(r => r.RecetaMedicamento) // Incluye los medicamentos recetados
                    .ThenInclude(rm => rm.Medicamento) // Incluye los detalles de los medicamentos
                .FirstOrDefault(r => r.RecetaId == recetaId);

            if (receta == null)
            {
                return NotFound();
            }
            if (receta.Paciente == null)
            {
                ViewData["ErrorPaciente"] = "No se encontró el paciente relacionado con la receta.";
            }

            if (receta.IdUsuarioNavigation == null)
            {
                ViewData["ErrorUsuario"] = "No se encontró el médico relacionado con la receta.";
            }

            if (receta.Diagnostico == null)
            {
                ViewData["ErrorDiagnostico"] = "No se encontró un diagnóstico relacionado.";
            }

            if (!receta.RecetaMedicamento.Any())
            {
                ViewData["ErrorMedicamentos"] = "No se encontraron medicamentos relacionados con la receta.";
            }


            return View(receta);
        }




    }
}

