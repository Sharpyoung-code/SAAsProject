﻿using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SaasEcom.Core.Models;
using SaasEcom.Core.DataServices;
using System.Collections.Generic;

namespace SAAsProject.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : SaasEcomUser
    {
        public virtual ICollection<Note> Notes { get; set; }
        public virtual BillingAddress BillingAddress { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {

            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : SaasEcomDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("NoteConnection")
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>().Map(m => m.MapInheritedProperties());
            base.OnModelCreating(modelBuilder);
        }

        public System.Data.Entity.DbSet<SAAsProject.Models.Note> Notes { get; set; }
    }
}