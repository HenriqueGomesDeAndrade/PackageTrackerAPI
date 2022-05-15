﻿using Microsoft.AspNetCore.Mvc;
using PackageTrackerAPI.Entities;
using PackageTrackerAPI.Models;
using PackageTrackerAPI.Persistence;

namespace PackageTrackerAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PackagesController : ControllerBase
    {
        private readonly PackageTrackerContext _context;
        public PackagesController(PackageTrackerContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_context.Packages);
        }

        [HttpPost]
        public IActionResult Post(AddPackageInputModel model)
        {
            var package = new Package(model.Title, model.Weight);
            _context.Packages.Add(package);

            return CreatedAtAction("GetByCode", new { code = package.Code }, package);
        }

        [HttpGet("{code}")]
        public IActionResult GetByCode(string code)
        {
            var package = _context.Packages.FirstOrDefault(p => p.Code == code);

            if (package == null)
            {
                return NotFound();
            }
            return Ok(package);
        }

        [HttpPost("{code}")]
        public IActionResult PostUpdate(string code, AddPackageUpdateInputModel model)
        {
            var package = _context.Packages.FirstOrDefault(p => p.Code == code);

            if (package == null)
            {
                return NotFound();
            }

            package.AddUpdate(model.Status, model.Delivered);

            _context.Packages.Add(package);

            return Ok(package);
        }
    }
}
