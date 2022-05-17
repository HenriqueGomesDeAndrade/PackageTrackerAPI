namespace PackageTrackerAPI.Entities
{
    public class PackageUpdate
    {
        public int Id { get; set; }
        public int PackageId { get; set; }
        public string Status { get; set; }
        public DateTime UpdateDate { get; set; }

        public PackageUpdate(int packageId, string status)
        {
            PackageId = packageId;
            Status = status;
            UpdateDate = DateTime.Now;
        }

        public void UpdatePackageUpdateStatus(string status)
        {
            Status = status;
        }
    }
}
