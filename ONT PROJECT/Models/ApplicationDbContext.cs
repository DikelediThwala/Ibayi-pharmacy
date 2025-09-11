using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ONT_PROJECT.Models;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActiveIngredient> ActiveIngredient { get; set; }

    public virtual DbSet<BOrder> BOrders { get; set; }

    public virtual DbSet<BOrderLine> BOrderLines { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<CustomerAllergy> CustomerAllergies { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<DosageForm> DosageForms { get; set; }

    public virtual DbSet<MedIngredient> MedIngredients { get; set; }

    public virtual DbSet<Medicine> Medicines { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderLine> OrderLines { get; set; }

    public virtual DbSet<Pharmacist> Pharmacists { get; set; }

    public virtual DbSet<Pharmacy> Pharmacies { get; set; }

    public virtual DbSet<PharmacyManager> PharmacyManagers { get; set; }

    public virtual DbSet<Prescription> Prescriptions { get; set; }

    public virtual DbSet<PrescriptionLine> PrescriptionLines { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<TblUser> TblUsers { get; set; }
    public virtual DbSet<ActivityLog> ActivityLogs { get; set; }

    public virtual DbSet<UnprocessedPrescription> UnprocessedPrescriptions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=soit-sql.mandela.ac.za;Database=GRP-04-04;User Id=GRP-04-04;Password=grp-04-04-2025#;MultipleActiveResultSets=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActiveIngredient>(entity =>
        {
            entity.HasKey(e => e.ActiveIngredientId);

            entity.ToTable("ActiveIngredient");

            entity.Property(e => e.ActiveIngredientId).HasColumnName("ActiveIngredientID");
            entity.Property(e => e.Ingredients).HasMaxLength(50);
            entity.Property(e => e.Status)
              .IsRequired()
              .HasMaxLength(20)
              .HasDefaultValue("Active");
        });

        modelBuilder.Entity<BOrder>(entity =>
        {
            entity.ToTable("B_Order");

            entity.Property(e => e.BOrderId).HasColumnName("B_OrderID");
            entity.Property(e => e.PharmacyManagerId).HasColumnName("PharmacyManagerID");
            entity.Property(e => e.Status).HasMaxLength(10);
            entity.Property(e => e.SupplierId).HasColumnName("SupplierID");

            entity.HasOne(d => d.PharmacyManager).WithMany(p => p.BOrders)
                .HasForeignKey(d => d.PharmacyManagerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_B_Order_PharmacyManager");

            entity.HasOne(d => d.Supplier).WithMany(p => p.BOrders)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_B_Order_Supplier");
        });

        modelBuilder.Entity<BOrderLine>(entity =>
        {
            entity.ToTable("B_OrderLine");

            entity.Property(e => e.BOrderLineId).HasColumnName("B_OrderLineID");
            entity.Property(e => e.BOrderId).HasColumnName("B_OrderID");
            entity.Property(e => e.MedicineId).HasColumnName("MedicineID");

            entity.HasOne(d => d.BOrder).WithMany(p => p.BOrderLines)
                .HasForeignKey(d => d.BOrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_B_OrderLine_B_Order");

            entity.HasOne(d => d.Medicine).WithMany(p => p.BOrderLines)
                .HasForeignKey(d => d.MedicineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_B_OrderLine_Medicine");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customer");

            entity.Property(e => e.CustomerId)
                .ValueGeneratedNever()
                .HasColumnName("CustomerID");

            entity.HasOne(d => d.CustomerNavigation).WithOne(p => p.Customer)
                .HasForeignKey<Customer>(d => d.CustomerId)
                .HasConstraintName("FK_Customer_TblUser");
        });

        modelBuilder.Entity<CustomerAllergy>(entity =>
        {
            entity.ToTable("CustomerAllergy");

            entity.Property(e => e.CustomerAllergyId).HasColumnName("CustomerAllergyID");
            entity.Property(e => e.ActiveIngredientId).HasColumnName("ActiveIngredientID");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

            entity.HasOne(d => d.ActiveIngredient).WithMany(p => p.CustomerAllergies)
                .HasForeignKey(d => d.ActiveIngredientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CustomerAllergy_ActiveIngredient");

            entity.HasOne(d => d.Customer).WithMany(p => p.CustomerAllergies)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CustomerAllergy_Customer");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.ToTable("Doctor");

            entity.Property(e => e.DoctorId).HasColumnName("DoctorID");
            entity.Property(e => e.ContactNo).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.PracticeNo).HasMaxLength(50);
            entity.Property(e => e.Surname).HasMaxLength(50);
            entity.Property(e => e.Status)
              .IsRequired()
              .HasMaxLength(20)
              .HasDefaultValue("Active");
        });

        modelBuilder.Entity<DosageForm>(entity =>
        {
            entity.HasKey(e => e.FormId);

            entity.ToTable("DosageForm");

            entity.Property(e => e.FormId).HasColumnName("FormID");
            entity.Property(e => e.FormName).HasMaxLength(50);
            entity.Property(e => e.Status)
              .IsRequired()
              .HasMaxLength(20)
              .HasDefaultValue("Active");
        });

        modelBuilder.Entity<MedIngredient>(entity =>
        {
            entity.ToTable("MedIngredient");

            entity.Property(e => e.MedIngredientId).HasColumnName("MedIngredientID");
            entity.Property(e => e.ActiveIngredientId).HasColumnName("ActiveIngredientID");
            entity.Property(e => e.MedicineId).HasColumnName("MedicineID");
            entity.Property(e => e.Strength).HasMaxLength(50);

            entity.HasOne(d => d.ActiveIngredient).WithMany(p => p.MedIngredients)
                .HasForeignKey(d => d.ActiveIngredientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MedIngredient_ActiveIngredient");

            entity.HasOne(d => d.Medicine).WithMany(p => p.MedIngredients)
                .HasForeignKey(d => d.MedicineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MedIngredient_Medicine");
        });

        modelBuilder.Entity<Medicine>(entity =>
        {
            entity.ToTable("Medicine");

            entity.Property(e => e.MedicineId).HasColumnName("MedicineID");
            entity.Property(e => e.FormId).HasColumnName("FormID");
            entity.Property(e => e.MedicineName).HasMaxLength(50);
            entity.Property(e => e.SupplierId).HasColumnName("SupplierID");

            entity.HasOne(d => d.Form).WithMany(p => p.Medicines)
                .HasForeignKey(d => d.FormId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Medicine_DosageForm");
            entity.Property(e => e.Status)
              .IsRequired()
              .HasMaxLength(20)
              .HasDefaultValue("Active");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Order");

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.PharmacistId).HasColumnName("PharmacistID");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.SupplierId).HasColumnName("SupplierID");
            entity.Property(e => e.Vat).HasColumnName("VAT");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_Customer");

            entity.HasOne(d => d.Pharmacist).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PharmacistId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_Pharmacist");
        });

        modelBuilder.Entity<OrderLine>(entity =>
        {
            entity.ToTable("OrderLine");

            entity.Property(e => e.OrderLineId).HasColumnName("OrderLineID");
            entity.Property(e => e.LineId).HasColumnName("LineID");
            entity.Property(e => e.MedicineId).HasColumnName("MedicineID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsFixedLength();

            entity.HasOne(d => d.Medicine).WithMany(p => p.OrderLines)
                .HasForeignKey(d => d.MedicineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderLine_Medicine");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderLines)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderLine_Order");
        });

        modelBuilder.Entity<Pharmacist>(entity =>
        {
            entity.ToTable("Pharmacist");

            entity.Property(e => e.PharmacistId)
                .ValueGeneratedNever()
                .HasColumnName("PharmacistID");

            entity.HasOne(d => d.PharmacistNavigation).WithOne(p => p.Pharmacist)
                .HasForeignKey<Pharmacist>(d => d.PharmacistId)
                .HasConstraintName("FK_Pharmacist_TblUser");
        });

        modelBuilder.Entity<Pharmacy>(entity =>
        {
            entity.ToTable("Pharmacy");

            entity.Property(e => e.PharmacyId).HasColumnName("PharmacyID");
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.ContactNo).HasMaxLength(15);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.HealthCounsilRegistrationNo).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.PharmacistId).HasColumnName("PharmacistID");
            entity.Property(e => e.Vatrate).HasColumnName("VATRate");
            entity.Property(e => e.WebsiteUrl).HasMaxLength(50);

            entity.HasOne(d => d.Pharmacist).WithMany(p => p.Pharmacies)
                .HasForeignKey(d => d.PharmacistId)
                .HasConstraintName("FK_Pharmacy_Pharmacist");
            entity.Property(e => e.Status)
              .IsRequired()
              .HasMaxLength(20)
              .HasDefaultValue("Active");
        });

        modelBuilder.Entity<PharmacyManager>(entity =>
        {
            entity.ToTable("PharmacyManager");

            entity.Property(e => e.PharmacyManagerId)
                .ValueGeneratedNever()
                .HasColumnName("PharmacyManagerID");

            entity.HasOne(d => d.PharmacyManagerNavigation).WithOne(p => p.PharmacyManager)
                .HasForeignKey<PharmacyManager>(d => d.PharmacyManagerId)
                .HasConstraintName("FK_PharmacyManager_TblUser");
        });

        modelBuilder.Entity<Prescription>(entity =>
        {
            entity.ToTable("Prescription");

            entity.Property(e => e.PrescriptionId).HasColumnName("PrescriptionID");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.DoctorId).HasColumnName("DoctorID");
            entity.Property(e => e.PharmacistId).HasColumnName("PharmacistID");
            entity.Property(e => e.PrescriptionPhoto).HasMaxLength(50);

            entity.HasOne(d => d.Customer).WithMany(p => p.Prescriptions)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Prescription_Customer");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Prescriptions)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Prescription_Doctor");

            entity.HasOne(d => d.Pharmacist).WithMany(p => p.Prescriptions)
                .HasForeignKey(d => d.PharmacistId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Prescription_Pharmacist");
        });

        modelBuilder.Entity<PrescriptionLine>(entity =>
        {
            entity.ToTable("PrescriptionLine");

            entity.Property(e => e.PrescriptionLineId)
                .ValueGeneratedNever()
                .HasColumnName("PrescriptionLineID");
            entity.Property(e => e.Instructions).HasMaxLength(50);
            entity.Property(e => e.MedicineId).HasColumnName("MedicineID");
            entity.Property(e => e.PrescriptionId).HasColumnName("PrescriptionID");

            entity.HasOne(d => d.Medicine).WithMany(p => p.PrescriptionLines)
                .HasForeignKey(d => d.MedicineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PrescriptionLine_Medicine");

            entity.HasOne(d => d.Prescription).WithMany(p => p.PrescriptionLines)
                .HasForeignKey(d => d.PrescriptionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PrescriptionLine_Prescription");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.ToTable("Supplier");

            entity.Property(e => e.SupplierId).HasColumnName("SupplierID");
            entity.Property(e => e.ContactNo).HasMaxLength(11);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Status)
              .IsRequired()
              .HasMaxLength(20)
              .HasDefaultValue("Active");
        });

        modelBuilder.Entity<TblUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK_User");

            entity.ToTable("tblUser");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.Idnumber)
                .HasMaxLength(50)
                .HasColumnName("IDNumber");
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(50)
                .HasColumnName("Phone Number");
            entity.Property(e => e.Role).HasMaxLength(50);
            entity.Property(e => e.Title)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Status)
              .IsRequired()
              .HasMaxLength(20)
              .HasDefaultValue("Active");
        });

        modelBuilder.Entity<UnprocessedPrescription>(entity =>
        {
            entity.ToTable("UnprocessedPrescription");

            entity.Property(e => e.UnprocessedPrescriptionId).HasColumnName("UnprocessedPrescriptionID");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.Dispense).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(10);

            entity.HasOne(d => d.Customer).WithMany(p => p.UnprocessedPrescriptions)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UnprocessedPrescription_Customer");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
