namespace GOoDkat.PixelLogic.Miscellaneous
{
    using System.Collections.Generic;

    internal class SaveFileDialogOptions
    {
        public SaveFileDialogOptions()
        {
            Filters = new List<KeyValuePair<string, string>>();
        }

        public string FileName { get; set; }

        public string DefaultExtension { get; set; }

        public ICollection<KeyValuePair<string, string>> Filters { get; }
    }
}