using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SnowModel_Demo.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace SnowModel_Demo.Services
{
	internal class KerasServiceCall
	{
		const string _serviceURL = "https://chall3-classifier.azurewebsites.net";

		readonly HttpClient _client = new HttpClient();


		public async Task<ClassModel> GetClassification(string imageData)
		{
			var callModel = new KerasCallModel();
			callModel.image = imageData;
			var serialized = JsonConvert.SerializeObject(callModel);
			var form = new StringContent(serialized);

			_client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");

			var response = await _client.PostAsync(_serviceURL, form);
			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsAsync<object>();
				dynamic data = JObject.Parse(content.ToString());

				return new ClassModel { @class = (bool)data.snowOnRoad ? "Snow" : "No Snow", score = (double)data.confidence };
			}
			return new ClassModel();
		}
	}
}
