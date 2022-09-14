using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPages.protos;
using System.ComponentModel.DataAnnotations;
using Grpc.Core;

namespace RazorPages.Pages.Recipes;

public class EditRecipeModel : PageModel
{
	[TempData]
	public string? Message { get; set; }
	[BindProperty]
	[Required]
	public Recipe Recipe { get; set; } = new();
	public IEnumerable<string> Categories { get; set; } = Enumerable.Empty<string>();
	[BindProperty]
	public IEnumerable<string> ChosenCategories { get; set; } = Enumerable.Empty<string>();
	[BindProperty]
	public string Ingredients { get; set; } = string.Empty;
	[BindProperty]
	public string Instructions { get; set; } = string.Empty;
	private readonly RecipeService.RecipeServiceClient _recipeServiceClient;
	private readonly CategoryService.CategoryServiceClient _categoryServiceClient;

	public EditRecipeModel(RecipeService.RecipeServiceClient recipeServiceClient, CategoryService.CategoryServiceClient categoryServiceClient)
	{
		_recipeServiceClient = recipeServiceClient;
		_categoryServiceClient = categoryServiceClient;
	}
	public async Task<IActionResult> OnGetAsync(Guid recipeId)
	{
		await PopulateRecipeAndCategoriesAsync(recipeId);
		if (Recipe == null)
			return NotFound();
		Ingredients = string.Join(Environment.NewLine, Recipe.Ingredients);
		Instructions = string.Join(Environment.NewLine, Recipe.Instructions);
		return Page();
	}

	public async Task<IActionResult> OnPostAsync(Guid recipeId)
	{
		if (!ModelState.IsValid)
		{
			await PopulateRecipeAndCategoriesAsync(recipeId);
			return Page();
		}
		var recipe = new Recipe
		{
			Id = recipeId.ToString(),
			Title = Recipe.Title
		};
		recipe.Categories.AddRange(ChosenCategories);
		if (Ingredients != null)
			recipe.Ingredients.AddRange(Ingredients.Split(Environment.NewLine).ToList());
		if (Instructions != null)
			recipe.Instructions.AddRange(Instructions.Split(Environment.NewLine).ToList());

		try
		{
			var response = await _recipeServiceClient.UpdateRecipeAsync(recipe);
			Message = "Successfully Edited";
		}
		catch (RpcException ex)
		{
			Message = ex.Status.Detail;
		}
		catch (Exception)
		{
			Message = "Something went wrong please try again later";
		}
		return RedirectToPage("./List");
	}

	public async Task PopulateRecipeAndCategoriesAsync(Guid recipeId)
	{
		var categoriesResponse = await _categoryServiceClient.ListCategoriesAsync(new());
		if (categoriesResponse != null)
			Categories = categoriesResponse.Categories;
		Recipe.Id = recipeId.ToString();
		var recipeResponse = await _recipeServiceClient.ReadRecipeAsync(Recipe);
		if (recipeResponse != null)
			Recipe = recipeResponse;
	}
}