using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using RazorPages.protos;
using Grpc.Core;

namespace RazorPages.Pages.Recipes;

public class DeleteRecipeModel : PageModel
{
	[TempData]
	public string? Message { get; set; }
	[BindProperty(SupportsGet = true)]
	public Guid RecipeId { get; set; } = Guid.Empty;
	public Recipe Recipe { get; set; } = new();
	private readonly RecipeService.RecipeServiceClient _recipeServiceClient;

	public DeleteRecipeModel(RecipeService.RecipeServiceClient recipeServiceClient) =>
			_recipeServiceClient = recipeServiceClient;

	public async Task<IActionResult> OnGet()
	{
		try
		{
			Recipe.Id = RecipeId.ToString();
			var response = await _recipeServiceClient.ReadRecipeAsync(Recipe);
			if (response == null)
				return NotFound();
			Recipe = response;
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

	public async Task<IActionResult> OnPostAsync()
	{
		try
		{
			Recipe.Id = RecipeId.ToString();
			var response = await _recipeServiceClient.DeleteRecipeAsync(Recipe);
			Message = "Successfully Deleted";
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
		return RedirectToPage("./List");
	}
}
