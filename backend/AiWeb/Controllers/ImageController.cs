// Controllers/ImageController.cs
using AiWeb.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AiWeb.Controllers
{
    [ApiController]
    [Route("api/image")]
    public class ImageController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<ImageController> _logger;
				private readonly WebsiteCache _cache;

        public ImageController(IConfiguration configuration, ILogger<ImageController> logger
				                				 ,IMemoryCache cache
																	)
        {
            _httpClient = new HttpClient();
            _apiKey = configuration["OpenAI:ApiKey"] ?? throw new Exception("Missing OpenAI API Key");
            _logger = logger;
  				  _cache = new WebsiteCache(cache);

        }


				[HttpGet("{imageType}")]
				public async Task<IActionResult> GetImage([FromQuery] string prompt, string imageType)
				{
				    var cacheKey = imageType;
				    byte[] cachedBytes = _cache.GetImage(cacheKey);
				    if (cachedBytes != null) {
				    	  _logger.LogInformation($"📥 Key from cache : {imageType}", "Prompt: {{prompt}}" );
				        return File(cachedBytes, "image/png");
				      }
				    
		    	  _logger.LogInformation($"📥 Key uncached : {imageType}", "Prompt: {{prompt}}" );

				    var generated = await GenerateImage(prompt, imageType) as FileContentResult;
				    if (generated == null) return StatusCode(500, "Chyba pri generovaní obrázku");

				    var imgBytes = generated.FileContents;

				    if (imgBytes != null)
				        _cache.SetImage(cacheKey, imgBytes);


				    return File(imgBytes, "image/png");
				}




        private async Task<IActionResult> GenerateImage(string prompt, string imageType)
        {

                string aiPrompt =  imageType switch
									{
									    "logo" => $"Vektorové logo pre tému: {prompt} v modernom štýle",
									    "feature" => $"Realistická Fotografia pre sekciu webu na tému: {prompt}, v modernom štýle",
									    "benefit1" or "benefit2" or "benefit2" => $"Fotografia niecoho lákavého na tému: {prompt}",
									    _   => $"Fotografia na tému: {prompt}" // default (otherwise)
									};


           try
            {
                var requestBody = new
                {
                    model = "dall-e-2",
                    prompt = aiPrompt,
                    n = 1,
                    size = "512x512",
                    response_format = "b64_json"
                };

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("https://api.openai.com/v1/images/generations", content);

                var responseJson = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("❌ Chyba pri volaní OpenAI: {Response}", responseJson);
                    return BadRequest("OpenAI API error");
                }

                using var doc = JsonDocument.Parse(responseJson);
                var base64 = doc.RootElement.GetProperty("data")[0].GetProperty("b64_json").GetString();
                var bytes = Convert.FromBase64String(base64);
                return File(bytes, "image/png");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Chyba pri generovaní loga");
                return StatusCode(500, "Chyba pri generovaní loga");
            }
        }
    }
}
