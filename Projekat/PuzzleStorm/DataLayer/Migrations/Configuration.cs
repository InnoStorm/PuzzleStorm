using System.Data.Entity.Migrations;
using DataLayer.Persistence;

namespace DataLayer.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<StormContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(StormContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
        }
    }
}
