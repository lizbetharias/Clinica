using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Clinica.Models;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

namespace Clinica.Controllers
{
    public class RecetaController : Controller
    {
        private readonly BDContext _context;

        // Constructor: Inyección de dependencia para el contexto de base de datos
        public RecetaController(BDContext context)
        {
            _context = context;
        }

        // GET: Receta (Vista principal con la lista de recetas)
        public async Task<IActionResult> Index()
        {
            var recetas = _context.Receta
                .Include(r => r.Diagnostico)
                .Include(r => r.Usuario) // Relación corregida
                .Include(r => r.Paciente);
            return View(await recetas.ToListAsync());
        }

        // GET: Receta/Details/5 (Ver detalles de una receta específica)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receta = await _context.Receta
                .Include(r => r.Diagnostico)
                .Include(r => r.Usuario)
                .Include(r => r.Paciente)
                .FirstOrDefaultAsync(m => m.RecetaId == id);

            if (receta == null)
            {
                return NotFound();
            }

            return View(receta);
        }

        // GET: Receta/Create (Formulario para crear una nueva receta)
        public IActionResult Create()
        {
            ViewData["DiagnosticoId"] = new SelectList(_context.Diagnostico, "DiagnosticoId", "Descripcion");
            ViewData["IdUsuario"] = new SelectList(_context.Usuario, "Id", "Nombre");
            ViewData["PacienteId"] = new SelectList(_context.Paciente, "PacienteId", "Nombre");
            return View();
        }

        // POST: Receta/Create (Guardar la nueva receta en la base de datos)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RecetaId,Fecha,PacienteId,IdUsuario,DiagnosticoId")] Receta receta)
        {
            ModelState.Remove("Usuario"); // Para evitar validaciones redundantes
            if (ModelState.IsValid)
            {
                _context.Add(receta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Volver a cargar las listas en caso de que haya un error
            ViewData["DiagnosticoId"] = new SelectList(_context.Diagnostico, "DiagnosticoId", "Descripcion", receta.DiagnosticoId);
            ViewData["IdUsuario"] = new SelectList(_context.Usuario, "Id", "Nombre", receta.IdUsuario);
            ViewData["PacienteId"] = new SelectList(_context.Paciente, "PacienteId", "Nombre", receta.PacienteId);
            return View(receta);
        }

        // GET: Receta/Edit/5 (Formulario para editar una receta existente)
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

            ViewData["DiagnosticoId"] = new SelectList(_context.Diagnostico, "DiagnosticoId", "Descripcion", receta.DiagnosticoId);
            ViewData["IdUsuario"] = new SelectList(_context.Usuario, "Id", "Nombre", receta.IdUsuario);
            ViewData["PacienteId"] = new SelectList(_context.Paciente, "PacienteId", "Nombre", receta.PacienteId);
            return View(receta);
        }

        // POST: Receta/Edit/5 (Guardar los cambios en la base de datos)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RecetaId,Fecha,PacienteId,IdUsuario,DiagnosticoId")] Receta receta)
        {
            if (id != receta.RecetaId)
            {
                return NotFound();
            }

            ModelState.Remove("Usuario");
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

            ViewData["DiagnosticoId"] = new SelectList(_context.Diagnostico, "DiagnosticoId", "Descripcion", receta.DiagnosticoId);
            ViewData["IdUsuario"] = new SelectList(_context.Usuario, "Id", "Nombre", receta.IdUsuario);
            ViewData["PacienteId"] = new SelectList(_context.Paciente, "PacienteId", "Nombre", receta.PacienteId);
            return View(receta);
        }

        // GET: Receta/Delete/5 (Confirmación para eliminar una receta)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receta = await _context.Receta
                .Include(r => r.Diagnostico)
                .Include(r => r.Usuario)
                .Include(r => r.Paciente)
                .FirstOrDefaultAsync(m => m.RecetaId == id);

            if (receta == null)
            {
                return NotFound();
            }

            return View(receta);
        }

        // POST: Receta/Delete/5 (Eliminar la receta confirmada)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var receta = await _context.Receta.FindAsync(id);
            if (receta != null)
            {
                _context.Receta.Remove(receta);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Método privado para verificar si una receta existe
        private bool RecetaExists(int id)
        {
            return _context.Receta.Any(e => e.RecetaId == id);
        }

        // GET: Método para generar el PDF de una receta médica
        [HttpGet]
        public IActionResult ImprimirReceta(int recetaid)
        {
            var receta = _context.Receta
                .Include(r => r.Paciente)
                .Include(r => r.Usuario)
                .Include(r => r.Diagnostico)
                .Include(r => r.RecetaMedicamento)
                .ThenInclude(m => m.Medicamento)
                .Where(r => r.RecetaId == recetaid)
                .Select(r => new
                {
                    Paciente = r.Paciente.Nombre,
                    Medico = r.Usuario.Nombre,
                    Fecha = r.Fecha,
                    Diagnostico = r.Diagnostico.Descripcion,
                    Medicamentos = r.RecetaMedicamento.Select(m => new
                    {
                        Nombre = m.Medicamento.Nombre,
                        Dosis = m.Dosis
                    }).ToList(),
                    Observaciones = r.Observaciones
                })
                .FirstOrDefault();

            if (receta == null)
            {
                return NotFound("Receta no encontrada");
            }

            var stream = new MemoryStream();
            using (var writer = new PdfWriter(stream))
            {
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                document.Add(new Paragraph("Dispensario \"Cristo Crucificado\"").SetBold().SetFontSize(16));
                document.Add(new Paragraph("Receta Médica").SetFontSize(14));
                document.Add(new Paragraph($"Fecha: {receta.Fecha:dd/MM/yyyy}"));
                document.Add(new Paragraph($"Paciente: {receta.Paciente}"));
                document.Add(new Paragraph($"Médico: {receta.Medico}"));
                document.Add(new Paragraph($"Diagnóstico: {receta.Diagnostico}"));
                document.Add(new Paragraph("Medicamentos:"));

                foreach (var medicamento in receta.Medicamentos)
                {
                    document.Add(new Paragraph($"- {medicamento.Nombre}: {medicamento.Dosis}"));
                }

                document.Add(new Paragraph($"Observaciones: {receta.Observaciones}"));

                document.Close();
            }

            stream.Position = 0;
            return File(stream.ToArray(), "application/pdf", "RecetaMedica.pdf");
        }
    }
}
