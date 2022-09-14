using Grpc.Core;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using Server.protos;


namespace Server.Services;

public class CategoryServices : CategoryService.CategoryServiceBase
{

	private static List<string> _categoriesList = new();
	private static List<Recipe> _recipesList = new();
	private readonly string _recipesFile = Path.Combine(Environment.CurrentDirectory, "Recipe.json");
	private readonly string _categoriesFile = Path.Combine(Environment.CurrentDirectory, "Category.json");
	public async Task LoadDataAsync()
	{
		// load previous recipes if exists
		if (File.Exists(_recipesFile))
		{
			var jsonRecipesString = await File.ReadAllTextAsync(_recipesFile);
			_recipesList = JsonConvert.DeserializeObject<List<Recipe>>(jsonRecipesString)! ?? new List<Recipe>();
		}
		else
		{
			File.Create(_recipesFile).Dispose();
		}

		// load previous categories if exists
		if (File.Exists(_categoriesFile))
		{
			var jsonCategoriesString = await File.ReadAllTextAsync(_categoriesFile);
			_categoriesList = JsonConvert.DeserializeObject<List<string>>(jsonCategoriesString)! ?? new List<string>();
		}
		else
		{
			File.Create(_categoriesFile).Dispose();
		}
	}

	public async Task SaveDataAsync()
	{
		await Task.WhenAll(
			File.WriteAllTextAsync(_recipesFile, JsonConvert.SerializeObject(
				_recipesList.OrderBy(o => o.Title).ToList(), Formatting.Indented)),

			File.WriteAllTextAsync(_categoriesFile, JsonConvert.SerializeObject(
				_categoriesList.OrderBy(o => o).ToList(), Formatting.Indented))
		);
	}

	public override async Task<ListCategoriesResponse> ListCategories(Empty request, ServerCallContext context)
	{
		await LoadDataAsync();
		ListCategoriesResponse response = new();
        response.Categories.AddRange(_categoriesList);
		return response;
	}

	public override async Task<Category> CreateCategory(Category request, ServerCallContext context)
	{
		await LoadDataAsync();

		if (request.NewTitle == null)
		{
			const string msg = "Invalid Category";
			throw new RpcException(new Status(StatusCode.InvalidArgument, msg));
		}

		if (_categoriesList.Find(r => r == request.NewTitle) is string category)
		{
			const string msg = "Category already exists";
			throw new RpcException(new Status(StatusCode.AlreadyExists, msg));
		}

		_categoriesList.Add(request.NewTitle);
		await SaveDataAsync();
		return request;
	}

	public override async Task<Category> UpdateCategory(Category request, ServerCallContext context)
	{
		await LoadDataAsync();

		if (_categoriesList.Find(r => r == request.OldTitle) is string oldCategory)
		{
			_categoriesList.Remove(oldCategory);
			_categoriesList.Add(request.NewTitle);
			foreach (var recipe in _recipesList)
			{
				if (recipe.Categories.Contains(oldCategory))
				{
					recipe.Categories.Remove(oldCategory);
					recipe.Categories.Add(request.NewTitle);
				}
			}
			await SaveDataAsync();
			return request;
		}

		const string msg = "Could not find category";
		throw new RpcException(new Status(StatusCode.NotFound, msg));
	}

	public override async Task<Category> DeleteCategory(Category request, ServerCallContext context)
	{
		await LoadDataAsync();

		if (_categoriesList.Find(r => r == request.NewTitle) is string oldCategory)
		{
			_categoriesList.Remove(oldCategory);
			foreach (Recipe recipe in _recipesList)
			{
				recipe.Categories.Remove(oldCategory);
			}
			await SaveDataAsync();
			return request;
		}

		const string msg = "Could not find category";
		throw new RpcException(new Status(StatusCode.NotFound, msg));
	}
}