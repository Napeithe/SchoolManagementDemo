﻿
namespace Model.Dto
{
    public class LoginViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public string Error { get; set; }
    }
}
