namespace TurnBasedStrategy
{
    public struct TutorialSettings
    {
        public string Title { get; }
        public string Body { get; }

        public TutorialSettings(string title, string body)
        {
            Title = title;
            Body = body;
        }
    }
}