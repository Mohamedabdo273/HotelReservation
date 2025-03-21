﻿
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models.Models;

namespace Infrastructures.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<HotelAmenities> HotelAmenities { get; set; }
        public DbSet<ImageList> ImageLists { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<ReportDetails> ReportDetails { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ReservationRoom> ReservationRooms { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Comment> Comments { get; set; }
        //public DbSet<Reply> Replies { get; set; } 
        public DbSet<ContactUs> ContactUs { get; set; }
        public DbSet<Message> Message { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                base.OnConfiguring(optionsBuilder);
                var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build();
                var connection = builder.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connection);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<HotelAmenities>()
                .HasKey(e => new { e.HotelId, e.AmenityId });

            modelBuilder.Entity<ReservationRoom>()
                .HasKey(e => new { e.ReservationID, e.RoomId });

            modelBuilder.Entity<Hotel>()
            .HasOne(h => h.company)
            .WithMany(c => c.Hotels)
            .HasForeignKey(h => h.CompanyId);

            modelBuilder.Entity<ImageList>()
            .HasOne(il => il.Hotel)
            .WithMany(h => h.ImageLists)
            .HasForeignKey(il => il.HotelId);

            modelBuilder.Entity<Hotel>()
            .HasOne(h => h.Report)
            .WithOne(r => r.Hotel)
            .HasForeignKey<Report>(r => r.HotelId);

            modelBuilder.Entity<Rating>()
            .HasOne(r => r.Hotel)
            .WithMany(h => h.Ratings)
            .HasForeignKey(r => r.HotelId);

            modelBuilder.Entity<Hotel>()
            .HasMany(r => r.Rooms)
            .WithOne(h => h.Hotel)
            .HasForeignKey(r => r.HotelId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Room>()
            .HasOne(r => r.RoomType)
            .WithMany(rt => rt.Rooms)
            .HasForeignKey(r => r.RoomTypeId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RoomType>()
            .HasOne(rt => rt.Hotel)
            .WithMany(h => h.RoomTypes)
            .HasForeignKey(rt => rt.HotelId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Reservation>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reservations)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reservation>()
           .HasOne(r => r.Hotel)
           .WithMany(u => u.Reservations)
           .HasForeignKey(r => r.HotelId)
           .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Hotel>()
            .HasIndex(h => new { h.Name, h.City })
            .IsUnique()
            .HasDatabaseName("IX_Hotel_Name_City_Unique");


            modelBuilder.Entity<Reservation>().ToTable(t => t.HasTrigger("SetRoomAvailableOnCheckOuts"));
            modelBuilder.Entity<Reservation>().ToTable(t => t.HasTrigger("SetRoomUnavailableOnCheckIns"));
            modelBuilder.Entity<Reservation>().ToTable(t => t.HasTrigger("RemovePendingReservations"));
            modelBuilder.Entity<Coupon>().ToTable(t => t.HasTrigger("TRG_DeleteExpiredOrDepletedCoupons"));



        }





    }

}


