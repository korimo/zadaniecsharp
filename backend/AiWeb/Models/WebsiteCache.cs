using Microsoft.Extensions.Caching.Memory;

namespace AiWeb.Models
{
	public class WebsiteCache
	{
	    private readonly IMemoryCache _cache;
	    private string _sessionId = "default";

	    public WebsiteCache(IMemoryCache cache)
	    {
	        _cache = cache;
	    }

	    public WebsiteCache(IMemoryCache cache, string sessionId)
	    {
	        _cache = cache;
	        _sessionId = sessionId;
	    }

	    public void SetSession(string sessionId)
	    {
	        _sessionId = sessionId;
	    }

	    private string CacheKey => $"current:{_sessionId}";

	    private WebsiteCacheData GetOrCreateData()
	    {
	        if (!_cache.TryGetValue(CacheKey, out WebsiteCacheData data))
	        {
	            data = new WebsiteCacheData();
	            _cache.Set(CacheKey, data);
	        }
	        return data;
	    }

	    public void Clear() => _cache.Remove(CacheKey);

	    public void SetHtml(string html) => GetOrCreateData().Html = html;

	    public string? GetHtml() => GetOrCreateData().Html;

	    public void SetImage(string key, byte[] data) => GetOrCreateData().Images[key] = data;

	    public byte[]? GetImage(string key) =>
	        GetOrCreateData().Images.TryGetValue(key, out var data) ? data : null;


			public Dictionary<string, byte[]> GetAllImages() => GetOrCreateData().Images;
			
			public Dictionary<string, string> GetAllTexts() => GetOrCreateData().Texts;

	    public void SetText(string key, string text) => GetOrCreateData().Texts[key] = text;

	    public string? GetText(string key) =>
	        GetOrCreateData().Texts.TryGetValue(key, out var text) ? text : null;
	}
	public class WebsiteCacheData
	{
	    public string Html { get; set; } = string.Empty;
	    public Dictionary<string, byte[]> Images { get; set; } = new();
	    public Dictionary<string, string> Texts { get; set; } = new();
	}
}


