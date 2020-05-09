// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Microsoft.Net.Insertions.Clients.Logging
{
    internal sealed class TextBoxTextWriterTraceListener : TextWriterTraceListener
    {
        private readonly TextBox _tbx;


        internal TextBoxTextWriterTraceListener(string fileName, string listenerName, TextBox tbx)
            : base(fileName, listenerName)
        {
            _tbx = tbx;
        }

        public override void Write(string message)
        {
            WriteLine(message);
        }

        public override void WriteLine(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                message = $"{DateTime.UtcNow.ToString("dd-M-yyyy hh:mm:ss.ffffff")} {message}";
                _ = Task.Factory.StartNew(() =>
                {
                    _tbx.Text = $"{message}{Environment.NewLine}{_tbx.Text}";
                }, CancellationToken.None, TaskCreationOptions.None, MainWindow.AppScheduler);
                base.WriteLine(message);
                Trace.Flush();
            }
        }
    }
}