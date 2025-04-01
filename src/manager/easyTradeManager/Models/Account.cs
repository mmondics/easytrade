using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace easyTradeManager.Models
{
    public class Account
    {
        public int Id { get; set; }
        public int PackageId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string HashedPassword { get; set; }
        public string Origin { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime PackageActivationDate { get; set; }
        public Boolean AccountActive { get; set; }
        public string Address { get; set; }
    }
}