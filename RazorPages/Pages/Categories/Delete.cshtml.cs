using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPages.protos;

namespace RazorPages.Pages.Categories;

public class DeleteModel : PageModel
{
	[TempData]
	public string? Message { get; set; }
	[FromRoute(Name = "category")]
	public string DeletedCategory { get; set; } = string.Empty;
	private readonly CategoryService.CategoryServiceClient _categoryServiceClient;

	public DeleteModel(CategoryService.CategoryServiceClient categoryServiceClient) =>
			_categoryServiceClient = categoryServiceClient;

	public void OnGet()
	{
	}

	public async Task<IActionResult> OnPostAsync()
	{
		try
		{
			var response = await _categoryServiceClient.DeleteCategoryAsync(new() { NewTitle = DeletedCategory });
			Message = "Created successfully";
		}
		catch (Exception)
		{
			Message = "Something went wrong, Try again later";
		}
		return RedirectToPage("./List");
	}
}