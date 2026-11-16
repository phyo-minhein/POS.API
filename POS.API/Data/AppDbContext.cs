using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using POS.Models;

namespace POS.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }

    public DbSet<Product> Products { get; set; }

    public DbSet<Sale> Sales { get; set; }

    public DbSet<SaleDetail> SaleDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CatId);

            entity.ToTable("Category");

            entity.Property(e => e.CatName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product", table =>
            {
                table.HasCheckConstraint("CK_Product_Price_NonNegative", "[Price] >= 0");
                table.HasCheckConstraint("CK_Product_StockQuantity_NonNegative", "[StockQuantity] >= 0");
                table.HasCheckConstraint("CK_Product_ReorderLevel_NonNegative", "[ReorderLevel] >= 0");
            });

            entity.HasAlternateKey(e => e.ProductCode)
                .HasName("UQ_Product_ProductCode");

            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ProductCode)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.ProductName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.StockQuantity).HasDefaultValue(0, "DF_Product_StockQuantity");
            entity.Property(e => e.ReorderLevel).HasDefaultValue(5, "DF_Product_ReorderLevel");

            entity.HasOne(d => d.Cat).WithMany(p => p.Products)
                .HasForeignKey(d => d.CatId)
                .HasConstraintName("FK_Product_Category");
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.ToTable("Sale");

            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.InvoiceNo)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<SaleDetail>(entity =>
        {
            entity.HasKey(e => new { e.SaleId, e.Lno });

            entity.ToTable("SaleDetail");

            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Product).WithMany(p => p.SaleDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SaleDetail_Product");

            entity.HasOne(d => d.Sale).WithMany(p => p.SaleDetails)
                .HasForeignKey(d => d.SaleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SaleDetail_Sale");
        });

    }
}
