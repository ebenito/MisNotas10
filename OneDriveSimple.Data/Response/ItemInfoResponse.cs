using Newtonsoft.Json;

namespace OneDriveSimple.Response
{
    public class ItemInfoResponse : ItemInfo
    {
        private FileResponseInfo _file;
        private FolderResponseInfo _folder;

        public AudioResponseInfo Audio
        {
            get;
            set;
        }

        [JsonProperty("@content.downloadUrl")]
        public string DownloadUrl
        {
            get;
            set;
        }

        public FileResponseInfo File

        {
            get
            {
                return _file;
            }
            set
            {
                _file = value;

                if (value != null)
                {
                    Kind = FileFolderKind.File;
                }
            }
        }

        public FolderResponseInfo Folder
        {
            get
            {
                return _folder;
            }
            set
            {
                _folder = value;

                if (value != null)
                {
                    Kind = FileFolderKind.Folder;
                }
            }
        }

        public ImageResponseInfo Image
        {
            get;
            set;
        }

        public ParentReferenceInfo ParentReference
        {
            get;
            set;
        }

        public PhotoResponseInfo Photo
        {
            get;
            set;
        }

        public VideoResponseInfo Video
        {
            get;
            set;
        }

        public class ParentReferenceInfo
        {
            public string DriveId
            {
                get;
                set;
            }

            public string Id
            {
                get;
                set;
            }
        }
    }
}