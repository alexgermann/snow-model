using Newtonsoft.Json.Linq;
using SnowModel_Demo.Models;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SnowModel_Demo.Services
{
	internal class WatsonServiceCall
	{
		const string _apiKey = "LDs934rlS6tzvDQqYIAdt6zd_v0intLVah8xqCfQQQnq";
		const string _classifierVersion = "2018-03-19";
		const string _classifierID = "NewModelxFinalxxFINALx5.0_956060399";
		const string _threshold = "0.0";
		const string _serviceURL = "https://gateway.watsonplatform.net/visual-recognition/api/v3/classify";

		readonly HttpClient client = new HttpClient();

		public WatsonServiceCall()
		{
			var authInfo = $"apikey:{_apiKey}";
			authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authInfo);
		}

		public async Task<ClassModel> GetClassification(byte[] imageData, string fileName)
		{
			MultipartFormDataContent form = new MultipartFormDataContent();
			form.Add(new StreamContent(new MemoryStream(imageData)), "images_file", fileName);
			form.Add(new StringContent(_threshold), "threshold");
			form.Add(new StringContent(_classifierID), "classifier_ids");

			var response = await client.PostAsync($"{_serviceURL}?version={_classifierVersion}", form);
			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsAsync<object>();
				dynamic data = JObject.Parse(content.ToString());
				var classes = data.images[0].classifiers[0].classes;
				var ret = classes[0].ToObject<ClassModel>();

				if (ret.score < 0.5)
				{
					ret.@class = "No Snow";
					ret.score = 1 - ret.score;
				}

				return ret;
			}
			return new ClassModel();
		}
	}
}
