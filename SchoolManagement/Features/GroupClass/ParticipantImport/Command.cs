using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Domain;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SchoolManagement.Aggregates;
using SchoolManagement.Infrastructure;

namespace SchoolManagement.Features.GroupClass.ParticipantImport
{
    public class Command : IRequest<DataResult>
    {
        public IFormFile File { get; set; }
        public int GroupId { get; set; }
    }

    public class Handler : IRequestHandler<Command, DataResult>
    {
        private readonly UserManager<User> _userManager;
        private readonly SchoolManagementContext _context;

        public Handler(UserManager<User> userManager, SchoolManagementContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<DataResult> Handle(Command request, CancellationToken cancellationToken)
        {
            IFormFile file = request.File;
            if (file.Length > 0)
            {
                List<GroupUser> users = GetUsers(file);

                await CreateNewAccounts(cancellationToken, users);

                List<string> emails = users.Select(x => x.Email).ToList();
                List<User> usersToGroup = await _context.Users.Where(x => emails.Contains(x.Email)).ToListAsync(cancellationToken);

                await AddUsersToParticipantRoles(usersToGroup);

                await AddParticipantsToGroup(request, cancellationToken, users, usersToGroup);

                await _context.SaveChangesAsync(cancellationToken);
                return DataResult.Success();
            }

            return DataResult.Error("Plik jest pusty");
        }

        private async Task AddUsersToParticipantRoles(List<User> usersToGroup)
        {
            string participantRoleId =
                _context.Roles.Where(x => x.Name == Roles.Participant).Select(x => x.Id).FirstOrDefault();
            List<string> usersId = usersToGroup.Select(x => x.Id).ToList();

            List<string> usersWithoutParticipantRole = _context.UserRoles
                .Where(x => usersId.Contains(x.UserId))
                .GroupBy(x => x.UserId)
                .Where(x => x.All(a => a.RoleId != participantRoleId))
                .Select(x => x.Key).ToList();

            List<User> usersToAddRole = usersToGroup.Where(x => usersWithoutParticipantRole.Contains(x.Id)).ToList();

            foreach (User user in usersToAddRole)
            {
                await _userManager.AddToRoleAsync(user, Roles.Participant);
            }
        }

        private async Task AddParticipantsToGroup(Command request, CancellationToken cancellationToken,
            List<GroupUser> users, List<User> usersToGroup)
        {
            Model.Domain.GroupClass groupClass = await _context
                .GroupClass
                .Include(x => x.Participants)
                .Include(x=>x.Schedule)
                .ThenInclude(x=>x.PresenceParticipants)
                .Where(x => x.Id == request.GroupId)
                .FirstOrDefaultAsync(cancellationToken);

            GroupClassAggregate groupClassAggregate = GroupClassAggregate.FromState(groupClass);
            foreach (User user in usersToGroup)
            {
                if (groupClassAggregate.IsParticipantExists(user.Id)) continue;
                PassAggregate passAggregate = PassAggregate.New()
                    .WithStartDate(groupClass.StartClasses)
                    .WithPrice(groupClass.PassPrice)
                    .WithNumberOfEntry(groupClass.NumberOfClasses)
                    .WithParticipant(user);
                GroupUser groupUser = users.First(x => x.Email == user.Email);
                groupClassAggregate.AddParticipant(user, groupUser.Role, passAggregate);
            }

            _context.GroupClass.Update(groupClass);
        }

        private async Task CreateNewAccounts(CancellationToken cancellationToken, List<GroupUser> users)
        {
            List<string> existEmails = await _context.Users.Select(x => x.Email).ToListAsync(cancellationToken);

            IEnumerable<User> notExistsUsers = users.Where(x => !existEmails.Contains(x.Email)).Select(x => x.CreateUser());
            foreach (var newUser in notExistsUsers)
            {
                IdentityResult createResult = await _userManager.CreateAsync(newUser);
                if (createResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newUser, Roles.Participant);
                }
            }
        }

        private static List<GroupUser> GetUsers(IFormFile file)
        {
            List<GroupUser> users = new List<GroupUser>();
            using (var stream = file.OpenReadStream())
            {
                stream.Position = 0;
                string sFileExtension = Path.GetExtension(file.FileName).ToLower();
                ISheet sheet;
                if (sFileExtension == ".xls")
                {
                    HSSFWorkbook hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  
                    sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook  
                }
                else
                {
                    XSSFWorkbook hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  
                    sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook   
                }

                for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;
                    if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                    ParticipantRole participantRole = Enum.Parse<ParticipantRole>(GetCellValue(row, 5));
                    GroupUser user = new GroupUser
                    {
                        FirstName = GetCellValue(row, 0),
                        LastName = GetCellValue(row, 1),
                        Email = GetCellValue(row, 2),
                        PhoneNumber = GetCellValue(row, 3),
                        FacebookLink = GetCellValue(row, 4),
                        Role = participantRole
                    };

                    users.Add(user);
                }
            }

            return users;
        }

        private static string GetCellValue(IRow row, int index)
        {
            ICell cell = row.GetCell(index, MissingCellPolicy.RETURN_NULL_AND_BLANK);
            if (cell is null) 
                return "";
            cell.SetCellType(CellType.String);
            return cell.StringCellValue;
        }

        private class GroupUser
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string FacebookLink { get; set; }
            public ParticipantRole Role { get; set; }

            public User CreateUser()
            {
                return new User
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    Email = Email,
                    PhoneNumber = PhoneNumber,
                    UserName = Email,
                    FacebookLink = FacebookLink
                };
            }
        }
    }
}
