using System.Collections.Generic;

namespace SchoolManagement.Features.Shared.DetailPartial
{

    public class ItemDetail
    {
        public enum DetailType
        {
            Simple, 
            Checkbox,
            List
        }

        private ItemDetail(string title)
        {
            Title = title;
        }
        public ItemDetail(string title, string value) :this(title)
        {
            Value = value;
            Type = DetailType.Simple;

        }
        public ItemDetail(string title, bool value) :this(title)
        {
            Value = value;
            Type = DetailType.Checkbox;
        }

        public ItemDetail(string title, List<string> values) : this(title)
        {
            Type = DetailType.List;
            Items = values;
        }

        public DetailType Type { get; }
        public List<string> Items { get; }
        public string Title { get; }
        public object Value { get; }
    }
}
