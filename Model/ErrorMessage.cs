using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public static class PolishReadableMessage
    {
        public class Attributes
        {
            public const string RequiredId = "Id jest wymagane.";
            public const string RequiredName = "Nazwa jest wymagana.";
        }

        public class Auth
        {
            public const string WrongLoginOrPassword = "Błędny login lub hasło.";
            public const string AccountNotVerified = "Aktywuj konto poprzez zresetowanie hasła.";
            public const string AccountBlocked = "Konto zostało zablokowane.";
            public const string AccountNotActive = "Konto jest nieaktywne.";
            public const string PasswordDoesNotMatched = "Hasła nie pasują do siebie.";
            public const string RoleDoesNotExist = "Rola nie istnieje.";
            public const string AccountDoesNotExist = "Konto o podanym adresie nie istnieje.";
            public const string SendEmailError = "Problem z wysłaniem maila";

            public const string AccountIsLockedOut =
                "Konto zostało tymczaosowa zablokowane z powodów błędnych prób logowania.";

            public const string PasswordUsedInPast = "Podane hasło było już wykorzystane w przeszłości";
        }

        public class Account
        {
            public static string CannotActivateEmailAccountNotExist = "Nie można aktywować konta. Konto nie istnieje.";
            public static string UserHasActivatedAccountAlready = "Użytkownik ma już aktywowane konto.";
        }

        public class User
        {
            public static string CannotFindUser = "Nie można edytowac użytkownika. Podany użytkownik nie istnieje w systemie.";
            public static string NotPermissionToRole = "Nie posiadasz uprawnień aby dodać użytkownika z rolą {0}";
            public static string RoleIsNotSet = "Nie ustawiono roli użytkownika.";
            public static string Removed = "Użytkownik został usunięty z systemu.";
        }

        public class Room
        {
            public static string NameDuplicate = "Pokój o podanej nazwie już istnieje.";
            public static string RoomNotExist = "Pokój o podanym Id nie istnieje";
        }

        public class Anchors
        {
            public static string NotFound = "Prowadzący o danym identyfikatorze nie istnieje";
        }

        public class Assign
        {
            public static string UserNotFound = "Użytkownik o danym identyfikatorze nie istnieje.";
            public static string UserIsAssignedToRoleAlready = "Użytkownik już jest przypisany do tej roli.";
            public static string RoleNameIsNotSet = "Nie przekazano nazwy roli do przypisania.";
            public static string DontHavePermissionForUnassignThisRole = "Nie masz uprawnień aby usunąc tego użytkownika z tej roli.";
        }

        public static string UserNotFound = "Nie odnaleziono użytkownika z podanym identyfikatorem";

        public class GroupClass
        {
            public static string NotFound = "Nie odnaleziono grupy zajęciowej z podanym id.";
            public static string AddSuccess = "Dodano nową grupę zajęciową :)";
            public static string RemovedSuccess = "Grupa została usunięta";
            public static string EditSuccess = "Grupa została edytowana.";
        }

        public class Presence
        {
            public static string ParticipantNotFound = "Nie odnaleziono uczestnika.";

            public static string MakeUpClassNotFound =
                "Nie udało dodać się uczestnika do zajęć. Nie odnaleziono zajęć do odrabiania.";

            public static string ClassNotFound = "Nie odnaleziono zajęć z podanym Id";

            public static string RemoveWrongType =
                "Nie można zmienić statusu uczestnika. Zły typ obecności uczestnika.";
        }

        public class Member
        {
            public static string NotFound = "Nie odnaleziono uczestnika grupy z podanym Id";
        }

        public class Pass
        {
            public static string NotFound = "Nie odnaleziono karnetu z podanum Id";
        }
    }
}
