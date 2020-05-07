using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SAAsProject.Models;
using System.Configuration;
using SaasEcom.Core.DataServices.Storage;
using SaasEcom.Core.Infrastructure.PaymentProcessor.Stripe;
using Microsoft.AspNet.Identity.Owin;
using SaasEcom.Core.Infrastructure.Facades;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using SAAsProject.Views.SaasEcom.ViewModels;

namespace SAAsProject.Controllers
{
    public class NotesController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        private SubscriptionsFacade _subscriptionsFacade;
        private SubscriptionsFacade SubscriptionsFacade
        {
            get
            {
                return _subscriptionsFacade ?? (_subscriptionsFacade = new SubscriptionsFacade(
                    new SubscriptionDataService<ApplicationDbContext, ApplicationUser>
                        (HttpContext.GetOwinContext().Get<ApplicationDbContext>()),
                    new SubscriptionProvider(ConfigurationManager.AppSettings["StripeApiSecretKey"]),
                    new CardProvider(ConfigurationManager.AppSettings["StripeApiSecretKey"],
                        new CardDataService<ApplicationDbContext, ApplicationUser>(Request.GetOwinContext().Get<ApplicationDbContext>())),
                    new CardDataService<ApplicationDbContext, ApplicationUser>(Request.GetOwinContext().Get<ApplicationDbContext>()),
                    new CustomerProvider(ConfigurationManager.AppSettings["StripeApiSecretKey"]),
                    new SubscriptionPlanDataService<ApplicationDbContext, ApplicationUser>(Request.GetOwinContext().Get<ApplicationDbContext>()),
                    new ChargeProvider(ConfigurationManager.AppSettings["StripeApiSecretKey"])));
            }
        }

        // GET: Notes
        public async Task<ActionResult> Index()
        {
            var userId = User.Identity.GetUserId();

            var userNotes =
                
                  await  db.Users.Where(u => u.Id == userId)
                        .Include(u => u.Notes)
                        .SelectMany(u => u.Notes)
                        .ToListAsync();

            return View(userNotes);
        }

        // GET: Notes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userId = User.Identity.GetUserId();
            ICollection<Note> userNotes = (
                 await db.Users.Where(u => u.Id == userId)
                .Include(u => u.Notes).Select(u => u.Notes)
                .FirstOrDefaultAsync());

            if (userNotes == null)
            {
                return HttpNotFound();
            }

            Note note = userNotes.FirstOrDefault(n => n.Id == id);
            if (note == null)
            {
                return HttpNotFound();
            }
            return View(note);
        }

        // GET: Notes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Notes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Title,Text,CreatedAt")] Note note)
        {
            if (ModelState.IsValid)
            {
                if (await UserHasEnoughSpace(User.Identity.GetUserId()))
                {
                    note.CreatedAt = DateTime.UtcNow;

                    // The note is added to the user object so the Foreign Key is saved too
                    var userId = User.Identity.GetUserId();
                    var user = await this.db.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
                    user.Notes.Add(note);

                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData.Add("flash", new FlashWarningViewModel("You can not add more notes, upgrade your subscription plan or delete some notes."));
                }
            }

            return View(note);
        }
        private async Task<bool> UserHasEnoughSpace(string userId)
        {
            var subscription = (await SubscriptionsFacade.UserActiveSubscriptionsAsync(userId)).FirstOrDefault();

            if (subscription == null)
            {
                return false;
            }

            var userNotes = await db.Users.Where(u => u.Id == userId).Include(u => u.Notes).Select(u => u.Notes).CountAsync();

            return subscription.SubscriptionPlan.GetPropertyInt("MaxNotes") > userNotes;
        }
        // GET: Notes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (!await NoteBelongToUser(User.Identity.GetUserId(), noteId: id.Value))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Note note = await db.Notes.FindAsync(id);
            if (note == null)
            {
                return HttpNotFound();
            }
            return View(note);
        }

        // POST: Notes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Title,Text,CreatedAt")] Note note)
        {
            if (ModelState.IsValid && await NoteBelongToUser(User.Identity.GetUserId(), note.Id))
            {
                db.Entry(note).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(note);
        }

        // GET: Notes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (!await NoteBelongToUser(User.Identity.GetUserId(), noteId: id.Value))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Note note = await db.Notes.FindAsync(id);
            if (note == null)
            {
                return HttpNotFound();
            }
            return View(note);
        }

        // POST: Notes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            if (!await NoteBelongToUser(User.Identity.GetUserId(), noteId: id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Note note = await db.Notes.FindAsync(id);
            db.Notes.Remove(note);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        private async Task<bool> NoteBelongToUser(string userId, int noteId)
        {
            return await db.Users.Where(u => u.Id == userId).Where(u => u.Notes.Any(n => n.Id == noteId)).AnyAsync();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
