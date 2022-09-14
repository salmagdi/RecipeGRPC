using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using RazorPages.protos;

namespace RazorPages.Pages.Categories;

public class CreateModel : PageModel
{
	[TempData]
	public string? Message { get; set; }
	[BindProperty]
	[Required]
	[Display(Name = "DeletedCategory Name")]
	public string AddedCategory 
	{ get; set; } = string.Empty;
	private readonly CategoryService.CategoryServiceClient _categoryServiceClient;

	public CreateModel(CategoryService.CategoryServiceClient categoryServiceClient) =>
			_categoryServiceClient = categoryServiceClient;
	public void OnGet()
	{
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (!ModelState.IsValid)
			return Page();
		try
		{
			var response = await _categoryServiceClient.CreateCategoryAsync(new()
			{
				NewTitle = AddedCategory
			});
			Message = "Created successfully";
		}
		catch (Exception)
		{
			Message = "Something went wrong, Try again later";
		}
		return RedirectToPage("./List");
	}
}