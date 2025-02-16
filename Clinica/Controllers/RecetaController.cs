﻿ using System;
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
using System.Reflection.Metadata;
using Document = System.Reflection.Metadata.Document;

using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;

using System.Security;
using iText.Kernel.Font;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using iText.IO.Font.Constants;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout.Borders;

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
        public IActionResult verPdf(int id)
        {
            try
            {
                // Consulta incluyendo las relaciones necesarias
                var receta = _context.Receta
               .Include(r => r.Paciente)
               .Include(r => r.IdUsuarioNavigation)
               .Include(r => r.RecetaMedicamento)
                  .ThenInclude(rm => rm.Medicamento)
                .Include(r => r.RecetaMedicamento)
               .FirstOrDefault(r => r.RecetaId == id);


                if (receta == null || receta.Paciente == null || receta.IdUsuarioNavigation == null ||
                    receta.RecetaMedicamento == null || !receta.RecetaMedicamento.Any())
                {
                    return Content("Error: La receta o sus datos asociados no están disponibles.");
                }

                using (var memoryStream = new MemoryStream())
                {
                    // Inicializar iText
                    var writer = new PdfWriter(memoryStream, new WriterProperties().SetPdfVersion(PdfVersion.PDF_1_7));
                    var pdf = new PdfDocument(writer);
                    var document = new iText.Layout.Document(pdf);

                    // Configurar márgenes
                    document.SetMargins(50, 50, 50, 50);

                    // Fuentes y estilos
                    var regularFont = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA);
                    var boldFont = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);

                    // Encabezado principal centrado
                    document.Add(new iText.Layout.Element.Paragraph("Clínica \"Cristo Crucificado\"")
                        .SetFont(boldFont)
                        .SetFontSize(16)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));
                    document.Add(new iText.Layout.Element.Paragraph("3ra. Calle Ote. Ba. Las Mercedes Nahuizalco")
                        .SetFontSize(12)
                        .SetFont(regularFont)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));
                    document.Add(new iText.Layout.Element.Paragraph("RECETA MÉDICA")
                        .SetFont(boldFont)
                        .SetFontSize(14)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));

                    // Línea divisoria
                    document.Add(new iText.Layout.Element.LineSeparator(new iText.Kernel.Pdf.Canvas.Draw.SolidLine()));

                    // Fecha centrada
                    document.Add(new iText.Layout.Element.Paragraph($"Fecha: {receta.Fecha:dd/MM/yyyy HH:mm:ss}")
                        .SetFontSize(12)
                        .SetFont(regularFont));

                    // Datos del paciente
                    document.Add(new iText.Layout.Element.Paragraph("\nPaciente:")
                        .SetFontSize(12)
                        .SetFont(boldFont));
                    document.Add(new iText.Layout.Element.Paragraph($"{receta.Paciente.Nombre} {receta.Paciente.Apellido}")
                        .SetFont(regularFont));

                    // Médico tratante
                    document.Add(new iText.Layout.Element.Paragraph("\nMédico:")
                        .SetFontSize(12)
                        .SetFont(boldFont));
                    document.Add(new iText.Layout.Element.Paragraph($"Dr. {receta.IdUsuarioNavigation.Nombre} {receta.IdUsuarioNavigation.Apellido}")
                        .SetFont(regularFont));

                    // Medicamentos
                    document.Add(new iText.Layout.Element.Paragraph("\nMedicamentos:")
                        .SetFontSize(12)
                        .SetFont(boldFont));

                    // Dosis
                    document.Add(new iText.Layout.Element.Paragraph("\nDosis:")
                        .SetFontSize(12)
                        .SetFont(boldFont));

                    foreach (var medicamento in receta.RecetaMedicamento)
                    {
                        document.Add(new iText.Layout.Element.Paragraph($"- {medicamento.Medicamento.Nombre}" + " "+ medicamento.Medicamento.Dosis)
                            .SetFont(regularFont));

                    }

                    // Cerrar el documento
                    document.Close();

                    return File(memoryStream.ToArray(), "application/pdf", "RecetaMedica.pdf");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al generar el PDF: {ex.Message}");
                return Content($"Error al generar el PDF: {ex.Message}");
            }
        }


        // GET: Receta
        public async Task<IActionResult> Index(string searchPaciente)
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
                .Include(r => r.Paciente)
                .FirstOrDefaultAsync(m => m.RecetaId == id);
            if (receta == null)
            {
                return NotFound();
            }

            ViewData["DiagnosticoId"] = new SelectList(_context.Diagnostico, "DiagnosticoId", "Descripcion", receta.DiagnosticoId);
            ViewData["IdUsuario"] = new SelectList(_context.Usuario, "Id", "Nombre", receta.IdUsuario);
            ViewData["PacienteId"] = new SelectList(_context.Paciente, "PacienteId", "Nombre", receta.PacienteId);
            return View(receta);
        }

        // GET: Receta/Creat
        // GET: Receta/Create
        public IActionResult Create()
        {
            // Cargar medicamentos desde la base de datos
            ViewBag.Medicamentos = _context.Medicamento.ToList();

            ViewData["DiagnosticoId"] = new SelectList(_context.Diagnostico, "DiagnosticoId", "Descripcion");
            ViewData["IdUsuario"] = new SelectList(_context.Usuario, "Id", "Nombre");
            ViewData["PacienteId"] = new SelectList(_context.Paciente, "PacienteId", "Nombre");
            return View();
        }

        // POST: Receta/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RecetaId,Fecha,PacienteId,IdUsuario,DiagnosticoId")] Receta receta, List<int> MedicamentosSeleccionados)
        {
            ModelState.Remove("IdUsuarioNavigation");
            if (ModelState.IsValid)
            {
                receta.IdUsuario = Models.Global.IdGlobal;
                // Guardar la receta
                _context.Add(receta);
                await _context.SaveChangesAsync();

                // Asociar los medicamentos seleccionados con la receta
                if (MedicamentosSeleccionados != null && MedicamentosSeleccionados.Any())
                {
                    foreach (var medicamentoId in MedicamentosSeleccionados)
                    {
                        var recetaMedicamento = new RecetaMedicamento
                        {
                            RecetaId = receta.RecetaId,
                            MedicamentoId = medicamentoId
                        };
                        _context.RecetaMedicamento.Add(recetaMedicamento);
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            // Recargar dropdowns y lista de medicamentos en caso de error
            ViewBag.Medicamentos = _context.Medicamento.ToList();
            ViewData["DiagnosticoId"] = new SelectList(_context.Diagnostico, "DiagnosticoId", "Descripcion", receta.DiagnosticoId);
            ViewData["IdUsuario"] = new SelectList(_context.Usuario, "Id", "Nombre", receta.IdUsuario);
            ViewData["PacienteId"] = new SelectList(_context.Paciente, "PacienteId", "Nombre", receta.PacienteId);
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
            ViewData["DiagnosticoId"] = new SelectList(_context.Diagnostico, "DiagnosticoId", "Descripcion", receta.DiagnosticoId);
            ViewData["IdUsuario"] = new SelectList(_context.Usuario, "Id", "Nombre", receta.IdUsuario);
            ViewData["PacienteId"] = new SelectList(_context.Paciente, "PacienteId", "Nombre", receta.PacienteId);
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
            ViewData["DiagnosticoId"] = new SelectList(_context.Diagnostico, "DiagnosticoId", "Descripcion", receta.DiagnosticoId);
            ViewData["IdUsuario"] = new SelectList(_context.Usuario, "Id", "Nombre", receta.IdUsuario);
            ViewData["PacienteId"] = new SelectList(_context.Paciente, "PacienteId", "Nombre", receta.PacienteId);
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
            ViewData["DiagnosticoId"] = new SelectList(_context.Diagnostico, "DiagnosticoId", "Descripcion", receta.DiagnosticoId);
            ViewData["IdUsuario"] = new SelectList(_context.Usuario, "Id", "Nombre", receta.IdUsuario);
            ViewData["PacienteId"] = new SelectList(_context.Paciente, "PacienteId", "Nombre", receta.PacienteId);
            return View(receta);
        }

        // POST: Receta/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var receta = await _context.Receta.FindAsync(id);
                if (receta != null)
                {
                    _context.Receta.Remove(receta);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                // Captura la excepción específica de clave foránea
                TempData["ErrorMessage"] = "No se puede eliminar esta Receta porque está asociada con otros registros.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                // Captura cualquier otro error inesperado
                TempData["ErrorMessage"] = "Ocurrió un error inesperado al intentar eliminar la Receta.";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool RecetaExists(int id)
        {
            return _context.Receta.Any(e => e.RecetaId == id);
        }

    }
}


