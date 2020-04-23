namespace OneDriveSimple.Response
{
    public class LinkResponseInfo
    {
        public InnerLinkInfo Link
        {
            get;
            set;
        }

        public class InnerLinkInfo
        {
            public string Type
            {
                get;
                set;
            }

            public string WebUrl
            {
                get;
                set;
            }
        }
    }
}