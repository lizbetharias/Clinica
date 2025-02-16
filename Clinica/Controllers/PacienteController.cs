﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Clinica.Models;

namespace Clinica.Controllers
{
    public class PacienteController : Controller
    {
        private readonly BDContext _context;

        public PacienteController(BDContext context)
        {
            _context = context;
        }
        // GET: Paciente
        public async Task<IActionResult> Index(string searchString)
        {
            // Consulta inicial
            var pacientes = from p in _context.Paciente
                            select p;

            // Aplicar filtro si se recibe un término de búsqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                pacientes = pacientes.Where(p => p.Nombre.Contains(searchString) ||
                                                 p.Apellido.Contains(searchString));
            }

            return View(await pacientes.ToListAsync());
        }


        // GET: Paciente/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paciente = await _context.Paciente
                .FirstOrDefaultAsync(m => m.PacienteId == id);
            if (paciente == null)
            {
                return NotFound();
            }

            return View(paciente);
        }

        // GET: Paciente/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Paciente/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PacienteId,Nombre,Apellido,FechaNacimiento,Direccion,Telefono,Email")] Paciente paciente)
        {
            if (ModelState.IsValid)
            {
                _context.Add(paciente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(paciente);
        }

        // GET: Paciente/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paciente = await _context.Paciente.FindAsync(id);
            if (paciente == null)
            {
                return NotFound();
            }
            return View(paciente);
        }

        // POST: Paciente/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PacienteId,Nombre,Apellido,FechaNacimiento,Direccion,Telefono,Email")] Paciente paciente)
        {
            if (id != paciente.PacienteId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(paciente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PacienteExists(paciente.PacienteId))
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
            return View(paciente);
        }

        // GET: Paciente/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paciente = await _context.Paciente
                .FirstOrDefaultAsync(m => m.PacienteId == id);
            if (paciente == null)
            {
                return NotFound();
            }

            return View(paciente);
        }

        // POST: Paciente/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var paciente = await _context.Paciente.FindAsync(id);
                if (paciente != null)
                {
                    _context.Paciente.Remove(paciente);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));

            }
            catch (DbUpdateException)
            {
                // Captura la excepción específica de clave foránea
                TempData["ErrorMessage"] = "No se puede eliminar este  Paciente ";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                // Captura cualquier otro error inesperado
                TempData["ErrorMessage"] = "Ocurrió un error inesperado al intentar eliminar el diagnóstico.";
                return RedirectToAction(nameof(Index));
            }
        }

            private bool PacienteExists(int id)
        {
            return _context.Paciente.Any(e => e.PacienteId == id);
        }
    }
}
