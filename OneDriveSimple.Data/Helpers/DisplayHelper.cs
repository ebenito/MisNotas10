using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using OneDriveSimple.Response;

namespace OneDriveSimple.Helpers
{
    public static class DisplayHelper
    {
        public static void ShowContent(
            string title, 
            ItemInfoResponse item, 
            IList<ItemInfoResponse> children,
            Action<string> showDialog)
        {
            Debug.WriteLine(title);

            Debug.WriteLine($"Folder name: {item.Name}");
            Debug.WriteLine($"Created on: {item.CreatedDateTime}");
            Debug.WriteLine($"Modified on: {item.LastModifiedDateTime}");
            Debug.WriteLine($"Size: {item.SizeInBytes.ConvertSize()}");
            Debug.WriteLine($"Web URL: {item.WebUrl}");

            if (item.Folder != null)
            {
                Debug.WriteLine($"Child count: {item.Folder.ChildCount}");

                showDialog(
                    $"The folder {item.Name} has {item.Folder.ChildCount} children. More details in the Output window!");
            }
            else
            {
                showDialog($"Retrieved info for folder {item.Name}. More details in the Output window!");
            }

            if (children == null)
            {
                return;
            }

            foreach (var child in children)
            {
                Debug.WriteLine("----");
                Debug.WriteLine($"Child name: {child.Name}");
                Debug.WriteLine($"Created on: {child.CreatedDateTime}");
                Debug.WriteLine($"Modified on: {child.LastModifiedDateTime}");
                Debug.WriteLine($"Size: {child.SizeInBytes.ConvertSize()}");
                Debug.WriteLine($"Web URL: {child.WebUrl}");
                Debug.WriteLine($"Kind: {child.Kind}");

                if (child.Audio != null)
                {
                    Debug.WriteLine("AUDIO INFO");
                    Debug.WriteLine($"Album: {child.Audio.Album}");
                    Debug.WriteLine($"Album Artist: {child.Audio.AlbumArtist}");
                    Debug.WriteLine($"Duration [ms]: {child.Audio.DurationMilliSeconds}");
                }

                if (child.Video != null)
                {
                    Debug.WriteLine("VIDEO INFO");
                    Debug.WriteLine($"Bitrate: {child.Video.Bitrate}");
                    Debug.WriteLine($"Width: {child.Video.Width}");
                    Debug.WriteLine($"Height: {child.Video.Height}");
                }

                if (child.Photo != null)
                {
                    Debug.WriteLine("PHOTO INFO");
                    Debug.WriteLine($"Camera Model: {child.Photo.CameraModel}");
                    Debug.WriteLine($"Camera Make: {child.Photo.CameraMake}");
                }

                if (child.Image != null)
                {
                    Debug.WriteLine("IMAGE INFO");
                    Debug.WriteLine($"Width: {child.Image.Width}");
                    Debug.WriteLine($"Height: {child.Image.Height}");
                }

                if (child.Folder != null)
                {
                    Debug.WriteLine("FOLDER INFO");
                    Debug.WriteLine($"Child count: {child.Folder.ChildCount}");
                    // At this point we would need to populate the subfolder
                    // with PopulateChildren
                }
            }
        }
    }
}
