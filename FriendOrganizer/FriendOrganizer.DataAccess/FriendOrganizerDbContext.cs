using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;
using FriendOrganizer.Model;

namespace FriendOrganizer.DataAccess
{
    public class FriendOrganizerDbContext :DbContext
    {
        public FriendOrganizerDbContext() : base("FriendOrganizerDb") { }

        public DbSet<Friend> Friends { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            /*Goes along with the exterior configuration class.*/
           // modelBuilder.Configurations.Add(new FriendConfigration());

            /*Fluent api, if used in the DbContext class(demo only)*/
            //modelBuilder.Entity<Friend>()
            //    .Property(f => f.FirstName)
            //    .IsRequired()
            //    .HasMaxLength(50);
        }

        /*Fluent api configuration used from an exterior class.(demo only)*/
        //public class FriendConfigration : EntityTypeConfiguration<Friend>
        //{
        //    public void FriendConfiguration()
        //    {
        //        Property(f => f.FirstName)
        //            .IsRequired()
        //            .HasMaxLength(50);
        //    }
        //}
    }
}
