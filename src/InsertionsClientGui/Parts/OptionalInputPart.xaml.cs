// Copyright (c) Microsoft. All rights reserved.

using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Net.Insertions.Clients.Parts
{
    public partial class OptionalInputPart : UserControl
    {
        public OptionalInputPart()
        {
            InitializeComponent();
        }

        private void HandlerToogleEnabledState(object sender, RoutedEventArgs e)
        {
            partInput.IsEnabled = chbx.IsChecked == true;
        }
    }
}
