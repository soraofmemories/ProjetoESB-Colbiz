using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ProjetoESB.Dominio.Entidades;
namespace ProjetoESB.Infra.Contexts;

public partial class ESBContext : DbContext
{
    public ESBContext()
    {
    }

    public ESBContext(DbContextOptions<ESBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Conector> Conectores { get; set; }
    public virtual DbSet<Integracao> Integracoes { get; set; }

    public virtual DbSet<LogExecucao> LogsExecucao { get; set; }

    public virtual DbSet<Orquestracao> Orquestracoes { get; set; }

    public virtual DbSet<PassoOrquestracao> PassosOrquestracao { get; set; }

    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    modelBuilder.Entity<Conector>(entity =>
    //    {
    //        entity.HasKey(e => e.ConectorId).HasName("PK__Conector__E863D9DE369A95E4");

    //        entity.Property(e => e.ConectorId)
    //            .ValueGeneratedNever()
    //            .HasColumnName("ConectorID");
    //        entity.Property(e => e.Endpoint)
    //            .HasMaxLength(255)
    //            .IsUnicode(false);
    //        entity.Property(e => e.IntegracaoId).HasColumnName("IntegracaoID");
    //        entity.Property(e => e.Metodo)
    //            .HasMaxLength(10)
    //            .IsUnicode(false);
    //        entity.Property(e => e.Tipo)
    //            .HasMaxLength(50)
    //            .IsUnicode(false);

    //        entity.HasOne(d => d.Integracao).WithMany(p => p.Conectores)
    //            .HasForeignKey(d => d.IntegracaoId)
    //            .HasConstraintName("FK__Conector__Integ__2FEF161B");
    //    });


    //    modelBuilder.Entity<Integracao>(entity =>
    //    {
    //        entity.HasKey(e => e.IntegracaoId).HasName("PK__Integrac__9A43BF3D93DD8CE8");

    //        entity.Property(e => e.IntegracaoId)
    //            .ValueGeneratedNever()
    //            .HasColumnName("IntegracaoID");
    //        entity.Property(e => e.Descricao)
    //            .HasMaxLength(255)
    //            .IsUnicode(false);
    //        entity.Property(e => e.Nome)
    //            .HasMaxLength(100)
    //            .IsUnicode(false);
    //    });

    //    modelBuilder.Entity<LogExecucao>(entity =>
    //    {
    //        entity.HasKey(e => e.LogId).HasName("PK__LogExec__5E5499A8CDC180C3");

    //        entity.ToTable("LogsExecucao");

    //        entity.Property(e => e.LogId)
    //            .ValueGeneratedNever()
    //            .HasColumnName("LogID");
    //        entity.Property(e => e.DataHora).HasColumnType("datetime");
    //        entity.Property(e => e.IntegracaoId).HasColumnName("IntegracaoID");
    //        entity.Property(e => e.Mensagem).IsUnicode(false);
    //        entity.Property(e => e.Status)
    //            .HasMaxLength(20)
    //            .IsUnicode(false);
    //    });

    //    modelBuilder.Entity<Orquestracao>(entity =>
    //    {
    //        entity.HasKey(e => e.OrquestracaoId).HasName("PK__Orquestr__378B0F5726E447A8");

    //        entity.Property(e => e.OrquestracaoId)
    //            .ValueGeneratedNever()
    //            .HasColumnName("OrquestracaoID");
    //        entity.Property(e => e.Descricao)
    //            .HasMaxLength(255)
    //            .IsUnicode(false);
    //        entity.Property(e => e.Nome)
    //            .HasMaxLength(100)
    //            .IsUnicode(false);
    //    });

    //    modelBuilder.Entity<PassoOrquestracao>(entity =>
    //    {
    //        entity.HasKey(e => e.PassoId).HasName("PK__PassoOrq__6E5AF5CC43755C90");

    //        entity.ToTable("PassosOrquestracao");

    //        entity.Property(e => e.PassoId)
    //            .ValueGeneratedNever()
    //            .HasColumnName("PassoID");
    //        entity.Property(e => e.ConectorId).HasColumnName("ConectorID");
    //        entity.Property(e => e.OrquestracaoId).HasColumnName("OrquestracaoID");

    //        entity.HasOne(d => d.Conector).WithMany(p => p.PassosOrquestracaos)
    //            .HasForeignKey(d => d.ConectorId)
    //            .HasConstraintName("FK__PassoOrq__Conec__35A7EF71");

    //        entity.HasOne(d => d.Orquestracao).WithMany(p => p.PassosOrquestracao)
    //            .HasForeignKey(d => d.OrquestracaoId)
    //            .HasConstraintName("FK__PassosOrq__Orque__34B3CB38");
    //    });
    //}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //
        // 🔹 Mapeamento das tabelas — garante que os nomes coincidam com o SQL Server
        //
        modelBuilder.Entity<Integracao>().ToTable("Integracoes");
        modelBuilder.Entity<Conector>().ToTable("Conectores");
        modelBuilder.Entity<Orquestracao>().ToTable("Orquestracoes");
        modelBuilder.Entity<PassoOrquestracao>().ToTable("PassosOrquestracao");
        modelBuilder.Entity<LogExecucao>().ToTable("LogsExecucao");

        //
        // 🔹 Configuração de chaves primárias
        //
        modelBuilder.Entity<Integracao>().HasKey(i => i.IntegracaoId);
        modelBuilder.Entity<Conector>().HasKey(c => c.ConectorId);
        modelBuilder.Entity<Orquestracao>().HasKey(o => o.OrquestracaoId);
        modelBuilder.Entity<PassoOrquestracao>().HasKey(p => p.PassoId);
        modelBuilder.Entity<LogExecucao>().HasKey(l => l.LogId);

        //
        // 🔹 Relacionamentos
        //

        // Integracao (1) -> (N) Conectores
        modelBuilder.Entity<Conector>()
            .HasOne(c => c.Integracao)
            .WithMany(i => i.Conectores)
            .HasForeignKey(c => c.IntegracaoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Orquestracao (1) -> (N) Passos
        modelBuilder.Entity<PassoOrquestracao>()
            .HasOne(p => p.Orquestracao)
            .WithMany(o => o.PassosOrquestracao)
            .HasForeignKey(p => p.OrquestracaoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Conector (1) -> (N) Passos
        modelBuilder.Entity<PassoOrquestracao>()
            .HasOne(p => p.Conector)
            .WithMany(c => c.PassosOrquestracaos)
            .HasForeignKey(p => p.ConectorId)
            .OnDelete(DeleteBehavior.Restrict);

        //
        // 🔹 Tamanho dos campos e constraints simples
        //
        modelBuilder.Entity<Integracao>(entity =>
        {
            entity.Property(e => e.Nome).HasMaxLength(100);
            entity.Property(e => e.Descricao).HasMaxLength(255);
        });

        modelBuilder.Entity<Conector>(entity =>
        {
            entity.Property(e => e.Tipo).HasMaxLength(50);
            entity.Property(e => e.Endpoint).HasMaxLength(255);
            entity.Property(e => e.Metodo).HasMaxLength(10);
        });

        modelBuilder.Entity<Orquestracao>(entity =>
        {
            entity.Property(e => e.Nome).HasMaxLength(100);
            entity.Property(e => e.Descricao).HasMaxLength(255);
        });

        modelBuilder.Entity<LogExecucao>(entity =>
        {
            entity.Property(e => e.Status).HasMaxLength(20);
        });
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
