using AGENDAHUB.Models;
using FluentAssertions.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AGENDAHUB.Controllers
{
    [Authorize]
    public class ConfiguracaoController : Controller
    {
        private readonly AppDbContext _context;

        public ConfiguracaoController(AppDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var usuario = await _context.Usuarios
                .Include(u => u.Configuracao)
                .FirstOrDefaultAsync(u => u.Id.ToString() == userId);

            if (usuario == null)
            {
                return NotFound();
            }

            // Verifique se Configuracao não é nulo
            if (usuario.Configuracao == null)
            {
                usuario.Configuracao = new Configuracao();
            }

            // Verifique se Imagem não é nulo
            ViewBag.HasExistingImage = (usuario.Configuracao.Imagem != null && usuario.Configuracao.Imagem.Length > 0);

            var viewModel = new ConfiguracaoUsuarioViewModel
            {
                Usuario = usuario,
                Configuracao = usuario.Configuracao
            };

            return View(viewModel);
        }


        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return 0;
        }

        // Imagem
        public FileContentResult GetImg(int id)
        {
            var configuracao = _context.Configuracao.FirstOrDefault(c => c.UsuarioID == id);
            byte[] byteArray = configuracao?.Imagem;
            return byteArray != null
                ? new FileContentResult(byteArray, "image/jpeg")
                : null;
        }


        [HttpPost]
        public async Task<IActionResult> RemoveImg()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(int.Parse(userId));

            if (usuario == null)
            {
                return NotFound();
            }

            try
            {
                usuario.Configuracao.Imagem = null;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erro ao remover a imagem: {ex.Message}";
            }

            return RedirectToAction("Index", "Configuracao");
        }


        [HttpGet]
        public IActionResult CreateImg()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateImg(IFormFile file)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }

            var configuracao = await _context.Configuracao.FindAsync(int.Parse(userId));

            if (configuracao == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (file != null && file.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    await file.CopyToAsync(memoryStream);
                    byte[] data = memoryStream.ToArray();
                    configuracao.Imagem = data;
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Configuracao");
            }

            return View(configuracao);
        }

        [HttpGet]
        public IActionResult Edit()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }

            var usuario = _context.Usuarios.Include(u => u.Configuracao).FirstOrDefault(u => u.Id.ToString() == userId);

            if (usuario == null)
            {
                return NotFound();
            }

            // Criar o ViewModel e passá-lo para a view
            var viewModel = new ConfiguracaoUsuarioViewModel
            {
                Usuario = usuario,
                Configuracao = usuario.Configuracao
            };

            ViewBag.HasExistingImage = (usuario.Configuracao.Imagem != null && usuario.Configuracao.Imagem.Length > 0);

            return View(viewModel);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ConfiguracaoUsuarioViewModel viewModel, IFormFile Imagem)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (ModelState.IsValid)
            {
                try
                {
                    // Carrega o usuário existente do banco de dados
                    var usuarioNoBanco = await _context.Usuarios
                        .Include(u => u.Configuracao)
                        .FirstOrDefaultAsync(u => u.Id.ToString() == userId);

                    if (usuarioNoBanco == null)
                    {
                        return NotFound();
                    }

                    // Atualiza as propriedades do usuário existente com as novas informações
                    usuarioNoBanco.NomeUsuario = viewModel.Usuario.NomeUsuario;
                    usuarioNoBanco.Email = viewModel.Usuario.Email;
                    usuarioNoBanco.Perfil = viewModel.Usuario.Perfil;



                    // Verifica se a senha foi alterada
                    if (!string.IsNullOrEmpty(viewModel.Usuario.Senha))
                    {
                        if (BCrypt.Net.BCrypt.Verify(viewModel.Usuario.Senha, usuarioNoBanco.Senha))
                        {
                            // A senha fornecida corresponde à senha atual no banco de dados,
                            // portanto, é seguro atualizar o email sem a necessidade de reautenticação
                            // Atualiza o email
                            usuarioNoBanco.Email = viewModel.Usuario.Email;
                        }
                        else
                        {
                            // A senha fornecida não corresponde à senha atual no banco de dados,
                            // portanto, requer autenticação adicional
                            // Atribua o ModelState.IsNotValid para exibir a mensagem de erro adequada
                            TempData["SenhaIncorreta"] = "Senha incorreta.";
                            return RedirectToAction("Index", "Configuracao");
                        }
                    }

                    // Se a senha foi alterada, re-hasha a nova senha
                    if (!string.IsNullOrEmpty(viewModel.Usuario.Senha))
                    {
                        usuarioNoBanco.Senha = BCrypt.Net.BCrypt.HashPassword(viewModel.Usuario.Senha);
                    }

                    // Atualiza o estado do usuário existente para modificado
                    _context.Entry(usuarioNoBanco).State = EntityState.Modified;

                    // Salva as alterações no banco de dados
                    await _context.SaveChangesAsync();

                    // Redireciona para a ação de sucesso
                    return RedirectToAction("Index", "Configuracao");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(viewModel.Usuario.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // Se houver erros de validação, retorne para a View com os dados existentes
            return View(viewModel);
        }





        [HttpGet]
        [ActionName("EditInformacoesEmpresariais")]
        public IActionResult EditInformacoesEmpresariais()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var usuario = _context.Usuarios.Include(u => u.Configuracao).FirstOrDefault(u => u.Id.ToString() == userId);

            if (usuario == null)
            {
                return NotFound();
            }

            ViewBag.HasExistingImage = (usuario.Configuracao.Imagem != null && usuario.Configuracao.Imagem.Length > 0);

            return View(usuario.Configuracao);
        }

        [HttpPost]
        [ActionName("EditInformacoesEmpresariais")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditInformacoesEmpresariais([FromForm] Configuracao configuracao, IFormFile Imagem)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdString))
            {
                return NotFound();
            }

            if (!int.TryParse(userIdString, out int userId))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var usuarioNoBanco = await _context.Usuarios
                        .Include(u => u.Configuracao)
                        .FirstOrDefaultAsync(u => u.Id == userId);

                    if (usuarioNoBanco == null)
                    {
                        return NotFound();
                    }

                    // Verifique se Configuracao não é nulo
                    if (usuarioNoBanco.Configuracao == null)
                    {
                        usuarioNoBanco.Configuracao = new Configuracao();
                    }

                    usuarioNoBanco.Configuracao.NomeEmpresa = configuracao.NomeEmpresa;
                    usuarioNoBanco.Configuracao.Cnpj = configuracao.Cnpj;
                    usuarioNoBanco.Configuracao.Email = configuracao.Email;
                    usuarioNoBanco.Configuracao.Endereco = configuracao.Endereco;

                    // Se a imagem é fornecida, atualize-a
                    if (Imagem != null && Imagem.Length > 0)
                    {
                        using var memoryStream = new MemoryStream();
                        await Imagem.CopyToAsync(memoryStream);
                        usuarioNoBanco.Configuracao.Imagem = memoryStream.ToArray();
                    }

                    // Salva as alterações no banco de dados
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index", "Configuracao");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(userId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(configuracao);
        }


        [HttpGet]
        [ActionName("EditDiasAtendimento")]
        public IActionResult EditDiasAtendimento()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var usuario = _context.Usuarios.Include(u => u.Configuracao).FirstOrDefault(u => u.Id.ToString() == userId);

            if (usuario == null)
            {
                return NotFound();
            }

            

            return View(usuario.Configuracao);
        }

        [HttpPost]
        [ActionName("EditDiasAtendimento")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDiasAtendimento([FromForm] Configuracao configuracao)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (ModelState.IsValid)
            {
                try
                {

                    var usuarioNoBanco = await _context.Usuarios.Include(u => u.Configuracao)
                                                         .FirstOrDefaultAsync(u => u.Id.ToString() == userId);

                    if (usuarioNoBanco == null)
                    {
                        return NotFound();
                    }

                    // Verifique se Configuracao não é nulo
                    if (usuarioNoBanco.Configuracao == null)
                    {
                        usuarioNoBanco.Configuracao = new Configuracao();
                    }


                    usuarioNoBanco.Configuracao.DiaAtendimento = configuracao.DiaAtendimento;
                    usuarioNoBanco.Configuracao.HoraInicio = configuracao.HoraInicio;
                    usuarioNoBanco.Configuracao.HoraFim = configuracao.HoraFim;

                    

                    // Salva as alterações no banco de dados
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index", "Configuracao");
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(configuracao.UsuarioID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(configuracao);
        }








      /*
        [HttpGet]
        [ActionName("EditInforAtendimento")]
        public IActionResult EditInforAtendimento()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var usuario = _context.Usuarios.Include(u => u.Configuracao).FirstOrDefault(u => u.Id.ToString() == userId);

            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario.Configuracao);
        }



        [HttpPost]
        [ActionName("EditInforAtendimento")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditInforAtendimento([FromForm] Configuracao configuracao)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var usuario = _context.Usuarios.Include(u => u.Configuracao).FirstOrDefault(u => u.Id.ToString() == userId);

                    if (usuario.Configuracao == null)
                    {
                        // Se não existir, é uma operação de criação
                        //Cria associando ao UsuarioID
                        configuracao.UsuarioID = int.Parse(userId);
                        _context.Configuracao.Add(configuracao);
                    }
                    else
                    {
                        // Se existir, é uma operação de atualização
                        usuario.Configuracao.DiaAtendimento = configuracao.DiaAtendimento;
                        usuario.Configuracao.HoraInicio = configuracao.HoraInicio;
                        usuario.Configuracao.HoraFim = configuracao.HoraFim;

                        _context.Entry(usuario).State = EntityState.Detached; // Desanexar o objeto existente
                        _context.Entry(usuario.Configuracao).State = EntityState.Modified;
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Configuracao");

                }
                catch (DbUpdateConcurrencyException)
                {
                    ModelState.AddModelError(string.Empty, "Erro de concorrência ao salvar as alterações. Tente novamente.");
                }
            }

            // Se houver erros de validação, retorne para a View com os dados existentes
            return View(configuracao);
        }

        */

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Verifique se ID_Configuracao é nulo antes de prosseguir
            var configuracao = await _context.Configuracao
                .Where(a => a.UsuarioID == int.Parse(userId) && a.ID_Configuracao == id)
                .FirstOrDefaultAsync();

            // Se a configuração ou ID_Configuracao for nulo, retorne NotFound
            if (configuracao == null)
            {
                return NotFound();
            }

            return View(configuracao);
        }



        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var usuario = _context.Usuarios
                .Include(u => u.Configuracao)
                .FirstOrDefault(u => u.Id.ToString() == userId);

            if (usuario == null || usuario.Configuracao == null)
            {
                return NotFound();
            }

            // Verifique se o ID da configuração corresponde ao ID fornecido
            if (usuario.Configuracao.ID_Configuracao != id)
            {
                return NotFound();
            }

            _context.Configuracao.Remove(usuario.Configuracao);
            await _context.SaveChangesAsync();

            return RedirectToAction("Edit");
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }

    }
}