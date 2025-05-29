// Controllers/GenerateController.cs
using AiWeb.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Encodings.Web;

namespace AiWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenerateController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<GenerateController> _logger;
				private readonly WebsiteCache _cache;
				
        public GenerateController(
        				 IConfiguration configuration, 
        				 ILogger<GenerateController> logger,
        				 IMemoryCache cache
        				 )
        {
            _httpClient = new HttpClient();
            _apiKey = configuration["OpenAI:ApiKey"] ?? throw new Exception("Missing OpenAI API Key");
            _logger = logger;
  				  _cache = new WebsiteCache(cache);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PromptRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Prompt))
                return BadRequest("Prompt is required");

            _logger.LogInformation("📥 Prompt prijatý: {Prompt}", request.Prompt);

            var titlePrompt = $"Vygeneruj hlavný nadpis pre web s témou: {request.Prompt}. Použi slovenčinu.";
            var subtitlePrompt = $"Vygeneruj podnadpis pre web s témou: {request.Prompt}. Použi slovenčinu.";
            var textPrompt = $"Vygeneruj úvodný text pre web s témou: {request.Prompt}. Použi slovenčinu.";
            var benefitTitlePrompt = $"Vymysli benefit pre zákazníkov vo forme krátkeho Titulku  pre web s témou: {request.Prompt}. Použi slovenčinu.";
            var benefitPrompt1 = $"povedz to iste co je uvedene v titulku ";
            var benefitPrompt2 = $" ale viacerimi slovami/ par vetami . bez komentara. Použi slovenčinu.";
						_cache.Clear();
						
            try
            {
                var title = await GetOpenAIContentAsync("title",titlePrompt);

	            	_logger.LogInformation("📥 Nadpis: {Prompt}", title);
              	var subtitle = await GetOpenAIContentAsync("subtitle",subtitlePrompt);

                var text = await GetOpenAIContentAsync("text",textPrompt);
            		_logger.LogInformation("📥 Text: {Prompt}", text );

                var benefitTitle1 = await GetOpenAIContentAsync("benefitTittle1",benefitTitlePrompt);
                var benefitText1 = await GetOpenAIContentAsync("benefitPrompt1"
                										,benefitPrompt1 +
                										$" {benefitTitle1} " +
                											benefitPrompt2
                											);
                var benefitTitle2 = await GetOpenAIContentAsync("benefitTittle2",benefitTitlePrompt);
                var benefitText2 = await GetOpenAIContentAsync("benefitPrompt2"
                										,benefitPrompt1 +
                										$" {benefitTitle2} " +
                											benefitPrompt2
                											);
                var benefitTitle3 = await GetOpenAIContentAsync("benefitTittle3",benefitTitlePrompt);
                var benefitText3 = await GetOpenAIContentAsync("benefitPrompt3"
                										,benefitPrompt1 +
                										$" {benefitTitle3} " +
                											benefitPrompt2
                											);

								var imgSrcDict = new Dictionary<string, string>();                											

                var html = $$"""
<!DOCTYPE html>
<html lang="sk">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>{{request.Prompt}}</title>
  <style>
    body { font-family: Arial, sans-serif; margin: 0; padding: 0; }
    header { display: flex; align-items: center; justify-content: space-between; padding: 1rem; background: #e3e3e3; }
    nav ul { list-style: none; display: flex; gap: 1rem; padding: 0; margin: 0; }
    nav ul li { font-weight: bold; }
    .main { display: flex; padding: 2rem; gap: 2rem; }
    .cta { text-align: center; margin: 2rem; }
    .logo-title {  display: flex;  align-items: center;  gap: 2rem;  padding: 1.5rem 2rem;}
    .logo {  max-width: 120px;  height: auto;}
    .titles h1 {  font-size: 1.8rem;  margin: 0;}
    .titles h3 {  font-size: 1.2rem;  margin-top: 0.5rem;  font-weight: normal;}
    footer { background: #333; color: white; padding: 1rem; text-align: center; }
    img { max-width: 100%; height: auto; }

		.limited-width {		  max-width: 50%;		  text-align: justify;		}

		.benefits {		  display: flex;		  gap: 2rem;		  padding: 2rem;		}

		.benefit {		  flex: 1;		  text-align: center;		}

		.benefit img {		  width: 100%;		  max-width: 200px;		  margin-bottom: 1rem;		}

  </style>
</head>
<body>

<header>
  <div  class="logo-title">
""";

								imgSrcDict["logo"]=@Uri.EscapeDataString(request.Prompt);
								///api/image/logo?prompt=@Uri.EscapeDataString(request.Prompt)

		  					html = html + $$"""
 <img src="{logo}" alt="@HtmlEncoder.Default.Encode(request.Prompt)" class="logo"  style="width: 120px; height: auto;" />
  </div>
  <nav class="menu">
    <ul>
      <li>Domov</li>
      <li>Produkty</li>
      <li>Kontakt</li>
    </ul>
  </nav>
</header>

<section class="intro">
  <div  class="logo-title">
    <div class="titles">
	    <h1>{{title}}</h1>
	    <h3>{{subtitle}}</h3>
    </div>
  </div>
</section>


<div class="main">
  <div class="text limited-width">{{text}}</div>
  <div class="image">
""";
								imgSrcDict["feature"]=@Uri.EscapeDataString(request.Prompt);
								html+=$$"""
		<img src="{feature}" alt=$"{@HtmlEncoder.Default.Encode(request.Prompt)}" style="width: 2048px; height: auto;"/>      
  </div>
</div>

  <div class="cta">
    <button>Kontaktujte nás</button>
  </div>
  
  
  <section class="benefits">
  <div class="benefit">
""";
								imgSrcDict["benefit1"]=@Uri.EscapeDataString(benefitTitle1);
								html+=$$"""  
    <img src="{benefit1}" alt="Benefit 1">
    <h3>{{benefitTitle1}}</h3>
  <p>{{benefitText1}}</p>
  </div>
  <div class="benefit">
""";
								imgSrcDict["benefit2"]=@Uri.EscapeDataString(benefitTitle2);
								html+=$$"""  
    <img src="{benefit2}" alt="Benefit 2">
    <h3>{{benefitTitle2}}</h3>
  <p>{{benefitText2}}</p>
  </div>
  <div class="benefit">
""";
								imgSrcDict["benefit3"]=@Uri.EscapeDataString(benefitTitle3);
								html+=$$"""  
    <img src="{benefit3}" alt="Benefit 3">
    <h3>{{benefitTitle3}}</h3>
  <p>{{benefitText3}}</p>
  </div>
</section>


  
  <footer>
    <img src="{logo}" alt="Logo" style="width: 120px; height: auto;">
    <p>&copy; 2025 AI Web Builder</p>
    <div class="socials">
		<img src="/images/logoig.png" alt="Instagram logo" width="32" height="32">
		<img src="/images/logofb.png" alt="Facebook logo" width="32" height="32">
		<img src="/images/logoX.png" alt="X logo" width="32" height="32">
    </div>
  </footer>
<script>
  const imgs = document.images;
  let loaded = 0;
  const total = imgs.length;
  for (let i = 0; i < total; i++) {
    imgs[i].onload = imgs[i].onerror = () => {
      loaded++;
      if (loaded === total) {
        parent.postMessage({ allImagesLoaded: true }, "*");
      }
    };
  }
</script>
</body>
</html>
""";

								var cacheHtml = html;

								foreach (var kvp in imgSrcDict)
								{
								    var key = kvp.Key;
								    var prompt = kvp.Value;
								    
								    var placeholder = $"<img src=\"{{{key}}}\"";
								    var replacement1 = $"<img src=\"/api/image/{key}?prompt={prompt}\"";
								    var replacement2 = $"<img src=\"images/{key}.png\"";

								    html = html.Replace(placeholder, replacement1);    
								    cacheHtml=cacheHtml.Replace(placeholder, replacement2);
								}

            		_logger.LogInformation("📥 html: ", html );

								_cache.SetHtml(cacheHtml);

                return Ok(new { html });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Chyba pri generovaní HTML");
                var fallbackHtml = GetFallbackHtml(request.Prompt);
                return Ok(new { html = fallbackHtml, error = ex.Message });
            }
        }

				private async Task<string> GetOpenAIContentAsync(string key, string prompt)
				{
				    var cached = _cache.GetText(key);
				    if (!string.IsNullOrWhiteSpace(cached))
				        return cached;

				    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
				    var body = new
				    {
				        model = "gpt-4",
				        messages = new[] { new { role = "user", content = prompt } },
				        temperature = 0.7
				    };

				    var response = await _httpClient.PostAsync(
				        "https://api.openai.com/v1/chat/completions",
				        new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
				    );

				    response.EnsureSuccessStatusCode();
				    var json = await response.Content.ReadAsStringAsync();
				    using var doc = JsonDocument.Parse(json);
				    var result = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

				    if (!string.IsNullOrWhiteSpace(result))
				        _cache.SetText(key, result);

				    return result ?? "";
				}

        private string GetFallbackHtml(string prompt)
        {
            var safePrompt = HtmlEncoder.Default.Encode(prompt);
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"sk\">");
            sb.AppendLine("<head><meta charset=\"UTF-8\"><title>Fallback</title></head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<h1>Fallback stránka pre: " + safePrompt + "</h1>");
            sb.AppendLine("<p>Dočasne sa nepodarilo získať AI obsah.</p>");
            sb.AppendLine("</body></html>");
            return sb.ToString();
        }
    }

    public class PromptRequest
    {
        public string Prompt { get; set; }
    }
}
