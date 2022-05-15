using Microsoft.AspNetCore.Mvc;
using PackageTrackerAPI.Entities;

namespace PackageTrackerAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PackagesController : ControllerBase
    {
        List<Package> packages = new List<Package>();

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(packages);
        }

        [HttpGet("{code}")]
        public IActionResult GetByCode(string code)
        {
            var package = packages.FirstOrDefault(p => p.Code == code);

            if (package == null)
            {
                return NotFound();
            }
            return Ok(package);
        }

        [HttpPost]
        public IActionResult Post(Package package)
        {
            package = new Package(package.Title, package.Weight);
            packages.Add(package);

            return CreatedAtAction("GetByCode", new {code = package.Code}, package);
        }

        [HttpPost("{code}")]
        public IActionResult PostUpdate(string code, PackageUpdate update)
        {
            var package = packages.FirstOrDefault(p => p.Code == code);
            package.Updates.Add(update);
            return Ok(package);
        }
    }
}
