using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using RazorPages.protos;
using Grpc.Core;

namespace RazorPages.Pages.Recipes;
public class ListModel : PageModel
{
	[TempData]
	public string? Message { get; set; }
	private readonly RecipeService.RecipeServiceClient _recipeServiceClient;
	public List<Recipe> RecipeList { get; set; } = new();

	public ListModel(RecipeService.RecipeServiceClient recipeServiceClient)
	{
		_recipeServiceClient = recipeServiceClient;
	}

	public async Task<IActionResult> OnGetAsync()
	{
		try
		{
			var response = await _recipeServiceClient.ListRecipesAsync(new());
			List<Recipe> recipes = response.Recipes.ToList();
			if (recipes != null)
				RecipeList = recipes;
			return Page();
		}
		catch (RpcException ex)
		{
			Message = ex.Status.Detail;
			return RedirectToPage("./List");
		}
		catch (Exception)
		{
			Message = "Something went wrong please try again later";
			return RedirectToPage("./List");
		}
	}
}
