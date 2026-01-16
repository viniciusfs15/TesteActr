using Microsoft.EntityFrameworkCore;
using Intf;

namespace Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{
		}

		public DbSet<EmpresaModel> Empresas { get; set; }
		public DbSet<FornecedorModel> Fornecedores { get; set; }
		public DbSet<EmpresaFornecedorModel> EmpresaFornecedores { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<EmpresaModel>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.CreatedAt).IsRequired(false);
				entity.Property(e => e.UpdatedAt).IsRequired(false);
				entity.Property(e => e.CreatedBy).HasMaxLength(100).IsRequired(false);
				entity.Property(e => e.UpdatedBy).HasMaxLength(100).IsRequired(false);
				entity.Property(e => e.Cnpj).IsRequired().HasMaxLength(14);
				entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
				entity.Property(e => e.Cep).IsRequired();
			});

			modelBuilder.Entity<FornecedorModel>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.CreatedAt).IsRequired(false);
				entity.Property(e => e.UpdatedAt).IsRequired(false);
				entity.Property(e => e.CreatedBy).HasMaxLength(100).IsRequired(false);
				entity.Property(e => e.UpdatedBy).HasMaxLength(100).IsRequired(false);
				entity.Property(e => e.Cpf).HasMaxLength(11).IsRequired(false);
				entity.Property(e => e.Cnpj).HasMaxLength(14).IsRequired(false);
				entity.Property(e => e.Rg).HasMaxLength(20).IsRequired(false);
				entity.Property(e => e.Type).IsRequired();
				entity.Property(e => e.BirthDate).IsRequired(false);
				entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
				entity.Property(e => e.Cep).IsRequired();
				entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
			});

			modelBuilder.Entity<EmpresaFornecedorModel>(entity =>
			{
				entity.HasKey(ef => ef.Id);

				// Índice único na combinação EmpresaId e FornecedorId
				entity.HasIndex(ef => new { ef.EmpresaId, ef.FornecedorId })
					.IsUnique();

				entity.Property(ef => ef.CreatedAt).IsRequired(false);
				entity.Property(ef => ef.UpdatedAt).IsRequired(false);
				entity.Property(ef => ef.CreatedBy).HasMaxLength(100).IsRequired(false);
				entity.Property(ef => ef.UpdatedBy).HasMaxLength(100).IsRequired(false);
				entity.Property(ef => ef.EmpresaId).IsRequired();
				entity.Property(ef => ef.FornecedorId).IsRequired();

				// Configurar relacionamentos
				entity.HasOne(ef => ef.Empresa)
					.WithMany(e => e.EmpresaFornecedores)
					.HasForeignKey(ef => ef.EmpresaId)
					.OnDelete(DeleteBehavior.Cascade);

				entity.HasOne(ef => ef.Fornecedor)
					.WithMany(f => f.EmpresaFornecedores)
					.HasForeignKey(ef => ef.FornecedorId)
					.OnDelete(DeleteBehavior.Cascade);
			});
		}
	}
}
