using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using OrderWaveAPI.Models;

namespace OrderWaveAPI.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DishDetail> DishDetails { get; set; }

    public virtual DbSet<Guest> Guests { get; set; }

    public virtual DbSet<KitchenQueue> KitchenQueues { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<MenuCategory> MenuCategories { get; set; }

    public virtual DbSet<MenuPhoto> MenuPhotos { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentDetail> PaymentDetails { get; set; }

    public virtual DbSet<RestaurantTable> RestaurantTables { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<TableAssignment> TableAssignments { get; set; }

    public virtual DbSet<TableSession> TableSessions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<Waiter> Waiters { get; set; }

    public virtual DbSet<WaitersShift> WaitersShifts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(
                "Data Source=SHARKIPC;Initial Catalog=Order_Wave;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");
        }
    }
// #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https: //go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//     => optionsBuilder.UseSqlServer("Data Source=SHARKIPC;Initial Catalog=Order_Wave;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DishDetail>(entity =>
        {
            entity.HasKey(e => e.DetailId).HasName("PK__dish_det__38E9A224D80C2A6A");

            entity.ToTable("dish_details", tb => tb.HasTrigger("trg_dish_details_updated_at"));

            entity.HasIndex(e => e.DishId, "UQ__dish_det__9F2B4CF87A69863E").IsUnique();

            entity.Property(e => e.DetailId).HasColumnName("detail_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.DishDescription)
                .HasMaxLength(1000)
                .HasColumnName("dish_description");
            entity.Property(e => e.DishId).HasColumnName("dish_id");
            entity.Property(e => e.DishIngredients)
                .HasMaxLength(1000)
                .HasColumnName("dish_ingredients");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.Dish).WithOne(p => p.DishDetail)
                .HasForeignKey<DishDetail>(d => d.DishId)
                .HasConstraintName("fk_details_dish");
        });

        modelBuilder.Entity<Guest>(entity =>
        {
            entity.HasKey(e => e.GuestId).HasName("PK__guests__19778E35C355F3F7");

            entity.ToTable("guests", tb => tb.HasTrigger("trg_guests_updated_at"));

            entity.HasIndex(e => e.SessionId, "ix_guests_session");

            entity.Property(e => e.GuestId).HasColumnName("guest_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.GuestName)
                .HasMaxLength(100)
                .HasColumnName("guest_name");
            entity.Property(e => e.GuestSurname)
                .HasMaxLength(100)
                .HasColumnName("guest_surname");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.Session).WithMany(p => p.Guests)
                .HasForeignKey(d => d.SessionId)
                .HasConstraintName("fk_guests_session");
        });

        modelBuilder.Entity<KitchenQueue>(entity =>
        {
            entity.HasKey(e => e.QueueId).HasName("PK__kitchen___2294FA6ED6BE7574");

            entity.ToTable("kitchen_queue", tb => tb.HasTrigger("trg_kitchen_queue_updated_at"));

            entity.HasIndex(e => e.OrderDetailId, "ix_kitchen_queue_detail");

            entity.HasIndex(e => e.DishStatus, "ix_kitchen_queue_status");

            entity.Property(e => e.QueueId).HasColumnName("queue_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.DishStatus)
                .HasMaxLength(20)
                .HasDefaultValue("Cooking")
                .HasColumnName("dish_status");
            entity.Property(e => e.OrderDetailId).HasColumnName("order_detail_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.OrderDetail).WithMany(p => p.KitchenQueues)
                .HasForeignKey(d => d.OrderDetailId)
                .HasConstraintName("fk_queue_order_detail");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.DishId).HasName("PK__menu__9F2B4CF9784D1EDA");

            entity.ToTable("menu", tb => tb.HasTrigger("trg_menu_updated_at"));

            entity.HasIndex(e => e.CategoryId, "ix_menu_category_id");

            entity.HasIndex(e => e.IsActive, "ix_menu_is_active");

            entity.Property(e => e.DishId).HasColumnName("dish_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.DishName)
                .HasMaxLength(255)
                .HasColumnName("dish_name");
            entity.Property(e => e.DishPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("dish_price");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.Category).WithMany(p => p.Menus)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_menu_category");
        });

        modelBuilder.Entity<MenuCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__menu_cat__D54EE9B469CC2D7E");

            entity.ToTable("menu_categories", tb => tb.HasTrigger("trg_menu_categories_updated_at"));

            entity.HasIndex(e => e.CategoryName, "UQ__menu_cat__5189E2555FC99894").IsUnique();

            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(100)
                .HasColumnName("category_name");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
        });

        modelBuilder.Entity<MenuPhoto>(entity =>
        {
            entity.HasKey(e => e.PhotoId).HasName("PK__menu_pho__CB48C83DB4D0B2B5");

            entity.ToTable("menu_photos", tb => tb.HasTrigger("trg_menu_photos_updated_at"));

            entity.HasIndex(e => e.DishId, "ix_menu_photos_dish");

            entity.HasIndex(e => e.IsMain, "ix_menu_photos_main");

            entity.HasIndex(e => new { e.DishId, e.PhotoUrl }, "uq_menu_photos_dish_url").IsUnique();

            entity.HasIndex(e => e.DishId, "ux_one_main_photo")
                .IsUnique()
                .HasFilter("([is_main]=(1))");

            entity.Property(e => e.PhotoId).HasColumnName("photo_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.DishId).HasColumnName("dish_id");
            entity.Property(e => e.IsMain).HasColumnName("is_main");
            entity.Property(e => e.PhotoUrl)
                .HasMaxLength(500)
                .HasColumnName("photo_url");
            entity.Property(e => e.SortOrder).HasColumnName("sort_order");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.Dish).WithOne(p => p.MenuPhoto)
                .HasForeignKey<MenuPhoto>(d => d.DishId)
                .HasConstraintName("fk_photos_dish");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__orders__46596229E099E597");

            entity.ToTable("orders", tb => tb.HasTrigger("trg_orders_updated_at"));

            entity.HasIndex(e => e.GuestId, "ix_orders_guest_id");

            entity.HasIndex(e => new { e.SessionId, e.CurrentStatus }, "ix_orders_session_status");

            entity.HasIndex(e => e.CurrentStatus, "ix_orders_status");

            entity.HasIndex(e => e.WaiterId, "ix_orders_waiter_id");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.CurrentStatus)
                .HasMaxLength(20)
                .HasDefaultValue("Pending")
                .HasColumnName("current_status");
            entity.Property(e => e.GuestId).HasColumnName("guest_id");
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("order_date");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.WaiterId).HasColumnName("waiter_id");

            entity.HasOne(d => d.Guest).WithMany(p => p.Orders)
                .HasForeignKey(d => d.GuestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_orders_guest");

            entity.HasOne(d => d.Session).WithMany(p => p.Orders)
                .HasForeignKey(d => d.SessionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_orders_session");

            entity.HasOne(d => d.Waiter).WithMany(p => p.Orders)
                .HasForeignKey(d => d.WaiterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_orders_waiter");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__order_de__3C5A4080D6956630");

            entity.ToTable("order_details", tb => tb.HasTrigger("trg_order_details_updated_at"));

            entity.HasIndex(e => e.DishId, "ix_order_details_dish");

            entity.HasIndex(e => e.OrderId, "ix_order_details_order");

            entity.Property(e => e.OrderDetailId).HasColumnName("order_detail_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.DishAmount).HasColumnName("dish_amount");
            entity.Property(e => e.DishId).HasColumnName("dish_id");
            entity.Property(e => e.DishPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("dish_price");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.Dish).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.DishId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_order_details_dish");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("fk_order_details_order");
        });

        modelBuilder.Entity<OrderStatusHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId).HasName("PK__order_st__096AA2E974BEC3FF");

            entity.ToTable("order_status_history", tb => tb.HasTrigger("trg_order_status_history_updated_at"));

            entity.HasIndex(e => e.OrderId, "ix_status_history_order");

            entity.HasIndex(e => e.ChangedBy, "ix_status_history_user");

            entity.Property(e => e.HistoryId).HasColumnName("history_id");
            entity.Property(e => e.ChangedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("changed_at");
            entity.Property(e => e.ChangedBy).HasColumnName("changed_by");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.ChangedByNavigation).WithMany(p => p.OrderStatusHistories)
                .HasForeignKey(d => d.ChangedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_history_user");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderStatusHistories)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("fk_history_order");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__payments__ED1FC9EAAEE11648");

            entity.ToTable("payments", tb => tb.HasTrigger("trg_payments_updated_at"));

            entity.HasIndex(e => e.TransactionId, "UQ__payments__85C600AE6747849C").IsUnique();

            entity.HasIndex(e => e.GuestId, "ix_payments_guest");

            entity.HasIndex(e => e.SessionId, "ix_payments_session");

            entity.HasIndex(e => e.PaymentStatus, "ix_payments_status");

            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.GuestId).HasColumnName("guest_id");
            entity.Property(e => e.PaidAt).HasColumnName("paid_at");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(20)
                .HasColumnName("payment_method");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(20)
                .HasDefaultValue("Pending")
                .HasColumnName("payment_status");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total_amount");
            entity.Property(e => e.TransactionId)
                .HasMaxLength(255)
                .HasColumnName("transaction_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.Guest).WithMany(p => p.Payments)
                .HasForeignKey(d => d.GuestId)
                .HasConstraintName("fk_payments_guest");

            entity.HasOne(d => d.Session).WithMany(p => p.Payments)
                .HasForeignKey(d => d.SessionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_payments_session");
        });

        modelBuilder.Entity<PaymentDetail>(entity =>
        {
            entity.HasKey(e => e.PaymentDetailId).HasName("PK__payment___C66E6E36346ECE87");

            entity.ToTable("payment_details", tb => tb.HasTrigger("trg_payment_details_updated_at"));

            entity.HasIndex(e => e.OrderDetailId, "ix_payment_details_order");

            entity.HasIndex(e => e.PaymentId, "ix_payment_details_payment");

            entity.Property(e => e.PaymentDetailId).HasColumnName("payment_detail_id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.OrderDetailId).HasColumnName("order_detail_id");
            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.OrderDetail).WithMany(p => p.PaymentDetails)
                .HasForeignKey(d => d.OrderDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_pay_details_order");

            entity.HasOne(d => d.Payment).WithMany(p => p.PaymentDetails)
                .HasForeignKey(d => d.PaymentId)
                .HasConstraintName("fk_pay_details_payment");
        });

        modelBuilder.Entity<RestaurantTable>(entity =>
        {
            entity.HasKey(e => e.TableId).HasName("PK__restaura__B21E8F2423B43F50");

            entity.ToTable("restaurant_tables", tb => tb.HasTrigger("trg_restaurant_tables_updated_at"));

            entity.HasIndex(e => e.TableNumber, "UQ__restaura__21B232CEDB2C0F6A").IsUnique();

            entity.HasIndex(e => e.IsActive, "ix_restaurant_tables_active");

            entity.Property(e => e.TableId).HasColumnName("table_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.TableCapacity).HasColumnName("table_capacity");
            entity.Property(e => e.TableNumber).HasColumnName("table_number");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__roles__760965CCF76CBDE7");

            entity.ToTable("roles", tb => tb.HasTrigger("trg_roles_updated_at"));

            entity.HasIndex(e => e.RoleName, "UQ__roles__783254B1113D738F").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .HasColumnName("role_name");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
        });

        modelBuilder.Entity<TableAssignment>(entity =>
        {
            entity.HasKey(e => e.AssignmentId).HasName("PK__table_as__DA89181479990F47");

            entity.ToTable("table_assignments", tb => tb.HasTrigger("trg_table_assignments_updated_at"));

            entity.HasIndex(e => e.SessionId, "ix_table_assignments_session");

            entity.HasIndex(e => e.WaiterId, "ix_table_assignments_waiter");

            entity.HasIndex(e => new { e.SessionId, e.WaiterId }, "uq_assignment").IsUnique();

            entity.Property(e => e.AssignmentId).HasColumnName("assignment_id");
            entity.Property(e => e.AssignedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("assigned_at");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.WaiterId).HasColumnName("waiter_id");

            entity.HasOne(d => d.Session).WithMany(p => p.TableAssignments)
                .HasForeignKey(d => d.SessionId)
                .HasConstraintName("fk_assignment_session");

            entity.HasOne(d => d.Waiter).WithMany(p => p.TableAssignments)
                .HasForeignKey(d => d.WaiterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_assignment_waiter");
        });

        modelBuilder.Entity<TableSession>(entity =>
        {
            entity.HasKey(e => e.SessionId).HasName("PK__table_se__69B13FDCE4C3EE52");

            entity.ToTable("table_sessions", tb => tb.HasTrigger("trg_table_sessions_updated_at"));

            entity.HasIndex(e => e.IsActive, "ix_table_sessions_is_active");

            entity.HasIndex(e => e.TableId, "ix_table_sessions_table");

            entity.HasIndex(e => e.TableId, "ux_one_active_session")
                .IsUnique()
                .HasFilter("([is_active]=(1))");

            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.ClosedAt).HasColumnName("closed_at");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.GuestsAmount).HasColumnName("guests_amount");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.OpenedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("opened_at");
            entity.Property(e => e.TableId).HasColumnName("table_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.Table).WithMany(p => p.TableSessions)
                .HasForeignKey(d => d.TableId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_sessions_table");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__users__B9BE370FC8040D97");

            entity.ToTable("users", tb => tb.HasTrigger("trg_users_updated_at"));

            entity.HasIndex(e => e.UserLogin, "UQ__users__9EA1B5AF761F5C64").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.AvatarUrl)
                .HasMaxLength(500)
                .HasColumnName("avatar_url");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UserLogin)
                .HasMaxLength(255)
                .HasColumnName("user_login");
            entity.Property(e => e.UserPassword)
                .HasMaxLength(255)
                .HasColumnName("user_password");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.UserRoleId).HasName("PK__user_rol__B8D9ABA2DAC0FB9A");

            entity.ToTable("user_roles", tb => tb.HasTrigger("trg_user_roles_updated_at"));

            entity.HasIndex(e => e.RoleId, "ix_user_roles_role_id");

            entity.HasIndex(e => e.UserId, "ix_user_roles_user_id");

            entity.HasIndex(e => new { e.UserId, e.RoleId }, "uq_user_role").IsUnique();

            entity.Property(e => e.UserRoleId).HasColumnName("user_role_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_user_roles_role");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_user_roles_user");
        });

        modelBuilder.Entity<Waiter>(entity =>
        {
            entity.HasKey(e => e.WaiterId).HasName("PK__waiters__8714E273F4784C36");

            entity.ToTable("waiters", tb => tb.HasTrigger("trg_waiters_updated_at"));

            entity.HasIndex(e => e.UserId, "UQ__waiters__B9BE370ED842463B").IsUnique();

            entity.HasIndex(e => e.UserId, "ix_waiters_user_id");

            entity.Property(e => e.WaiterId).HasColumnName("waiter_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithOne(p => p.Waiter)
                .HasForeignKey<Waiter>(d => d.UserId)
                .HasConstraintName("fk_waiters_user");
        });

        modelBuilder.Entity<WaitersShift>(entity =>
        {
            entity.HasKey(e => e.ShiftId).HasName("PK__waiters___7B2672201C7BEAE4");

            entity.ToTable("waiters_shifts", tb => tb.HasTrigger("trg_waiters_shifts_updated_at"));

            entity.HasIndex(e => e.WaiterId, "ix_waiters_shifts_waiter");

            entity.Property(e => e.ShiftId).HasColumnName("shift_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.ShiftEnd).HasColumnName("shift_end");
            entity.Property(e => e.ShiftStart).HasColumnName("shift_start");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.WaiterId).HasColumnName("waiter_id");

            entity.HasOne(d => d.Waiter).WithMany(p => p.WaitersShifts)
                .HasForeignKey(d => d.WaiterId)
                .HasConstraintName("fk_shifts_waiter");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
