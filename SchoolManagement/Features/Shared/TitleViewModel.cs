namespace SchoolManagement.Features.Shared
{
    public class TitleViewModel
    {
        public TitleViewModel(string title, string backUrl="", object removeId = null)
        {
            Title = title;
            BackUrl = backUrl;
            RemoveId = removeId;
            if (!string.IsNullOrEmpty(BackUrl))
            {
                ShowBackButton = true;
            }

            if (removeId != null)
            {
                ShowRemoveButton = true;
            }
        }
        public bool ShowBackButton { get; set; }
        public string BackUrl { get; set; }
        public string Title { get; set; }
        public bool ShowRemoveButton { get; private set; }
        public object RemoveId { get; set; }
    }
}
