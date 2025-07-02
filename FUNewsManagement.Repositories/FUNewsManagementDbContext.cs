using Microsoft.EntityFrameworkCore;
using FUNewsManagement.Models;

namespace FUNewsManagement.Repositories;

public class FUNewsManagementDbContext : DbContext
{
    public FUNewsManagementDbContext(DbContextOptions<FUNewsManagementDbContext> options) : base(options)
    {
    }

    public DbSet<SystemAccount> SystemAccounts { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<NewsArticle> NewsArticles { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<NewsTag> NewsTags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure composite key for NewsTag
        modelBuilder.Entity<NewsTag>()
            .HasKey(nt => new { nt.NewsArticleId, nt.TagId });

        // Configure relationships
        modelBuilder.Entity<NewsArticle>()
            .HasOne(n => n.Category)
            .WithMany(c => c.NewsArticles)
            .HasForeignKey(n => n.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<NewsArticle>()
            .HasOne(n => n.CreatedBy)
            .WithMany(a => a.CreatedNewsArticles)
            .HasForeignKey(n => n.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<NewsArticle>()
            .HasOne(n => n.UpdatedBy)
            .WithMany(a => a.UpdatedNewsArticles)
            .HasForeignKey(n => n.UpdatedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Category>()
            .HasOne(c => c.ParentCategory)
            .WithMany(c => c.SubCategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<NewsTag>()
            .HasOne(nt => nt.NewsArticle)
            .WithMany(n => n.NewsTags)
            .HasForeignKey(nt => nt.NewsArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<NewsTag>()
            .HasOne(nt => nt.Tag)
            .WithMany(t => t.NewsTags)
            .HasForeignKey(nt => nt.TagId)
            .OnDelete(DeleteBehavior.Cascade);

        // Seed data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed default admin account
        modelBuilder.Entity<SystemAccount>().HasData(
            new SystemAccount
            {
                AccountId = 1,
                AccountName = "Administrator",
                AccountEmail = "admin@FUNewsManagementSystem.org",
                AccountRole = 0, // Admin role
                AccountPassword = "@@abc123@@"
            }
        );

        // Seed default categories
        modelBuilder.Entity<Category>().HasData(
            new Category
            {
                CategoryId = 1,
                CategoryName = "General News",
                CategoryDescription = "General news and announcements",
                IsActive = true
            },
            new Category
            {
                CategoryId = 2,
                CategoryName = "Academic",
                CategoryDescription = "Academic related news",
                IsActive = true
            },
            new Category
            {
                CategoryId = 3,
                CategoryName = "Events",
                CategoryDescription = "University events and activities",
                IsActive = true
            }
        );
    }
}
