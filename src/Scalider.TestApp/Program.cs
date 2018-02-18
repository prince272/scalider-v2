using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Scalider.Data.Entity;
using Scalider.Data.Repository;
using Scalider.Security;

namespace Scalider.TestApp
{

    public class Program
    {

        public static void Main(string[] args)
        {
            var db = new Db();
//            db.Database.EnsureDeleted();
//            db.Database.EnsureCreated();
//            
            var set = db.Set<Entity>();
//            var one = new Entity();
//            var two = new Entity2 {Parent = one};
//
//            set.AddRange(one, two);
//            db.SaveChanges();

            var two2 = set
                       .OfType<Entity2>()
                       .FirstOrDefault();

            var entry = db.Entry(two2);
        }

    }

    public class O : EfRepository<Db, Entity, long>
    {

        public O(Db dbContext)
            : base(dbContext)
        {
        }
        
    }

    public class Entity : IEntity<long>
    {

        [Column("__id")]
        public long Id { get; set; }
        
        public string Name { get; set; }
        
        public byte[] RowVersion { get; set; }

    }
    
    public class Entity2 : Entity {
    
        public Entity Parent { get; set; }
    
    }

    public class Db : DbContext
    {

        /// <inheritdoc />
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=./test.db");
            optionsBuilder.UseLoggerFactory(new LoggerFactory().AddConsole());
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entity>()
                .HasKey(t=>t.Id);

            modelBuilder.Entity<Entity>()
                        .Property(t=>t.RowVersion)
                        .IsConcurrencyToken();

            modelBuilder.Entity<Entity2>()
                        .HasOne(t => t.Parent)
                        .WithMany()
                .IsRequired();
        }

    }
    
}