using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPages.protos;
namespace RazorPages.Pages.Categories;

    public class ListModel : PageModel
    {
	[TempData]
	public string? Message { get; set; }
	private readonly CategoryService.CategoryServiceClient _categoryServiceClient;

	public ListModel(CategoryService.CategoryServiceClient categoryServiceClient) =>
			_categoryServiceClient = categoryServiceClient;

	public List<string> CategoryList { get; set; } = new();

	public async Task<IActionResult> OnGetAsync()
	{
		try
		{
			var response = await _categoryServiceClient.ListCategoriesAsync(new());
			List<string> categories = response.Categories.ToList();
			if (categories != null)
				CategoryList = categories;
			return Page();
		}
		catch (Exception)
		{
			Message = "Something went wrong, Try again later";
			return RedirectToPage("./List");
		}
	}
}
