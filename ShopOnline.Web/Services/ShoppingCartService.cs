using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using ShopOnline.Models.Dto;
using ShopOnline.Web.Services.Contracts;

namespace ShopOnline.Web.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly HttpClient httpClient;

        public ShoppingCartService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public event EventHandler CartItemAdded;
        public event EventHandler CartItemUpdated;

		protected void OnCartItemAdded(EventArgs e)
		{
			CartItemAdded?.Invoke(this, e);
		}

        protected void OnCartItemUpdated(EventArgs e)
        {
            CartItemUpdated?.Invoke(this, e);
        }

		public async Task<CartItemDto> AddItemAsync(CartItemToAddDto item)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync($"api/ShoppingCart", item);

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return default;
                    }

                    OnCartItemAdded(EventArgs.Empty);
                    return await response.Content.ReadFromJsonAsync<CartItemDto>();
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Http Status: {response.StatusCode} - Message - {message}");
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<CartItemDto>> GetCartItemsAsync(string userId)
        {
            try
            {
                var response = await httpClient.GetAsync($"api/ShoppingCart/{userId}/GetItems");
                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return Enumerable.Empty<CartItemDto>().ToList();
                    }

                    return await response.Content.ReadFromJsonAsync<List<CartItemDto>>();
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Http Status Code {response.StatusCode} - Message {message}");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<CartItemDto> RemoveItemAsync(int id)
        {
            try
            {
                var response = await httpClient.DeleteAsync($"api/ShoppingCart/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<CartItemDto>();
                }
                return default(CartItemDto);
            }
            catch (Exception)
            {

                throw;
            }
        }

		public async Task<CartItemDto> UpdateItemAsync(CartItemUpdateQtyDto item)
		{
            try
            {
                var jsonRequest = JsonConvert.SerializeObject(item);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json-patch+json");

                var response = await httpClient.PatchAsync($"api/ShoppingCart/{item.Id}", content); 
                if (response.IsSuccessStatusCode)
                {
                    OnCartItemUpdated(EventArgs.Empty);
                    return await response.Content.ReadFromJsonAsync<CartItemDto>();
                }
                return null;
            }
            catch (Exception ex)
            {

                throw;
            }
		}
	}
}
