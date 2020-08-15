using System.Collections.Generic;

namespace AdventuresOfWilbur
{
    public class ImageData
    {
        public string Title { get; }
        public string ImageUrl { get; }
        public string Description { get; }
        public List<string> Friends { get; }
        public long NumberOfItems { get; }
        
        public ImageData(string imageUrl, string title, string description, List<string> friends, long numberOfItems)
        {
            ImageUrl = imageUrl;
            Title = title;
            Description = description;
            Friends = friends;
            NumberOfItems = numberOfItems;
        }
    }
}