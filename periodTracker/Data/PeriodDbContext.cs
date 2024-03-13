using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using periodTracker.Models;
using periodTracker.Models.ViewModels;

namespace periodTracker.Data
{
    public class PeriodDbContext : IdentityDbContext<User>
    {
        public PeriodDbContext(DbContextOptions options) : base(options)
        {

        }
        public override DbSet<User> Users { get; set; }
        public DbSet<MenstrualCycle>MenstrualCycles { get; set; }
        public DbSet<MenstrualProfile> MenstrualProfiles { get; set; }
        public DbSet<Symptom> Symptoms { get; set; }
        public DbSet<UserSymptom> UserSymptoms { get; set; }
        //public DbSet<InputSymptom> InputSymptoms { get; set; }
        public DbSet<SymptomType> SymptomTypes { get; set; }
        public DbSet<Subtype> Subtypes { get; set; }
        public DbSet<Severity> Severities { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Define the one-to-many relationship between ApplicationUser and MenstrualCycle
            builder.Entity<MenstrualCycle>()
                .HasOne(mc => mc.User)
                .WithMany()
                .HasForeignKey(mc => mc.UserId);

            // Configure other entity configurations if necessary
        }
    }
    
}

