using System.Data.Entity;

namespace AttendanceSyncApp.Models.Payroll
{
    public class PayrollDbContext : DbContext
    {
        public PayrollDbContext() : base("name=AttandanceSyncConnection")
        {
            Database.SetInitializer<PayrollDbContext>(null);
        }

        public DbSet<BonusType> BonusTypes { get; set; }
    }
}