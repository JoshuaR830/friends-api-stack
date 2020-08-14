﻿using System;
using System.Collections.Generic;
using System.Net.Mime;

namespace AdventuresOfWilbur
{
    public class ImageData
    {
        public string Title { get; }
        public string ImageUrl { get; }
        public string Description { get; }
        public List<string> Friends { get; }
        
        public ImageData(string imageUrl, string title, string description, List<string> friends)
        {
            ImageUrl = imageUrl;
            Title = title;
            Description = description;
            Friends = friends;
        }
    }
}