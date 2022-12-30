using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using bridgefluence.Services.Publisher;
using bridgefluence.Tools;
using Microsoft.EntityFrameworkCore;

namespace bridgefluence_api
{
    public sealed class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public DbSet<Publisher> Publishers { get; set; } 
        public DbSet<Brand> Brands { get; set; } 
        public DbSet<Post> Posts { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
        }
    }

    [Table("publisher")]
    public class Publisher
    {
        [Column("id")] public int Id { get; set; }
        [Column("name")] public string Name { get; set; }
        [Column("payment_qr")] public string PaymentQr { get; set; }
        [Column("subscriber_count")] public int SubscriberCount { get; set; }
        [Column("geo", TypeName = "jsonb")] public List<GeoItem> Geo { get; set; }
        
        public List<Post> Posts { get; set; }
    }

    [Table("brand")]
    public class Brand
    {
        [Column("id")] public int Id { get; set; } 
        [Column("title")] public string Title { get; set; }
        [Column("brief")] public string Brief { get; set; }
        [Column("website")] public string Website { get; set; }
        [Column("hashtags")] public string[] Hashtags { get; set; }
        
        public List<Post> Posts { get; set; }
    }

    [Table("post")]
    public class Post
    {
        [Column("id")] public int Id { get; set; }
        [Column("post_id")] public int PostId { get; set; }
        [Column("paid")] public bool Paid { get; set; }
        [Column("rejected")] public bool Rejected { get; set; }
        [Column("postponed")] public bool Postponed { get; set; }
        [Column("requested_price")] public int RequestedPrice { get; set; }
        [Column("publisher_id")] public int PublisherId { get; set; }
        [Column("rejection_reasons")] public EnumsForBrands.PostRejectionReason[] RejectionReasons { get; set; }
        [Column("brand_id")] public int BrandId { get; set; }
        [Column("paid_price")] public int? PaidPrice { get; set; }
        [Column("submitted_at")] public DateTime SubmittedAt { get; set; }
        [Column("paid_at")] public DateTime? PaidAt { get; set; }
        [Column("rejected_at")] public DateTime? RejectedAt { get; set; }
        [Column("postponed_at")] public DateTime? PostponedAt { get; set; }

        public Publisher Publisher { get; set; }
        public Brand Brand { get; set; }
    }

}