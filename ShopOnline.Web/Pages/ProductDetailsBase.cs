using Microsoft.AspNetCore.Components;
using ShopOnline.Models.Dto;
using ShopOnline.Web.Services.Contracts;

namespace ShopOnline.Web.Pages
{
	public class ProductDetailsBase: ComponentBase
	{
        [Inject]
        public IProductService ProductService{ get; set; }

		[Inject]
		public IShoppingCartService ShoppingCartService { get; set; }

		[Inject]
		public NavigationManager NavigationManager { get; set; }

		public int currentCount { get; set; } = 1;
		
		public void IncrementCount()
		{
			currentCount++;
		}

		public void DecrementCount()
		{
			if (currentCount > 1)
			{
				currentCount--;
			}
		}

		[Parameter]
		public int ProductId { get; set; }

        protected ProductDto Product { get; set; }

        public string ErrorMessage { get; set; }

        protected override async Task OnInitializedAsync()
		{
			try
			{
				Product = await ProductService.GetAsync(ProductId);
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}
		}

		protected async Task AddToCart_Click(CartItemToAddDto item)
		{
			try
			{
				var cartItemDto = await ShoppingCartService.AddItemAsync(item);
				NavigationManager.NavigateTo("/ShoppingCart");
				 
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}
		}

	}
}
