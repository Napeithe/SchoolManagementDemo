using System;

namespace Model.Dto
{
    public interface IUpdatePass : IPassDto
    {
        int MemberId { get; set; }
    }

    public interface IPassDto
    {
        DateTime Start { get; set; }
        int Price { get; set; }
        int NumberOfEntry { get; set; }
        bool WasPaid { get; set; }
        int Used { get; set; }
        int Id { get; set; }
        bool IsStudent { get; set; }
    }

    public class PassDto : IPassDto
    {
        public DateTime Start { get; set; }

        public int Price { get; set; }

        public int NumberOfEntry { get; set; }

        public bool WasPaid { get; set; }


        public int Used { get; set; }

        public int Id { get; set; }

        public bool IsStudent { get; set; }

        public int PassNumber { get; set; }

        public static PassDto FromPass(Model.Domain.Pass pass)
        {
            return new PassDto()
            {
                NumberOfEntry = pass.NumberOfEntry,
                Price = pass.Price,
                Start = pass.Start,
                WasPaid = pass.Paid,
                PassNumber = pass.PassNumber,
                Used = pass.Used,
                Id = pass.Id,
                IsStudent = pass.IsStudent
            };
        }
    }
}