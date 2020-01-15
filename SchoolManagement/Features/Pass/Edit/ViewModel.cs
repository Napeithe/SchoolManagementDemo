using System;
using Model.Dto;

namespace SchoolManagement.Features.Pass.Edit
{
    public class ViewModel : IUpdatePass
    {
        public int MemberId { get; set; }
        public DateTime Start { get; set; }
        public int Price { get; set; }
        public int NumberOfEntry { get; set; }
        public bool WasPaid { get; set; }
        public int Used { get; set; }
        public int Id { get; set; }
        public bool IsStudent { get; set; }
    }
}
