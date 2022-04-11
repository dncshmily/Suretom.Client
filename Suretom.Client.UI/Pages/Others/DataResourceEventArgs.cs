using Suretom.Client.Entity;
using System;

namespace Suretom.Client.UI.Others
{
    public class RemoveCtlEventArgs : EventArgs
    {
        public string WordFileName { get; }

        public RemoveCtlEventArgs(string wordFileName)
        {
            this.WordFileName = wordFileName;
        }
    }

    public class StatusChangeEventArgs : EventArgs
    {
        public BatchImportStatus Status { get; }

        public StatusChangeEventArgs(BatchImportStatus status)
        {
            this.Status = status;
        }
    }
}