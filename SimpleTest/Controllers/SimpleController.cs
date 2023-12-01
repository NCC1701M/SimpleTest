using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace SimpleTest.Controllers;

public class FileCreationRequest
{
	public string FileName { get; set; }
	public string Content { get; set; }
}


[Route("api/[controller]")]
[ApiController]
public class SimpleController : ControllerBase
{

	private readonly IWebHostEnvironment _env;

	public SimpleController(IWebHostEnvironment env)
	{
		_env = env;
	}

	[HttpPost]
	public virtual IActionResult Post([FromBody] FileCreationRequest req)
	{
		// FileName is something like 'myfile.txt'
		var path = Path.Join("assets", "tests", req.FileName);
		StoreFile(path, req.Content);
		return Ok();
	}

	private void StoreFile(string path, string content)
	{
		var fi = new FileInfo(_env.WebRootFileProvider.GetFileInfo(path).PhysicalPath);
		if (!fi.Directory.Exists)
			fi.Directory.Create();

		using (var writer = fi.CreateText())
		{
			writer.Write(content);
		}
	}
}
