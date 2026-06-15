using System;
using System.Collections.Generic;
using CourseProject.DataModels;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace CourseProject.Data;

public partial class EmploymentServiceContext : DbContext
{
    public EmploymentServiceContext()
    {
    }

    public EmploymentServiceContext(DbContextOptions<EmploymentServiceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Photo> Photos { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseMySql("data source=localhost;initial catalog=russian_work;user id=root;password=root", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.44-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PRIMARY");

            entity.ToTable("event");

            entity.HasIndex(e => e.PhotoId, "FK_event_photo_idx");

            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.AvailableSpace).HasColumnName("available_space");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");
            entity.Property(e => e.EventDate)
                .HasColumnType("datetime")
                .HasColumnName("event_date");
            entity.Property(e => e.PhotoId).HasColumnName("photo_id");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'В планах'")
                .HasColumnType("enum('Отменено','Проведено','В планах')")
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedOn).HasColumnName("updated_on");

            entity.HasOne(d => d.Photo).WithMany(p => p.Events)
                .HasForeignKey(d => d.PhotoId)
                .HasConstraintName("FK_event_photo");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PRIMARY");

            entity.ToTable("notification");

            entity.HasIndex(e => e.UserId, "FK_User_Notification_idx");

            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("date_created");
            entity.Property(e => e.IsRead).HasColumnName("is_read");
            entity.Property(e => e.Message)
                .HasColumnType("text")
                .HasColumnName("message");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_user_notification");
        });

        modelBuilder.Entity<Photo>(entity =>
        {
            entity.HasKey(e => e.PhotoId).HasName("PRIMARY");

            entity.ToTable("photo");

            entity.Property(e => e.PhotoId).HasColumnName("photo_id");
            entity.Property(e => e.FileName)
                .HasColumnType("text")
                .HasColumnName("file_name");
            entity.Property(e => e.ImageData)
                .HasColumnType("MEDIUMBLOB")
                .HasColumnName("image_data");
            entity.Property(e => e.Title)
                .HasColumnType("text")
                .HasColumnName("title");
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PRIMARY");

            entity.ToTable("request");

            entity.HasIndex(e => e.EventId, "FK_request_event_idx");

            entity.HasIndex(e => e.UserId, "FK_request_user_idx");

            entity.Property(e => e.RequestId).HasColumnName("request_id");
            entity.Property(e => e.Content)
                .HasMaxLength(100)
                .HasColumnName("content");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.Institution)
                .HasMaxLength(100)
                .HasDefaultValueSql("'Нет'")
                .HasColumnName("institution");
            entity.Property(e => e.PeopleNumber)
                .HasDefaultValueSql("'1'")
                .HasColumnName("people_number");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'На рассмотрении'")
                .HasColumnType("enum('На рассмотрении','Отклонена','Принята')")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedOn).HasColumnName("updated_on");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Event).WithMany(p => p.Requests)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_request_event");

            entity.HasOne(d => d.User).WithMany(p => p.Requests)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_request_user");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PRIMARY");

            entity.ToTable("role");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("user");

            entity.HasIndex(e => e.RoleId, "FK_user_role_idx");

            entity.HasIndex(e => e.Username, "username_UNIQUE").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(16)
                .HasColumnName("password");
            entity.Property(e => e.Patronymic)
                .HasMaxLength(25)
                .HasColumnName("patronymic");
            entity.Property(e => e.RoleId)
                .HasDefaultValueSql("'1'")
                .HasColumnName("role_id");
            entity.Property(e => e.Surname)
                .HasMaxLength(20)
                .HasColumnName("surname");
            entity.Property(e => e.Username)
                .HasMaxLength(20)
                .HasColumnName("username");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_user_role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
