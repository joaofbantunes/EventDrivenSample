using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BurgerJoint.StoreFront.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BurgerJoint.StoreFront.Features.Orders
{
    public class InProgress : PageModel
    {
        private readonly BurgerDbContext _db;

        public class OrderViewModel
        {
            public Guid Id { get; set; }

            public Status Status { get; set; }

            public DateTime CreatedAt { get; set; }
        }

        public InProgress(BurgerDbContext db)
        {
            _db = db;
        }

        public IReadOnlyCollection<OrderViewModel> Orders { get; set; }

        [BindProperty] public string CancelReason { get; set; }

        public async Task OnGetAsync()
        {
            Orders = await _db
                .Orders
                .AsNoTracking()
                .OrderByDescending(o => o.CreatedAt)
                .Take(20)
                .Select(o => new OrderViewModel {Id = o.Id, Status = o.Status, CreatedAt = o.CreatedAt})
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDeliverAsync(Guid orderId)
        {
            var order = await _db
                .Orders
                .Include(o => o.Dish)
                .SingleAsync(o => o.Id == orderId);

            order.Deliver();

            await _db.SaveChangesAsync();

            return RedirectToPage(nameof(InProgress));
        }

        public async Task<IActionResult> OnPostCancelAsync(Guid orderId)
        {
            var order = await _db
                .Orders
                .Include(o => o.Dish)
                .SingleAsync(o => o.Id == orderId);

            order.Cancel(CancelReason);

            await _db.SaveChangesAsync();

            return RedirectToPage(nameof(InProgress));
        }
    }
}