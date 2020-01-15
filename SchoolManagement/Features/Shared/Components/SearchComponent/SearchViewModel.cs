namespace SchoolManagement.Features.Shared.Components.SearchComponent
{
    public class SearchViewModel
    {
        public string Id { get; }
        public string Url { get; }
        public string CallbackName { get; }
        public string GetExcludedDelegate { get; }

        public SearchViewModel(string id, string url, string callbackName = "", string getExcludedDelegate = "")
        {
            Id = id;
            Url = url;
            CallbackName = callbackName;
            GetExcludedDelegate = getExcludedDelegate;
        }
    }
}
