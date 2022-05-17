using PackageTrackerAPI.Entities;

namespace PackageTrackerAPI.Persistence.Repository
{
    public interface IPackageRepository
    {
        List<Package> GetAll();
        Package GetByCode(string code);
        void Add(Package package);
        void Update(Package package);
        PackageUpdate GetByUpdateId(Package package, int updateId);
        void Remove(Package package);
    }
}
