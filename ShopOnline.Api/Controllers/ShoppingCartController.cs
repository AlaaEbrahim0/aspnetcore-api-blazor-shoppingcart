using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ShopOnline.Api.Extensions;
using ShopOnline.Api.Models;
using ShopOnline.Api.Repositories.Contracts;
using ShopOnline.Models.Dto;

namespace ShopOnline.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ShoppingCartController : ControllerBase
	{
		private readonly IShoppingCartRepository shoppingCartRepository;
		private readonly IProductRepository productRepository;

		public ShoppingCartController(IShoppingCartRepository shoppingCartRepository,
							IProductRepository productRepository)
		{
			this.shoppingCartRepository = shoppingCartRepository;
			this.productRepository = productRepository;
		}

		[HttpGet("{userId}/GetItems")]
		public async Task<ActionResult<IEnumerable<CartItemDto>>> GetItems(string userId)
		{
			try
			{
				var cartItems = await shoppingCartRepository.GetAll(userId);

				if (cartItems == null)
				{
					return NoContent();
				}

				var products = await productRepository.GetAllAsync();

				if (products == null)
				{
					throw new Exception("Something went wrong when attempting to retrieve products");
				}

				var dtos = cartItems.ConvertToDto(products);

				return Ok(dtos);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
			}
		}


		[HttpGet("/{id:int}")]
		public async Task<ActionResult<CartItemDto>> GetItem(int id)
		{
			try
			{
				var cartItem = await shoppingCartRepository.GetItem(id);

				if (cartItem == null)
				{
					return NotFound();
				}

				var product = await productRepository.GetAsync(cartItem.ProductId);
				if (product == null)
				{
					return NotFound();
				}

				var dtos = cartItem.ConvertToDto(product);
				return Ok(dtos);

			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
			}
		}



		[HttpPost]
		public async Task<ActionResult<CartItemDto>> PostItem([FromBody] CartItemToAddDto cartItem)
		{
			try
			{
				var item = await shoppingCartRepository.Add(cartItem);

				if (item == null)
				{
					return NoContent();
				}

				var product = await productRepository.GetAsync(item.ProductId);
				if (product == null)
				{
					throw new Exception("Something went wrong when attempting to retrieve product");
				}

				var dto = item.ConvertToDto(product);
				return CreatedAtAction(nameof(GetItem), new { id = item.Id }, dto);
				
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
			}
		}

		[HttpDelete("{id:int}")]
		public async Task<ActionResult<CartItemDto>> Remove (int id)
		{
			try
			{
				var cartItem = await shoppingCartRepository.Delete(id);
				if (cartItem == null)
				{
					return NotFound();
				}

				var product = await productRepository.GetAsync(cartItem.ProductId);
				if (product == null)
				{
					return NotFound();
				}
				var dto = cartItem.ConvertToDto(product);
				return Ok(dto);


			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
				
			}
		}

		[HttpPatch("{id:int}")]
		public async Task<ActionResult<CartItemDto>> Update(int id, CartItemUpdateQtyDto cartItem)
		{
			try
			{
				var item = await shoppingCartRepository.Update(id, cartItem);
				if (item == null)
				{ 
					return NotFound();
				}

				var product = await productRepository.GetAsync(item.ProductId);
				if (product == null)
				{
					return NotFound();
				}

				var dto = item.ConvertToDto(product); 
				return Ok(dto);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
			}
		}
	}
}
