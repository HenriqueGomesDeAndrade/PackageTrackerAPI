using PackageTrackerAPI.Entities;

namespace PackageTrackerAPI.Persistence
{
    public class PackageTrackerContext
    {
        public PackageTrackerContext()
        {
            Packages = new List<Package>();   
        }

        public List<Package> Packages { get; set; }
    }
}
    