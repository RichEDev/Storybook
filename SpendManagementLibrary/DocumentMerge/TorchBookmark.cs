using Syncfusion.DocIO.DLS;

namespace SpendManagementLibrary.DocumentMerge
{
    public class TorchBookmark
    {
        public int SequenceNumber { get; private set; }
        public string BookmarkName { get; private set; }
        public TextBodyPart Content { get; private set; }
        public TorchBookmark(int sequenceNumber, string bookmarkName, TextBodyPart content)
        {
            this.SequenceNumber = sequenceNumber;
            this.BookmarkName = bookmarkName;
            this.Content = content;
        }
    }
}
