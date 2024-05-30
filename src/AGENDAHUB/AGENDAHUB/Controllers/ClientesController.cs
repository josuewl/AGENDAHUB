﻿using AGENDAHUB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AGENDAHUB.Controllers
{
    [Authorize]
    public class ClientesController : Controller
    {
        private readonly AppDbContext _context;

        public ClientesController(AppDbContext context)
        {
            _context = context;
        }

        //Função para visualizar a página de clientes
        public async Task<IActionResult> Index(int? page)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Obtém o ID do usuário logado
            var clientes = await _context.Clientes
                .Where(c => c.UsuarioId == int.Parse(userId)) // Restringe os clientes pelo UsuarioID
                .ToListAsync();

            if (clientes == null)
            {
                return NotFound();
            }
            return View(clientes);
        }


        // Função para exibir a tela de Cadastro de Clientes
        public IActionResult Create()
        {
            return View();
        }

        // Resposta HTTP para adicionar um cliente cadastrado no banco de dados e redirecionar para a View
        [HttpPost]
        public async Task<IActionResult> Create(Cliente cliente)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Obtém o ID do usuário logado
            cliente.UsuarioId = int.Parse(userId); // Define o UsuarioID do cliente

            // Verifica se o CPF já está cadastrado
            if (_context.Clientes.Any(c => c.CPF == cliente.CPF))
            {
                ModelState.AddModelError("CPF", "Este CPF já está cadastrado.");
            }

            if (ModelState.IsValid)
            {
                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(cliente);
        }

        //Função para Recuperar os dados do cliente
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Obtém o ID do usuário logado
            var cliente = await _context.Clientes
                .Where(c => c.UsuarioId == int.Parse(userId)) // Restringe os clientes pelo UsuarioID
                .FirstOrDefaultAsync(c => c.IdCliente == id);

            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        // Resposta HTTP para alterar um cliente cadastrado no banco de dados e redirecionar para a View
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Cliente cliente)
        {
            if (id != cliente.IdCliente)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Obtém o ID do usuário logado
            cliente.UsuarioId = int.Parse(userId); // Define o UsuarioID do cliente

            if (ModelState.IsValid)
            {
                _context.Clientes.Update(cliente);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View();
        }

        // Tela de confirmação de exclusão do cliente
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Obtém o ID do usuário logado
            var cliente = await _context.Clientes
                .Where(c => c.UsuarioId == int.Parse(userId)) // Restringe os clientes pelo UsuarioID
                .FirstOrDefaultAsync(c => c.IdCliente == id);

            if (cliente == null)
                return NotFound();

            return View(cliente);
        }

        // Resposta HTTP para excluir um cliente cadastrado no banco de dados e redirecionar para a View
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Obtém o ID do usuário logado
            var cliente = await _context.Clientes
                .Where(c => c.UsuarioId == int.Parse(userId)) // Restringe os clientes pelo UsuarioID
                .FirstOrDefaultAsync(c => c.IdCliente == id);

            if (cliente == null)
                return NotFound();

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}