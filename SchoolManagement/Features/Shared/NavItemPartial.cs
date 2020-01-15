namespace SchoolManagement.Features.Shared
{
    public class NavItemPartial
    {
        internal NavItemPartial()
        {

        }

        public string ControllerName { get; private set; }
        public string ActionName { get; private set; }
        public string Title { get; private set; }
        public string Icon { get; private set; }

        public static NavItemPartial Create(string title, string controller, string action, string icon)
        {
            return new NavItemPartial()
            {
                ActionName = action,
                ControllerName = controller,
                Title = title,
                Icon = icon
            };
        }

    }
}
