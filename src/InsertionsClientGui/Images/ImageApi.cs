// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Microsoft.Net.Insertions.Clients.Images
{
    internal static class ImageApi
    {
        private static readonly IDictionary<ImageOption, Uri> ImagesUriMap = new Dictionary<ImageOption, Uri>();

        private static readonly string FolderImages = "Images";


        static ImageApi()
        {
            ImagesUriMap.Add(ImageOption.Start, GetMediaUri("Run_16x.png", FolderImages));
            ImagesUriMap.Add(ImageOption.Open, GetMediaUri("OpenQueryView_16x.png", FolderImages));
            ImagesUriMap.Add(ImageOption.Save, GetMediaUri("Save_16x.png", FolderImages));
            ImagesUriMap.Add(ImageOption.Help, GetMediaUri("StatusHelp_16x.png", FolderImages));
            ImagesUriMap.Add(ImageOption.LoadInputFile, GetMediaUri("OpenTopic_16x.png", FolderImages));
            ImagesUriMap.Add(ImageOption.OpenFolder, GetMediaUri("OpenFolder_16x.png", FolderImages));
        }


        internal static ImageBrush CreateImageBrush(ImageOption option)
        {
            return option == ImageOption.Default ? null : new ImageBrush(new BitmapImage(GetMediaUri(option)));
        }


        private static Uri GetMediaUri(ImageOption option)
        {
            return ImagesUriMap[option];
        }

        private static Uri GetMediaUri(string file, string mediaFolder)
        {
            return new Uri($"pack://application:,,,/Resources/{mediaFolder}/{file}");
        }
    }
}