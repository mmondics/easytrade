using System;


namespace easyTradeLoginService.Models
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

        public Account()
        {
        }

        public Account(AccountRequest accountRequest)
        {
            PackageId = accountRequest.PackageId;
            FirstName = accountRequest.FirstName;
            LastName = accountRequest.LastName;
            Username = accountRequest.Username;
            Email = accountRequest.Email;
            HashedPassword = accountRequest.HashedPassword;
            Origin = accountRequest.Origin;
            CreationDate = DateTime.Now;
            PackageActivationDate = DateTime.Now;
            AccountActive = true;
            Address = accountRequest.Address;
        }
        public Account(SignupRequest signupRequest)
        {
            PackageId = signupRequest.PackageId;
            FirstName = signupRequest.FirstName;
            LastName = signupRequest.LastName;
            Username = signupRequest.Username;
            Email = signupRequest.Email;
            HashedPassword = signupRequest.Password;
            Origin = signupRequest.Origin;
            CreationDate = DateTime.Now;
            PackageActivationDate = DateTime.Now;
            AccountActive = true;
            Address = signupRequest.Address;
        }
    }
}
