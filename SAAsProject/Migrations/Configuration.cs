namespace SAAsProject.Migrations
{
    using SaasEcom.Core.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<SAAsProject.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(SAAsProject.Models.ApplicationDbContext context)
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
            var basicMonthly = new SubscriptionPlan
            {
                Id = "basic_monthly",
                Name = "Basic",
                Interval = SubscriptionPlan.SubscriptionInterval.Monthly,
                TrialPeriodInDays = 30,
                Price = 30.00,
                Currency = "USD"
            };
            basicMonthly.Properties.Add(new SubscriptionPlanProperty { Key = "MaxNotes", Value = "100" });

            var standardMonthly = new SubscriptionPlan
            {
                Id = "standard_monthly",
                Name = "Standard",
                Interval = SubscriptionPlan.SubscriptionInterval.Monthly,
                TrialPeriodInDays = 30,
                Price = 50.00,
                Currency = "USD"
            };
            standardMonthly.Properties.Add(new SubscriptionPlanProperty
            {
                Key = "MaxNotes",
                Value = "10000"
            });

            var premiumMonthly = new SubscriptionPlan
            {
                Id = "premium_monthly",
                Name = "Premium",
                Interval = SubscriptionPlan.SubscriptionInterval.Monthly,
                TrialPeriodInDays = 30,
                Price = 100.00,
                Currency = "USD"
            };
            premiumMonthly.Properties.Add(new SubscriptionPlanProperty
            {
                Key = "MaxNotes",
                Value = "1000000"
            });

            context.SubscriptionPlans.AddOrUpdate(
                sp => sp.Id,
                basicMonthly,
                standardMonthly,
                premiumMonthly);
        }
    }
}
