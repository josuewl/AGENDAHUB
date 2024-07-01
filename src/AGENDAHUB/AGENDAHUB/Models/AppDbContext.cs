using Microsoft.EntityFrameworkCore;

namespace AGENDAHUB.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Servico> Servicos { get; set; }
        public DbSet<Agendamento> Agendamentos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Profissional> Profissionais { get; set; }
        public DbSet<Configuracao> Configuracao { get; set; }
        public DbSet<Caixa> Caixa { get; set; }
        public DbSet<ServicoProfissional> ServicoProfissional { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.NomeUsuario)
                .IsUnique();

            // Configuração do relacionamento um-para-um entre Usuario e Configuracao
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Configuracao)
                .WithOne(c => c.Usuario)
                .HasForeignKey<Configuracao>(c => c.UsuarioID);

            modelBuilder.Entity<Agendamento>()
                .HasOne(a => a.Caixa)
                .WithOne(c => c.Agendamento)
                .HasForeignKey<Caixa>(c => c.ID_Agendamento);

            modelBuilder.Entity<Configuracao>()
                .Property(c => c.DiasDaSemanaJson)
                .IsRequired();

            modelBuilder.Entity<Configuracao>()
                .Property(c => c.HoraInicio)
                .IsRequired();

            modelBuilder.Entity<ServicoProfissional>()
                .HasKey(sp => new { sp.ID_Servico, sp.ID_Profissional });

            modelBuilder.Entity<ServicoProfissional>()
                .HasOne(sp => sp.Servico)
                .WithMany(s => s.ServicosProfissionais)
                .HasForeignKey(sp => sp.ID_Servico);

            modelBuilder.Entity<ServicoProfissional>()
                .HasOne(sp => sp.Profissional)
                .WithMany(p => p.ServicosProfissionais)
                .HasForeignKey(sp => sp.ID_Profissional);

            // Corrigindo múltiplos caminhos de cascata removendo ou ajustando deletar em cascata
            modelBuilder.Entity<Agendamento>()
                .HasOne(a => a.Cliente)
                .WithMany(c => c.Agendamento)
                .HasForeignKey(a => a.IdCliente)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Agendamento>()
                .HasOne(a => a.Servico)
                .WithMany(s => s.Agendamento)
                .HasForeignKey(a => a.ID_Servico)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Agendamento>()
                .HasOne(a => a.Profissional)
                .WithMany(p => p.Agendamento)
                .HasForeignKey(a => a.ID_Profissional)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
