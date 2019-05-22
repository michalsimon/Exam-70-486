﻿namespace OdeToFood.Pages.Restaurants
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using OdeToFood.Core;
    using OdeToFood.Data;

    public class DetailModel : PageModel
    {
        private readonly IRestaurantData _restaurantData;

        public DetailModel(IRestaurantData restaurantData)
        {
            _restaurantData = restaurantData;
        }
        public Restaurant Restaurant { get; set; }
        [TempData]
        public string Message { get; set; }

        public IActionResult OnGet(int restaurantId)
        {
            Restaurant = _restaurantData.GetById(restaurantId);
            if(Restaurant == null)
            {
                return RedirectToPage("./NotFound");
            }

            return Page();
        }
    }
}