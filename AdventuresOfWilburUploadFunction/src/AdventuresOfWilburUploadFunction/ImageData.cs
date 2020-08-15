using System.Collections.Generic;
using System.IO;

namespace AdventuresOfWilbur
{
    public class ImageData
    {
        public string Title { get; set; }
        public string FileName { get; set; }
        public byte[] Image { get; set; }
        public string Description { get; set; }
        public List<string> Friends { get; set; }
        public long NumberOfItems { get; set; }
    }
}