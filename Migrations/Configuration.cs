namespace TraballhoDM106.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using TraballhoDM106.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<TraballhoDM106.Models.TraballhoDM106Context>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(TraballhoDM106.Models.TraballhoDM106Context context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            context.Products.AddOrUpdate(
                    p => p.Id,
                    new Product
                    {
                        Id = 1,
                        name = "produto	1",
                        description = "descrição  produto 1",
                        color = "Branco",
                        model = "importado",
                        code = "COD1",
                        price	=	10,
                        weight = 1,
                        height = 1,
                        width = 1,
                        lenght = 1,
                        diameter = 1,
                        url = "google.com/1"},

                    new Product
                    {
                        Id = 2,
                        name = "produto	2",
                        description = "descrição  produto 2",
                        color = "Branco",
                        model = "nacional",
                        code = "COD2",
                        price = 20,
                        weight = 2,
                        height = 2,
                        width = 2,
                        lenght = 2,
                        diameter = 2,
                        url = "google.com/2"
                    },

                    new Product
                    {
                        Id = 3,
                        name = "produto	3",
                        description = "descrição  produto 3",
                        color = "Branco",
                        model = "importado",
                        code = "COD3",
                        price = 30,
                        weight = 3,
                        height = 3,
                        width = 3,
                        lenght = 3,
                        diameter = 3,
                        url = "google.com/3"
                    }

            );
        }
    }
}
