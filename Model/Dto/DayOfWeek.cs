using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Model.Dto
{
    public static class DayOfWeek
    {
        public static List<SelectListItem> GetItems => new List<SelectListItem>
        {
            new SelectListItem("Poniedziałki", "1"),
            new SelectListItem("Wtorki", "2"),
            new SelectListItem("Środy", "3"),
            new SelectListItem("Czwartki", "4"),
            new SelectListItem("Piątki", "5"),
            new SelectListItem("Soboty", "6"),
            new SelectListItem("Niedziele", "0"),
        };
    }
}
