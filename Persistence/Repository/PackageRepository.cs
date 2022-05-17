using Microsoft.EntityFrameworkCore;
using PackageTrackerAPI.Entities;

namespace PackageTrackerAPI.Persistence.Repository
{
    public class PackageRepository : IPackageRepository
    {

        private readonly PackageTrackerContext _context;

        public PackageRepository(PackageTrackerContext context)
        {
            _context = context;
        }

        public void Add(Package package)
        {
            _context.Packages.Add(package);
            _context.SaveChanges();
        }

        public List<Package> GetAll()
        {
            return _context.Packages.ToList();
        }

        public Package GetByCode(string code)
        {
            return _context
                .Packages
                .Include(p => p.Updates)
                .FirstOrDefault(p => p.Code == code);
        }

        public PackageUpdate GetByUpdateId(Package package, int updateId)
        {
            return package
                .Updates
                .FirstOrDefault(pu => pu.Id == updateId);
        }

        public void Update(Package package)
        {
            _context.Entry(package).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Remove(Package package)
        {
            foreach(PackageUpdate update in package.Updates)
            {
                _context.PackageUpdates.Remove(update);
            }
            _context.Packages.Remove(package);
            _context.SaveChanges();
        }
    }
}
