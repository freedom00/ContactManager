using ContactManager.Authorization;
using ContactManager.Data;
using ContactManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ContactManager.Controllers
{
    public class ContactsController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IAuthorizationService authorizationService;

        public ContactsController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IAuthorizationService authorizationService)
        {
            this.context = context;
            this.userManager = userManager;
            this.authorizationService = authorizationService;
        }

        // GET: Contacts
        public async Task<IActionResult> Index()
        {
            var isAuthorized = User.IsInRole(Constants.ContactAdministratorsRole) || User.IsInRole(Constants.ContactManagersRole);
            var currentUserId = userManager.GetUserId(User);
            if (!isAuthorized)
            {
                return View(await context.Contact.Where(c => c.Status == ContactStatus.Approved || c.OwnerId == currentUserId).ToListAsync());
            }

            return View(await context.Contact.ToListAsync());
        }

        // GET: Contacts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await context.Contact.FirstOrDefaultAsync(m => m.ContactId == id);
            if (contact == null)
            {
                return NotFound();
            }

            var isAuthorized = User.IsInRole(Constants.ContactAdministratorsRole) || User.IsInRole(Constants.ContactManagersRole);
            var currentUserId = userManager.GetUserId(User);
            if (!isAuthorized && currentUserId != contact.OwnerId && contact.Status != ContactStatus.Approved)
            {
                return Forbid();
            }

            return View(contact);
        }

        //GET: Contacts/ChangeStatus/5?status=Approve
        public async Task<IActionResult> ChangeStatus(int? id, ContactStatus status)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await context.Contact.FirstOrDefaultAsync(m => m.ContactId == id);
            if (contact == null)
            {
                return NotFound();
            }

            var contactOperation = status == ContactStatus.Approved ? ContactOperations.Approve : ContactOperations.Reject;

            var isAuthorized = await authorizationService.AuthorizeAsync(User, contact, contactOperation);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            contact.Status = status;
            context.Contact.Update(contact);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Contacts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Contacts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ContactId,OwnerId,FirstName,LastName,Address,City,State,Zip,Email,Status")] Contact contact)
        {
            if (ModelState.IsValid)
            {
                contact.OwnerId = userManager.GetUserId(User);

                var isAuthorized = await authorizationService.AuthorizeAsync(User, contact, ContactOperations.Create);
                if (!isAuthorized.Succeeded)
                {
                    return Forbid();
                }

                context.Add(contact);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(contact);
        }

        // GET: Contacts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await context.Contact.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }

            var isAuthorized = await authorizationService.AuthorizeAsync(User, contact, ContactOperations.Update);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            return View(contact);
        }

        // POST: Contacts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ContactId,OwnerId,FirstName,LastName,Address,City,State,Zip,Email,Status")] Contact contact)
        {
            if (id != contact.ContactId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var isAuthorized = await authorizationService.AuthorizeAsync(User, contact, ContactOperations.Update);
                if (!isAuthorized.Succeeded)
                {
                    return Forbid();
                }

                try
                {
                    context.Update(contact);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactExists(contact.ContactId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(contact);
        }

        // GET: Contacts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await context.Contact
                .FirstOrDefaultAsync(m => m.ContactId == id);
            if (contact == null)
            {
                return NotFound();
            }

            var isAuthorized = await authorizationService.AuthorizeAsync(User, contact, ContactOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            return View(contact);
        }

        // POST: Contacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contact = await context.Contact.FindAsync(id);

            var isAuthorized = await authorizationService.AuthorizeAsync(User, contact, ContactOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            context.Contact.Remove(contact);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool ContactExists(int id)
        {
            return context.Contact.Any(e => e.ContactId == id);
        }
    }
}