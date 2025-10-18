using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace student_management.Models;

public partial class QuanlyhocDbContext : DbContext
{
    public QuanlyhocDbContext()
    {
    }

    public QuanlyhocDbContext(DbContextOptions<QuanlyhocDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BangDiem> BangDiems { get; set; }

    public virtual DbSet<DangKyHoc> DangKyHocs { get; set; }

    public virtual DbSet<DiemDanh> DiemDanhs { get; set; }

    public virtual DbSet<GiaoVien> GiaoViens { get; set; }

    public virtual DbSet<Khoa> Khoas { get; set; }

    public virtual DbSet<LopChinhQuy> LopChinhQuies { get; set; }

    public virtual DbSet<LopHocPhan> LopHocPhans { get; set; }

    public virtual DbSet<MonHoc> MonHocs { get; set; }

    public virtual DbSet<SinhVien> SinhViens { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

    public virtual DbSet<ThoiKhoaBieu> ThoiKhoaBieus { get; set; }

    public virtual DbSet<ThongBao> ThongBaos { get; set; }

    public virtual DbSet<VaiTro> VaiTros { get; set; }

    public virtual DbSet<Vien> Viens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=ADMIN\\SQLEXPRESS03;Database=quanlyhoc_db;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BangDiem>(entity =>
        {
            entity.HasKey(e => new { e.MaSv, e.MaLhp }).HasName("PK__BangDiem__349CB173D61B806A");

            entity.ToTable("BangDiem");

            entity.Property(e => e.MaSv)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("MaSV");
            entity.Property(e => e.MaLhp).HasColumnName("MaLHP");
            entity.Property(e => e.DiemChuyenCan).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.DiemCuoiKy).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.DiemGiuaKy).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.DiemTongKet)
                .HasComputedColumnSql("(case when [DiemChuyenCan] IS NULL AND [DiemGiuaKy] IS NULL AND [DiemCuoiKy] IS NULL then NULL else round((isnull([DiemChuyenCan],(0))*(0.1)+isnull([DiemGiuaKy],(0))*(0.3))+isnull([DiemCuoiKy],(0))*(0.6),(2)) end)", true)
                .HasColumnType("numeric(9, 3)");

            entity.HasOne(d => d.DangKyHoc).WithOne(p => p.BangDiem)
                .HasForeignKey<BangDiem>(d => new { d.MaSv, d.MaLhp })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BangDiem_DangKyHoc");
        });

        modelBuilder.Entity<DangKyHoc>(entity =>
        {
            entity.HasKey(e => new { e.MaSv, e.MaLhp }).HasName("PK__DangKyHo__349CB173D2FD733E");

            entity.ToTable("DangKyHoc");

            entity.Property(e => e.MaSv)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("MaSV");
            entity.Property(e => e.MaLhp).HasColumnName("MaLHP");
            entity.Property(e => e.NgayDangKy).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(20)
                .HasDefaultValue("enrolled");

            entity.HasOne(d => d.MaLhpNavigation).WithMany(p => p.DangKyHocs)
                .HasForeignKey(d => d.MaLhp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DangKyHoc_LopHocPhan");

            entity.HasOne(d => d.MaSvNavigation).WithMany(p => p.DangKyHocs)
                .HasForeignKey(d => d.MaSv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DangKyHoc_SinhVien");
        });

        modelBuilder.Entity<DiemDanh>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DiemDanh__3214EC0785CEC9FC");

            entity.ToTable("DiemDanh");

            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.MaLhp).HasColumnName("MaLHP");
            entity.Property(e => e.MaSv)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("MaSV");
            entity.Property(e => e.TrangThai).HasMaxLength(20);

            entity.HasOne(d => d.DangKyHoc).WithMany(p => p.DiemDanhs)
                .HasForeignKey(d => new { d.MaSv, d.MaLhp })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DiemDanh_DangKyHoc");
        });

        modelBuilder.Entity<GiaoVien>(entity =>
        {
            entity.HasKey(e => e.MaGv).HasName("PK__GiaoVien__2725AEF38F53316A");

            entity.ToTable("GiaoVien");

            entity.HasIndex(e => e.Email, "UQ__GiaoVien__A9D10534D97CCF3F").IsUnique();

            entity.Property(e => e.MaGv)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MaGV");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.MaKhoa)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.HasOne(d => d.MaKhoaNavigation).WithMany(p => p.GiaoViens)
                .HasForeignKey(d => d.MaKhoa)
                .HasConstraintName("FK_GiaoVien_Khoa");
        });

        modelBuilder.Entity<Khoa>(entity =>
        {
            entity.HasKey(e => e.MaKhoa).HasName("PK__Khoa__6539040534437BCE");

            entity.ToTable("Khoa");

            entity.Property(e => e.MaKhoa)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaVien)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TenKhoa).HasMaxLength(100);

            entity.HasOne(d => d.MaVienNavigation).WithMany(p => p.Khoas)
                .HasForeignKey(d => d.MaVien)
                .HasConstraintName("FK_Khoa_Vien");
        });

        modelBuilder.Entity<LopChinhQuy>(entity =>
        {
            entity.HasKey(e => e.MaLop).HasName("PK__LopChinh__3B98D2732A3F74F4");

            entity.ToTable("LopChinhQuy");

            entity.Property(e => e.MaLop)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.MaKhoa)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TenLop).HasMaxLength(100);

            entity.HasOne(d => d.MaKhoaNavigation).WithMany(p => p.LopChinhQuies)
                .HasForeignKey(d => d.MaKhoa)
                .HasConstraintName("FK_LopChinhQuy_Khoa");
        });

        modelBuilder.Entity<LopHocPhan>(entity =>
        {
            entity.HasKey(e => e.MaLhp).HasName("PK__LopHocPh__3B9B9690B5F084A4");

            entity.ToTable("LopHocPhan");

            entity.Property(e => e.MaLhp).HasColumnName("MaLHP");
            entity.Property(e => e.MaGv)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MaGV");
            entity.Property(e => e.MaMh)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("MaMH");
            entity.Property(e => e.NamHoc)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TenLhp)
                .HasMaxLength(100)
                .HasColumnName("TenLHP");

            entity.HasOne(d => d.MaGvNavigation).WithMany(p => p.LopHocPhans)
                .HasForeignKey(d => d.MaGv)
                .HasConstraintName("FK_LopHocPhan_GiaoVien");

            entity.HasOne(d => d.MaMhNavigation).WithMany(p => p.LopHocPhans)
                .HasForeignKey(d => d.MaMh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LopHocPhan_MonHoc");
        });

        modelBuilder.Entity<MonHoc>(entity =>
        {
            entity.HasKey(e => e.MaMh).HasName("PK__MonHoc__2725DFD9EAB72EBC");

            entity.ToTable("MonHoc");

            entity.Property(e => e.MaMh)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("MaMH");
            entity.Property(e => e.MaKhoa)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TenMh)
                .HasMaxLength(150)
                .HasColumnName("TenMH");

            entity.HasOne(d => d.MaKhoaNavigation).WithMany(p => p.MonHocs)
                .HasForeignKey(d => d.MaKhoa)
                .HasConstraintName("FK_MonHoc_Khoa");
        });

        modelBuilder.Entity<SinhVien>(entity =>
        {
            entity.HasKey(e => e.MaSv).HasName("PK__SinhVien__2725081A775DECD3");

            entity.ToTable("SinhVien");

            entity.HasIndex(e => e.Email, "UQ__SinhVien__A9D10534E170E25F").IsUnique();

            entity.Property(e => e.MaSv)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("MaSV");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.MaKhoa)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaLop)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.HasOne(d => d.MaKhoaNavigation).WithMany(p => p.SinhViens)
                .HasForeignKey(d => d.MaKhoa)
                .HasConstraintName("FK_SinhVien_Khoa");

            entity.HasOne(d => d.MaLopNavigation).WithMany(p => p.SinhViens)
                .HasForeignKey(d => d.MaLop)
                .HasConstraintName("FK_SinhVien_Lop");
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TaiKhoan__3214EC07552601A6");

            entity.ToTable("TaiKhoan");

            entity.HasIndex(e => e.TenDangNhap, "UQ__TaiKhoan__55F68FC0FA5424FF").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.MaGv)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MaGV");
            entity.Property(e => e.MaSv)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("MaSV");
            entity.Property(e => e.MatKhau).HasMaxLength(255);
            entity.Property(e => e.TenDangNhap)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);

            entity.HasOne(d => d.MaGvNavigation).WithMany(p => p.TaiKhoans)
                .HasForeignKey(d => d.MaGv)
                .HasConstraintName("FK_TaiKhoan_GiaoVien");

            entity.HasOne(d => d.MaSvNavigation).WithMany(p => p.TaiKhoans)
                .HasForeignKey(d => d.MaSv)
                .HasConstraintName("FK_TaiKhoan_SinhVien");

            entity.HasOne(d => d.MaVaiTroNavigation).WithMany(p => p.TaiKhoans)
                .HasForeignKey(d => d.MaVaiTro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaiKhoan_VaiTro");
        });

        modelBuilder.Entity<ThoiKhoaBieu>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ThoiKhoa__3214EC07434A225B");

            entity.ToTable("ThoiKhoaBieu");

            entity.Property(e => e.MaLhp).HasColumnName("MaLHP");
            entity.Property(e => e.PhongHoc)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.MaLhpNavigation).WithMany(p => p.ThoiKhoaBieus)
                .HasForeignKey(d => d.MaLhp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ThoiKhoaBieu_LopHocPhan");
        });

        modelBuilder.Entity<ThongBao>(entity =>
        {
            entity.HasKey(e => e.MaTb).HasName("PK__ThongBao__2725006F2E068A47");

            entity.ToTable("ThongBao");

            entity.Property(e => e.MaTb).HasColumnName("MaTB");
            entity.Property(e => e.MaGv)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MaGV");
            entity.Property(e => e.MaLhp).HasColumnName("MaLHP");
            entity.Property(e => e.NgayDang).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.TieuDe).HasMaxLength(200);

            entity.HasOne(d => d.MaGvNavigation).WithMany(p => p.ThongBaos)
                .HasForeignKey(d => d.MaGv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ThongBao_GiaoVien");

            entity.HasOne(d => d.MaLhpNavigation).WithMany(p => p.ThongBaos)
                .HasForeignKey(d => d.MaLhp)
                .HasConstraintName("FK_ThongBao_LopHocPhan");
        });

        modelBuilder.Entity<VaiTro>(entity =>
        {
            entity.HasKey(e => e.MaVaiTro).HasName("PK__VaiTro__C24C41CF17DEC37C");

            entity.ToTable("VaiTro");

            entity.HasIndex(e => e.TenVaiTro, "UQ__VaiTro__1DA5581490DABBF5").IsUnique();

            entity.Property(e => e.MaVaiTro).ValueGeneratedNever();
            entity.Property(e => e.TenVaiTro).HasMaxLength(50);
        });

        modelBuilder.Entity<Vien>(entity =>
        {
            entity.HasKey(e => e.MaVien).HasName("PK__Vien__8D4CA7C60A9BEE37");

            entity.ToTable("Vien");

            entity.Property(e => e.MaVien)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MoTa).HasMaxLength(255);
            entity.Property(e => e.TenVien).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
