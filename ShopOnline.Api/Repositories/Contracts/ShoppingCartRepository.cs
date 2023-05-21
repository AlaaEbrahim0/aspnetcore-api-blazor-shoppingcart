using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using ShopOnline.Api.Data;
using ShopOnline.Api.Models;
using ShopOnline.Models.Dto;

namespace ShopOnline.Api.Repositories.Contracts
{
	public class ShoppingCartRepository : IShoppingCartRepository
	{
		private readonly ApplicationDbContext context;

		public ShoppingCartRepository(ApplicationDbContext context)
        {
			this.context = context;
		}
		
		private async Task<bool> CartItemExists(int cartId, int productId)
		{
			return await context.CartItems.AnyAsync(c => c.CartId == cartId && c.ProductId == productId);
		}


        public async Task<CartItem> Add(CartItemToAddDto item)
		{
			if (await CartItemExists(item.CartId, item.ProductId) == false)
			{
				var product = await context.Products.FindAsync(item.ProductId);

				if (product != null)
				{
					var cartItem = new CartItem
					{
						CartId = item.CartId,
						ProductId = item.ProductId,
						Qty = item.Quantity
					};
					var result = await context.CartItems.AddAsync(cartItem);
					await context.SaveChangesAsync();
					return result.Entity;
				}
			}
			return null;
		}

		public async Task<CartItem> Delete(int id)
		{
			var cartItem = await context.CartItems.FindAsync(id);
			if (cartItem != null)
			{
				context.CartItems.Remove(cartItem);
				await context.SaveChangesAsync();
			}
			return cartItem;
		}

		public async Task<IEnumerable<CartItem>> GetAll(string userId)
		{
			return await (
					from cart in context.Carts
					join cartItem in context.CartItems
					on cart.Id equals cartItem.CartId
					where cart.UserId == userId
					select new CartItem
					{
						Id = cartItem.Id,
						CartId = cartItem.CartId,
						ProductId = cartItem.ProductId,
						Qty = cartItem.Qty
					}).ToListAsync();
		}

		public async Task<CartItem> GetItem(int id)
		{
			return await (
				from cart in context.Carts
				join cartItem in context.CartItems
				on cart.Id equals cartItem.CartId
				where cartItem.Id == id
				select new CartItem
				{
					Id = cartItem.Id,
					CartId = cartItem.CartId,
					ProductId = cartItem.ProductId,
					Qty = cartItem.Qty
				}).SingleOrDefaultAsync();
		}

		public async Task<CartItem> Update(int id, CartItemUpdateQtyDto item)
		{
			var cartItem = await context.CartItems.FindAsync(id);
			if (cartItem != null)
			{
				cartItem.Qty = item.Qty;
				await context.SaveChangesAsync();
			}
			return cartItem;
		}
	}
}
