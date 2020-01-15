using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SchoolManagement.Features.Shared.Components.SelectComponent
{
    public class SelectViewModel
    {
        public SelectViewModel()
        {
            InitList();
        }

        public SelectViewModel(string id, string @for, bool isMultiple) : this()
        {
            Id = id;
            For = @for;
            IsMultiple = isMultiple;
        }
        public SelectViewModel(string @for, int? value) : this(@for, @for, false)
        {
            Value = value?.ToString() ??"";
        }
        public SelectViewModel(string @for, string value) : this(@for, @for, false)
        {
            Value = value;
        }
        public SelectViewModel(string @for, List<string> value) : this(@for, @for, true)
        {
            SelectedValues = value;
        }

        public List<string> SelectedValues { get; set; }

        public string Value { get; set; }

        public string For { get; set; }

        public string Id { get; set; }

        public bool IsMultiple { get; set; }

        public List<SelectListItem> Items { get; private set; }

        public SelectViewModel WithItems(List<SelectListItem> items)
        {
            items.ForEach(x =>
            {
                if (SelectedValues.Contains(x.Value) || Value == x.Value)
                {
                    x.Selected = true;
                }
            });
            Items = items;
            return this;
        }

        private void InitList()
        {
            Items = new List<SelectListItem>();
            SelectedValues = new List<string>();
        }
    }
}
