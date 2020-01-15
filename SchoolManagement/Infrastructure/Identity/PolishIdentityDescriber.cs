using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace SchoolManagement.Infrastructure.Identity
{
    public class PolishIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DefaultError()
        {
            return new IdentityError { Code = nameof(DefaultError), Description = $"Wystąpił nieznany błąd." };
        }

        public override IdentityError ConcurrencyFailure()
        {
            return new IdentityError
            {
                Code = nameof(ConcurrencyFailure),
                Description = "Optymistyczna awaria współbieżności, obiekt został zmodyfikowany."
            };
        }

        public override IdentityError PasswordMismatch()
        {
            return new IdentityError { Code = nameof(PasswordMismatch), Description = "Niepoprawne hasło." };
        }

        public override IdentityError InvalidToken()
        {
            return new IdentityError { Code = nameof(InvalidToken), Description = "Nieprawidłowy Token." };
        }

        public override IdentityError LoginAlreadyAssociated()
        {
            return new IdentityError
            { Code = nameof(LoginAlreadyAssociated), Description = "Użytkownik z tym loginem już istnieje." };
        }

        public override IdentityError InvalidUserName(string userName)
        {
            return new IdentityError
            {
                Code = nameof(InvalidUserName),
                Description = $"Nazwa użytkownika '{userName}' jest nieprawidłowa, może zawierać tylko litery lub cyfry."
            };
        }

        public override IdentityError InvalidEmail(string email)
        {
            return new IdentityError { Code = nameof(InvalidEmail), Description = $"Adres e-mail '{email}' jest nieprawidłowy.." };
        }

        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError
            { Code = nameof(DuplicateUserName), Description = $"Nazwa użytkownika '{userName}' jest już zajęta." };
        }

        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError
            { Code = nameof(DuplicateEmail), Description = $"Adres e-mail '{email}' jest już zajęty." };
        }

        public override IdentityError InvalidRoleName(string role)
        {
            return new IdentityError { Code = nameof(InvalidRoleName), Description = $"Nazwa roli '{role}' jest nieprawidłowa" };
        }

        public override IdentityError DuplicateRoleName(string role)
        {
            return new IdentityError
            { Code = nameof(DuplicateRoleName), Description = $"Nazwa roli '{role}' jest już zajęta." };
        }

        public override IdentityError UserAlreadyHasPassword()
        {
            return new IdentityError
            { Code = nameof(UserAlreadyHasPassword), Description = "Użytkownik ma już ustawione hasło." };
        }

        public override IdentityError UserLockoutNotEnabled()
        {
            return new IdentityError
            { Code = nameof(UserLockoutNotEnabled), Description = "Blokada nie jest włączona dla tego użytkownika." };
        }

        public override IdentityError UserAlreadyInRole(string role)
        {
            return new IdentityError
            { Code = nameof(UserAlreadyInRole), Description = $"Użytkownik już jest w roli '{role}'." };
        }

        public override IdentityError UserNotInRole(string role)
        {
            return new IdentityError { Code = nameof(UserNotInRole), Description = $"Użytkownik nie jest w roli '{role}'." };
        }

        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError
            { Code = nameof(PasswordTooShort), Description = $"Hasła muszą mieć co najmniej  {length} znaków." };
        }

        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresNonAlphanumeric),
                Description = "Hasła muszą mieć co najmniej jeden niealfanumeryczny znak."
            };
        }

        public override IdentityError PasswordRequiresDigit()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresDigit),
                Description = "Hasła muszą mieć co najmniej jedną cyfrę ('0'-'9')."
            };
        }

        public override IdentityError PasswordRequiresLower()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresLower),
                Description = "Hasła muszą zawierać co najmniej jedną małą literę ('a'-'z')."
            };
        }

        public override IdentityError PasswordRequiresUpper()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresUpper),
                Description = "Hasła muszą zawierać co najmniej jedną wielką literę ('A'-'Z')."
            };
        }
    }
}
