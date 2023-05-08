using System;
using System.Collections.Generic;

public abstract class MediaItem : LibraryItem
{
    public List<Track> Tracks { get; set; } = new List<Track>();
}