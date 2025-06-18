using DA_Web.Models;
using DA_Web.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace DA_Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Register all your models here
        public DbSet<Location> Locations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserOtp> UserOtps { get; set; }
        public DbSet<LocationDetail> LocationDetails { get; set; }
        public DbSet<BestTime> BestTimes { get; set; }
        public DbSet<TravelMethod> TravelMethods { get; set; }
        public DbSet<TravelInfo> TravelInfos { get; set; }
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<Cuisine> Cuisines { get; set; }
        public DbSet<Tip> Tips { get; set; }
        public DbSet<NearbyLocation> NearbyLocations { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<LocationHotel> LocationHotels { get; set; }
        public DbSet<LocationImage> Location_Images { get; set; }
        public DbSet<Tour> Tours { get; set; }
        public DbSet<TourHighlight> Tour_Highlights { get; set; }
        public DbSet<TourSchedule> Tour_Schedules { get; set; }
        public DbSet<TourLocation> Tour_Locations { get; set; }
        public DbSet<ScheduleActivity> Schedule_Activities { get; set; }
        public DbSet<TourInclude> Tour_Includes { get; set; }
        public DbSet<TourExclude> Tour_Excludes { get; set; }
        public DbSet<TourNote> Tour_Notes { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<ReviewImage> Review_Images { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        // ... bên trong public class ApplicationDbContext
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure composite primary keys for many-to-many join tables
            modelBuilder.Entity<NearbyLocation>()
                .HasKey(nl => new { nl.LocationId, nl.NearbyLocationId });

            modelBuilder.Entity<LocationHotel>()
                .HasKey(lh => new { lh.LocationId, lh.HotelId });

            modelBuilder.Entity<TourLocation>()
                .HasKey(tl => new { tl.TourId, tl.LocationId, tl.ScheduleId });

            // Configure the one-to-one relationship between Location and LocationDetail
            modelBuilder.Entity<Location>()
                .HasOne(l => l.LocationDetail)
                .WithOne(ld => ld.Location)
                .HasForeignKey<LocationDetail>(ld => ld.LocationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure the one-to-one relationship between Location and TravelInfo
            modelBuilder.Entity<Location>()
                .HasOne(l => l.TravelInfo)
                .WithOne(ti => ti.Location)
                .HasForeignKey<TravelInfo>(ti => ti.LocationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure the self-referencing many-to-many relationship for NearbyLocation
            modelBuilder.Entity<NearbyLocation>()
                .HasOne(nl => nl.Location)
                .WithMany(l => l.NearbyLocations)
                .HasForeignKey(nl => nl.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NearbyLocation>()
                .HasOne(nl => nl.Nearby)
                .WithMany(l => l.LocationsNearby)
                .HasForeignKey(nl => nl.NearbyLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            // ======================== PHẦN THÊM VÀO ĐỂ SỬA LỖI ========================
            // Sửa lỗi multiple cascade paths cho TourLocation
            // Khi xóa Tour, không tự động xóa TourLocation trực tiếp
            // mà để nó được xóa thông qua TourSchedule
            modelBuilder.Entity<TourLocation>()
                .HasOne(tl => tl.Tour)
                .WithMany(t => t.TourLocations)
                .HasForeignKey(tl => tl.TourId)
                .OnDelete(DeleteBehavior.Restrict); // Thay đổi từ CASCADE (mặc định) thành RESTRICT

            modelBuilder.Entity<TourLocation>()
                .HasOne(tl => tl.TourSchedule)
                .WithMany(ts => ts.TourLocations)
                .HasForeignKey(tl => tl.ScheduleId)
                .OnDelete(DeleteBehavior.Cascade); // Giữ nguyên CASCADE cho đường này
                                                   // ============================= KẾT THÚC PHẦN SỬA =============================


            // Configure enum to string conversions for cross-database compatibility
            modelBuilder.Entity<User>().Property(u => u.Role).HasConversion<string>();
            modelBuilder.Entity<TravelMethod>().Property(tm => tm.MethodType).HasConversion<string>();
            modelBuilder.Entity<LocationImage>().Property(li => li.ImageType).HasConversion<string>();
            modelBuilder.Entity<Tour>().Property(t => t.Status).HasConversion<string>();
            modelBuilder.Entity<ContactMessage>().Property(cm => cm.Status).HasConversion<string>();

            // Configure indexes from your SQL
            modelBuilder.Entity<Location>().HasIndex(l => l.Name);
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.Phone).IsUnique();
            modelBuilder.Entity<Tour>().HasIndex(t => t.Destination);
            modelBuilder.Entity<Review>().HasIndex(r => r.TourId);
            modelBuilder.Entity<TourSchedule>().HasIndex(ts => ts.TourId);
        }
    }
}