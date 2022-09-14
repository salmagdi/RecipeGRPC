using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPages.protos;
using System.ComponentModel.DataAnnotations;

namespace RazorPages.Pages.Recipes;

public class RecipeDetailsModel : PageModel
{
	[TempData]
	public string? Message { get; set; }

	public Recipe recipe { get; set; } = new();
	public IEnumerable<string> DetailedIngredients { get; set; } = new List<string>();
	public IEnumerable<string> DetailedInstructions { get; set; } = new List<string>();

	private readonly RecipeService.RecipeServiceClient _recipeServiceClient;
	private readonly CategoryService.CategoryServiceClient _categoryServiceClient;
	public RecipeDetailsModel(RecipeService.RecipeServiceClient recipeServiceClient, CategoryService.CategoryServiceClient categoryServiceClient)
	{
		_recipeServiceClient = recipeServiceClient;
		_categoryServiceClient = categoryServiceClient;
	}
	public async Task<IActionResult> OnGet(Guid id)
	{
		try
		{
		recipe = _recipeServiceClient.GetRecipe(new RecipeLookUpModel { Id = id.ToString() });
		}
		catch (Exception)
		{
			Message = "something went wrong";
		}
		return RedirectToPage("/List");
	}
}
