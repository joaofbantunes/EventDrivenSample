using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BurgerJoint.Events;
using BurgerJoint.StoreFront.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BurgerJoint.StoreFront.Features.Orders
{
    public class Create : PageModel
    {
        private readonly BurgerDbContext _db;
        private readonly IOrderEventPublisher _orderCreatedPublisher;

        public Create(BurgerDbContext db, IOrderEventPublisher orderCreatedPublisher)
        {
            _db = db;
            _orderCreatedPublisher = orderCreatedPublisher;
        }

        public IReadOnlyCollection<SelectListItem> Dishes { get; private set; }

        [BindProperty] public Guid DishId { get; set; }
        [BindProperty] public string CustomerNumber { get; set; }

        public async Task OnGetAsync()
        {
            Dishes = await _db.Dishes
                .AsNoTracking()
                .Select(d => new SelectListItem(d.Name, d.Id.ToString()))
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var dish = await _db.Dishes.FindAsync(DishId);
            var order = Order.Create(dish, CustomerNumber);
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();
            
            // what if after completing the transaction, this fails?
            await _orderCreatedPublisher.PublishAsync(new OrderCreated
            {
                Id = Guid.NewGuid(),
                DishId = order.Dish.Id,
                OrderId = order.Id,
                OccurredAt = DateTime.UtcNow,
                CustomerNumber = CustomerNumber
            });
            
            return RedirectToPage(nameof(InProgress));
        }
    }
}