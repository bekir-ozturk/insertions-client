// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Net.Insertions.Clients.Images;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Net.Insertions.Clients.Parts
{
    public partial class CommonButtonPart : UserControl
    {
        public CommonButtonPart()
        {
            InitializeComponent();
        }

        internal void InitializeLookAndFeel(ImageOption resource, string toolTipMsg, bool smallerButton = false)
        {
            txbkTooltip.Text = string.IsNullOrEmpty(toolTipMsg) ? string.Empty : toolTipMsg;
            rectButtonImage.Fill = ImageApi.CreateImageBrush(resource);

            if (smallerButton)
            {
                rectButtonImage.Height = 15;
                rectButtonImage.Width = 15;
            }
        }

        internal event RoutedEventHandler Click
        {
            add => button.Click += value;
            remove => button.Click -= value;
        }


    }
}
