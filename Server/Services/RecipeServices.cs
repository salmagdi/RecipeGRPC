using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Newtonsoft.Json;
using Server.protos;

namespace Server.Services;

public class RecipeServices : RecipeService.RecipeServiceBase
{
	private static List<Recipe> s_recipesList = new();
	private readonly string _recipesFile = "Recipe.json";

	public async Task LoadDataAsync()
	{
		// load previous recipes if exists
		if (File.Exists(_recipesFile))
		{
			var jsonRecipesString = await File.ReadAllTextAsync(_recipesFile);
			s_recipesList = JsonConvert.DeserializeObject<List<Recipe>>(jsonRecipesString)!;
		}
		else
		{
			File.Create(_recipesFile).Dispose();
		}
	}

	public async Task SaveDataAsync()
	{
		await File.WriteAllTextAsync(_recipesFile, JsonConvert.SerializeObject(
				s_recipesList.OrderBy(o => o.Title).ToList(), Formatting.Indented));
	}

	public override async Task<ListRecipesResponse> ListRecipes(Empty request, ServerCallContext context)
	{
		await LoadDataAsync();
		ListRecipesResponse response = new();
		response.Recipes.AddRange(s_recipesList);
		return response;
	}
	public override async Task<Recipe> GetRecipe(RecipeLookUpModel request, ServerCallContext context)
	{
		await LoadDataAsync();
		var selectedRecipe = s_recipesList.FirstOrDefault(x => x.Id == request.Id);
		if (selectedRecipe == null)
		{
			throw new RpcException(new Status(StatusCode.NotFound, "Recipe not found"));
		}
		else
		{
			return selectedRecipe;
		}
	}
	public override async Task<Recipe> CreateRecipe(Recipe recipe, ServerCallContext context)
	{
		await LoadDataAsync();

		if (recipe.Title == null)
		{
			const string msg = "Invalid Recipe";
			throw new RpcException(new Status(StatusCode.InvalidArgument, msg));
		}

		recipe.Id = Guid.NewGuid().ToString();
		s_recipesList.Add(recipe);
		await SaveDataAsync();
		return recipe;
	}

	public override async Task<Recipe> ReadRecipe(Recipe recipe, ServerCallContext context)
	{
		await LoadDataAsync();

		if (s_recipesList.Find(r => r.Id == recipe.Id) is Recipe foundRecipe)
		{
			return foundRecipe;
		}
		const string msg = "Could not find recipe";
		throw new RpcException(new Status(StatusCode.NotFound, msg));
	}

	public override async Task<Recipe> UpdateRecipe(Recipe recipe, ServerCallContext context)
	{
		await LoadDataAsync();

		if (s_recipesList.Find(r => r.Id == recipe.Id) is Recipe oldRecipe)
		{
			s_recipesList.Remove(oldRecipe);
			s_recipesList.Add(recipe);
			await SaveDataAsync();
			return recipe;
		}

		const string msg = "Could not find recipe";
		throw new RpcException(new Status(StatusCode.NotFound, msg));
	}

	public override async Task<Recipe> DeleteRecipe(Recipe recipe, ServerCallContext context)
	{
		await LoadDataAsync();

		if (s_recipesList.Find(r => r.Id == recipe.Id) is Recipe oldRecipe)
		{
			s_recipesList.Remove(oldRecipe);
			await SaveDataAsync();
			return recipe;
		}

		const string msg = "Could not find recipe";
		throw new RpcException(new Status(StatusCode.NotFound, msg));
	}
}
