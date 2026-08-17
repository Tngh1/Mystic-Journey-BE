using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    // Initializes a new default instance of the Role class.
    public class Role
    {
        // Executes role id operation.
        public int RoleId { get; set; }

        // Executes name operation.
        public string Name { get; set; } = string.Empty;

        // Executes accounts operation.
        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
