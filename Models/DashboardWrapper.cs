using System.Collections.Generic;

namespace WeddingPlanner.Models
{
    public class DashboardWrapper
    {
        public User LoggedInUser { get; set; }
        public List<Wedding> AllWeddings { get; set; }
    }
}