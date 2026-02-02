using Microsoft.EntityFrameworkCore;

namespace CRUD.Models
{
    public partial class DumContext : DbContext
    {
        public DumContext()
        {
        }

        public DumContext(DbContextOptions<DumContext> options)
            : base(options)
        {
        }

        public virtual DbSet<EmployeeInfo> EmployeeInfos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmployeeInfo>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__employee__3213E83F1EA7D212");

                entity.ToTable("employee_info");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Adress)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("adress");
                entity.Property(e => e.Designation)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("designation");
                entity.Property(e => e.Fname)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("fname");
                entity.Property(e => e.Isdeleted).HasColumnName("isdeleted");
                entity.Property(e => e.Lname)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("lname");
                entity.Property(e => e.Salary).HasColumnName("salary");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
