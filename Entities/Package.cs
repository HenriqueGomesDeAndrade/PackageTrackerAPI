namespace PackageTrackerAPI.Entities
{
    public class Package
    {
        public int Id { get; private set; }
        public string Code { get; private set; }
        public string Title { get; private set; }
        public decimal Weight { get; private set; }
        public bool Delivered { get; private set; }
        public DateTime PostedAt { get; private set; }
        public List<PackageUpdate> Updates { get; private set; }

        public Package(string title, decimal weight)
        {
            Code = Guid.NewGuid().ToString();
            Title = title;
            Weight = weight;
            Delivered = false;
            PostedAt = DateTime.Now;
            Updates = new List<PackageUpdate>();
        }

        public void AddUpdate(string status, bool delivered)
        {
            if (Delivered)
            {
                throw new Exception("The package is already delivered");
            }
            var update = new PackageUpdate(Id, status);
            Updates.Add(update);
            Delivered = delivered;
        }
    }
}
