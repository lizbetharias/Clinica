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
    public class RecetaMedicamentoController : Controller
    {
        private readonly BDContext _context;

        public RecetaMedicamentoController(BDContext context)
        {
            _context = context;
        }

        // GET: RecetaMedicamento
        public async Task<IActionResult> Index()
        {
            var bDContext = _context.RecetaMedicamento.Include(r => r.Medicamento).Include(r => r.Receta);
            return View(await bDContext.ToListAsync());
        }

        // GET: RecetaMedicamento/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recetaMedicamento = await _context.RecetaMedicamento
                .Include(r => r.Medicamento)
                .Include(r => r.Receta)
                .FirstOrDefaultAsync(m => m.RecetaId == id);
            if (recetaMedicamento == null)
            {
                return NotFound();
            }

            return View(recetaMedicamento);
        }

        // GET: RecetaMedicamento/Create
        public IActionResult Create()
        {
            ViewData["MedicamentoId"] = new SelectList(_context.Medicamento, "MedicamentoId", "MedicamentoId");
            ViewData["RecetaId"] = new SelectList(_context.Receta, "RecetaId", "RecetaId");
            return View();
        }

        // POST: RecetaMedicamento/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RecetaId,MedicamentoId,Cantidad,Instrucciones")] RecetaMedicamento recetaMedicamento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(recetaMedicamento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MedicamentoId"] = new SelectList(_context.Medicamento, "MedicamentoId", "MedicamentoId", recetaMedicamento.MedicamentoId);
            ViewData["RecetaId"] = new SelectList(_context.Receta, "RecetaId", "RecetaId", recetaMedicamento.RecetaId);
            return View(recetaMedicamento);
        }

        // GET: RecetaMedicamento/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recetaMedicamento = await _context.RecetaMedicamento.FindAsync(id);
            if (recetaMedicamento == null)
            {
                return NotFound();
            }
            ViewData["MedicamentoId"] = new SelectList(_context.Medicamento, "MedicamentoId", "MedicamentoId", recetaMedicamento.MedicamentoId);
            ViewData["RecetaId"] = new SelectList(_context.Receta, "RecetaId", "RecetaId", recetaMedicamento.RecetaId);
            return View(recetaMedicamento);
        }

        // POST: RecetaMedicamento/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RecetaId,MedicamentoId,Cantidad,Instrucciones")] RecetaMedicamento recetaMedicamento)
        {
            if (id != recetaMedicamento.RecetaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recetaMedicamento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecetaMedicamentoExists(recetaMedicamento.RecetaId))
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
            ViewData["MedicamentoId"] = new SelectList(_context.Medicamento, "MedicamentoId", "MedicamentoId", recetaMedicamento.MedicamentoId);
            ViewData["RecetaId"] = new SelectList(_context.Receta, "RecetaId", "RecetaId", recetaMedicamento.RecetaId);
            return View(recetaMedicamento);
        }

        // GET: RecetaMedicamento/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recetaMedicamento = await _context.RecetaMedicamento
                .Include(r => r.Medicamento)
                .Include(r => r.Receta)
                .FirstOrDefaultAsync(m => m.RecetaId == id);
            if (recetaMedicamento == null)
            {
                return NotFound();
            }

            return View(recetaMedicamento);
        }

        // POST: RecetaMedicamento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recetaMedicamento = await _context.RecetaMedicamento.FindAsync(id);
            if (recetaMedicamento != null)
            {
                _context.RecetaMedicamento.Remove(recetaMedicamento);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecetaMedicamentoExists(int id)
        {
            return _context.RecetaMedicamento.Any(e => e.RecetaId == id);
        }
    }
}
