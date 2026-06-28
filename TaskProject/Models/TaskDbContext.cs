using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TaskProject.Models;

public partial class TaskDbContext : DbContext
{
    public TaskDbContext()
    {
    }

    public TaskDbContext(DbContextOptions<TaskDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<CategoryTask> CategoryTasks { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Title).HasMaxLength(50);
        });

        modelBuilder.Entity<CategoryTask>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Category__3214EC27D6B38AE1");

            entity.ToTable("CategoryTask");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.TaskId).HasColumnName("TaskID");

            entity.HasOne(d => d.Category).WithMany(p => p.CategoryTasks)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CategoryTask_Category");

            entity.HasOne(d => d.Task).WithMany(p => p.CategoryTasks)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CategoryTask_Task");
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Title).HasMaxLength(50);
        });
        modelBuilder.Entity<Task>()
    .Property(e => e.Status)
    .HasConversion<int>();

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
