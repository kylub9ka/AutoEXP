using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AutoService.API.Models;

public partial class User19DbContext : DbContext
{
    public User19DbContext()
    {
    }

    public User19DbContext(DbContextOptions<User19DbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<KursAkcii> KursAkciis { get; set; }

    public virtual DbSet<KursAvtomobili> KursAvtomobilis { get; set; }

    public virtual DbSet<KursIspolniteli> KursIspolnitelis { get; set; }

    public virtual DbSet<KursKlienty> KursKlienties { get; set; }

    public virtual DbSet<KursOtzvy> KursOtzvies { get; set; }

    public virtual DbSet<KursUser> KursUsers { get; set; }

    public virtual DbSet<KursUslugi> KursUslugis { get; set; }

    public virtual DbSet<KursZakazy> KursZakazies { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=stud-mssql.sttec.yar.ru,38325;Database=user19_db;User Id=user19_db;Password=user19;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Cyrillic_General_CI_AS");

        modelBuilder.Entity<KursAkcii>(entity =>
        {
            entity.HasKey(e => e.KodAkcii).HasName("PK__KURS_Akc__40AA2A59DAE8977B");

            entity.ToTable("KURS_Akcii");

            entity.Property(e => e.KodAkcii).HasColumnName("Kod_akcii");
            entity.Property(e => e.Opisanie).HasMaxLength(500);
            entity.Property(e => e.Zagolovok).HasMaxLength(100);
        });

        modelBuilder.Entity<KursAvtomobili>(entity =>
        {
            entity.HasKey(e => e.KodAvto).HasName("PK__KURS_Avt__FF489CA33DC22BD7");

            entity.ToTable("KURS_Avtomobili");

            entity.Property(e => e.KodAvto).HasColumnName("Kod_avto");
            entity.Property(e => e.Gosnomer).HasMaxLength(20);
            entity.Property(e => e.KodKlienta).HasColumnName("Kod_klienta");
            entity.Property(e => e.Marka).HasMaxLength(50);

            entity.HasOne(d => d.KodKlientaNavigation).WithMany(p => p.KursAvtomobilis)
                .HasForeignKey(d => d.KodKlienta)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__KURS_Avto__Kod_k__0856260D");
        });

        modelBuilder.Entity<KursIspolniteli>(entity =>
        {
            entity.HasKey(e => e.KodIspolnitelya).HasName("PK__KURS_Isp__521728B48F99C966");

            entity.ToTable("KURS_Ispolniteli");

            entity.Property(e => e.KodIspolnitelya).HasColumnName("Kod_ispolnitelya");
            entity.Property(e => e.Fio)
                .HasMaxLength(100)
                .HasColumnName("FIO");
            entity.Property(e => e.Specializaciya).HasMaxLength(100);
        });

        modelBuilder.Entity<KursKlienty>(entity =>
        {
            entity.HasKey(e => e.KodKlienta).HasName("PK__KURS_Kli__8A204F4BC59334E2");

            entity.ToTable("KURS_Klienty");

            entity.Property(e => e.KodKlienta).HasColumnName("Kod_klienta");
            entity.Property(e => e.Fio)
                .HasMaxLength(100)
                .HasColumnName("FIO");
            entity.Property(e => e.Telefon).HasMaxLength(20);
        });

        modelBuilder.Entity<KursOtzvy>(entity =>
        {
            entity.HasKey(e => e.KodOtzyva).HasName("PK__KURS_Otz__3DF500E11F5ECE87");

            entity.ToTable("KURS_Otzvy");

            entity.Property(e => e.KodOtzyva).HasColumnName("Kod_otzyva");
            entity.Property(e => e.DataOtzyva)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Data_otzyva");
            entity.Property(e => e.KodIspolnitelya).HasColumnName("Kod_ispolnitelya");
            entity.Property(e => e.KodKlienta).HasColumnName("Kod_klienta");
            entity.Property(e => e.TextOtzyva)
                .HasMaxLength(500)
                .HasColumnName("Text_otzyva");

            entity.HasOne(d => d.KodIspolnitelyaNavigation).WithMany(p => p.KursOtzvies)
                .HasForeignKey(d => d.KodIspolnitelya)
                .HasConstraintName("FK__KURS_Otzv__Kod_i__1C5D1EBA");

            entity.HasOne(d => d.KodKlientaNavigation).WithMany(p => p.KursOtzvies)
                .HasForeignKey(d => d.KodKlienta)
                .HasConstraintName("FK__KURS_Otzv__Kod_k__1B68FA81");
        });

        modelBuilder.Entity<KursUser>(entity =>
        {
            entity.HasKey(e => e.KodUser).HasName("PK__KURS_Use__1F99BF7E535A1185");

            entity.ToTable("KURS_Users");

            entity.HasIndex(e => e.Login, "UQ__KURS_Use__5E55825B34B81667").IsUnique();

            entity.Property(e => e.KodUser).HasColumnName("Kod_user");
            entity.Property(e => e.KodIspolnitelya).HasColumnName("Kod_ispolnitelya");
            entity.Property(e => e.KodKlienta).HasColumnName("Kod_klienta");
            entity.Property(e => e.Login).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.Role).HasMaxLength(20);
        });

        modelBuilder.Entity<KursUslugi>(entity =>
        {
            entity.HasKey(e => e.KodUslugi).HasName("PK__KURS_Usl__315178C2E93149C3");

            entity.ToTable("KURS_Uslugi");

            entity.Property(e => e.KodUslugi).HasColumnName("Kod_uslugi");
            entity.Property(e => e.Cena).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Nazvanie).HasMaxLength(100);
        });

        modelBuilder.Entity<KursZakazy>(entity =>
        {
            entity.HasKey(e => e.KodZakaza).HasName("PK__KURS_Zak__EE2A099C2AD4CE1B");

            entity.ToTable("KURS_Zakazy");

            entity.Property(e => e.KodZakaza).HasColumnName("Kod_zakaza");
            entity.Property(e => e.DataZakaza)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Data_zakaza");
            entity.Property(e => e.ItogovayaStoimost)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Itogovaya_stoimost");
            entity.Property(e => e.KodAvto).HasColumnName("Kod_avto");
            entity.Property(e => e.KodIspolnitelya).HasColumnName("Kod_ispolnitelya");
            entity.Property(e => e.KodKlienta).HasColumnName("Kod_klienta");
            entity.Property(e => e.KodUslugi).HasColumnName("Kod_uslugi");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("В работе");

            entity.HasOne(d => d.KodAvtoNavigation).WithMany(p => p.KursZakazies)
                .HasForeignKey(d => d.KodAvto)
                .HasConstraintName("FK__KURS_Zaka__Kod_a__0FF747D5");

            entity.HasOne(d => d.KodIspolnitelyaNavigation).WithMany(p => p.KursZakazies)
                .HasForeignKey(d => d.KodIspolnitelya)
                .HasConstraintName("FK__KURS_Zaka__Kod_i__11DF9047");

            entity.HasOne(d => d.KodKlientaNavigation).WithMany(p => p.KursZakazies)
                .HasForeignKey(d => d.KodKlienta)
                .HasConstraintName("FK__KURS_Zaka__Kod_k__0F03239C");

            entity.HasOne(d => d.KodUslugiNavigation).WithMany(p => p.KursZakazies)
                .HasForeignKey(d => d.KodUslugi)
                .HasConstraintName("FK__KURS_Zaka__Kod_u__10EB6C0E");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
