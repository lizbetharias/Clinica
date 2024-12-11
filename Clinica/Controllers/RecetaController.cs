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
using System.Reflection.Metadata;

using IronPdf;

using Document = System.Reflection.Metadata.Document;


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


        public IActionResult verpdf(int id)
        {
            var receta = _context.Receta
                .Include(r => r.Paciente)
                .Include(r => r.Diagnostico)
                .Include(r => r.RecetaMedicamento)
                    .ThenInclude(rm => rm.Medicamento)
                .FirstOrDefault(r => r.RecetaId == id);

            if (receta == null)
            {
                return NotFound();
            }

            var renderer = new HtmlToPdf();
            var htmlView = RenderRazorViewToString("RecetaView", receta);
            var pdf = renderer.RenderHtmlAsPdf(htmlView);
            return File(pdf.BinaryData, "application/pdf", $"Receta_{id}.pdf");
        }

        private string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = _viewEngine.FindView(ControllerContext, viewName, false);
                var viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    ViewData,
                    TempData,
                    sw,
                    new HtmlHelperOptions()
                );
                viewResult.View.RenderAsync(viewContext);
                return sw.GetStringBuilder().ToString();
            }
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



        //Método RecetaView
        public IActionResult RecetaView(int id)
        {
            var receta = _context.Receta
                .Include(r => r.Paciente)
                .Include(r => r.IdUsuarioNavigation)
                .Include(r => r.Diagnostico)
                .Include(r => r.RecetaMedicamento)
                    .ThenInclude(rm => rm.Medicamento)
                .FirstOrDefault(r => r.RecetaId == id);

            if (receta == null)
            {
                return NotFound();
            }

            // Depuración del modelo
            if (receta.Paciente == null)
            {
                Console.WriteLine("Paciente no encontrado");
            }

            if (receta.IdUsuarioNavigation == null)
            {
                Console.WriteLine("Médico no encontrado");
            }

            if (receta.Diagnostico == null)
            {
                Console.WriteLine("Diagnóstico no encontrado");
            }

            if (!receta.RecetaMedicamento.Any())
            {
                Console.WriteLine("No se encontraron medicamentos recetados");
            }

            return View(receta);
        }

        [HttpGet]
        public JsonResult FiltrarMedicamentos(string term)
        {
            var medicamentosFiltrados = _context.Medicamento
                .Where(p => p.Nombre.Contains(term))
                .Select(p => new
                {
                    id = p.MedicamentoId,
                    nombre = p.Nombre,
                    precio = p.Dosis
                })
                .ToList();

            return Json(medicamentosFiltrados);
        }
    }
}


