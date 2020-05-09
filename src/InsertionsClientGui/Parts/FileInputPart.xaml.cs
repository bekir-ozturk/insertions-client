// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Net.Insertions.Clients.Images;
using System;
using System.Windows.Controls;

namespace Microsoft.Net.Insertions.Clients.Parts
{
    public partial class FileInputPart : UserControl
    {
        public FileInputPart()
        {
            InitializeComponent();

            bttnFindFile.InitializeLookAndFeel(ImageOption.Open, "Find file", true);
            bttnFindDirectory.InitializeLookAndFeel(ImageOption.OpenFolder, "Find directory", true);

            bttnFindFile.Click += (sender, e) => App.OpenFile(OpenAction, Filter);
            bttnFindDirectory.Click += (sender, e) => App.OpenDirectory(OpenAction);
        }


        internal Action<string> OpenAction { get; set; }

        internal string Filter { get; set; }
    }
}
