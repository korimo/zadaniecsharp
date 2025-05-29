using AiWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.IO.Compression;
using Microsoft.Extensions.Logging;


[ApiController]
[Route("api/[controller]")]
public class DownloadController : ControllerBase
{
    private readonly WebsiteCache _cache;
    private readonly ILogger<DownloadController> _logger;

    public DownloadController(IMemoryCache cache,ILogger<DownloadController> logger)
    {
        _cache = new WebsiteCache(cache);
         _logger = logger;
    }

		[HttpGet]
		public IActionResult Download()
		{
		    var html = _cache.GetHtml();
		    var images = _cache.GetAllImages();

		    if (string.IsNullOrEmpty(html) || images.Count == 0)
		    {
		        return BadRequest("Cache is empty or incomplete.");
		    }

		    using var archiveStream = new MemoryStream();
		    using (var archive = new System.IO.Compression.ZipArchive(archiveStream, System.IO.Compression.ZipArchiveMode.Create, true))
		    {
		        // HTML súbor
		        var htmlEntry = archive.CreateEntry("index.html");
		        using (var writer = new StreamWriter(htmlEntry.Open()))
		        	writer.Write(html);

		        // Všetky obrázky
		        foreach (var (filename, bytes) in images)
		        {
				    	  _logger.LogInformation($"📥 {filename} will be written to zip", "========" );
			        	var entry = archive.CreateEntry($"images/{filename}.png");
		            using (var entryStream = entry.Open())
		            {
		                entryStream.Write(bytes, 0, bytes.Length);
		            }
						        
				    	  _logger.LogInformation($"📥 {{filename}} written to zip", "========" );
  				   }
        }
    
		    archiveStream.Position = 0;
		    return File(archiveStream.ToArray(), "application/zip", "web.zip");
		}
}
