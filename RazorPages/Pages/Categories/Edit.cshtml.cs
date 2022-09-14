using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPages.protos;
using System.ComponentModel.DataAnnotations;
using Grpc.Core;

namespace RazorPages.Pages.Categories
{
	public class EditModel : PageModel
	{
		[TempData]
		public string? Message { get; set; }
		[FromRoute(Name = "category")]
		[Display(Name = "Old DeletedCategory Name")]
		public string OldCategory { get; set; } = string.Empty;
		[BindProperty]
		[Required]
		[Display(Name = "New DeletedCategory Name")]
		public string NewCategory { get; set; } = string.Empty;
		private readonly CategoryService.CategoryServiceClient _categoryServiceClient;

		public EditModel(CategoryService.CategoryServiceClient categoryServiceClient) =>
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
				var response = await _categoryServiceClient.UpdateCategoryAsync(new()
				{
					NewTitle = NewCategory,
					OldTitle = OldCategory
				});
				Message = "Created successfully";
			}
			catch (RpcException ex)
			{
				Message = ex.Status.Detail;
			}
			catch (Exception)
			{
				Message = "Something went wrong, Try again later";
			}
			return RedirectToPage("./List");
		}
	}
}