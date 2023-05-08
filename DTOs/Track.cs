using System;
using System.Collections.Generic;

public abstract class Track
{
    public string Title { get; set; }
    public string Artist { get; set; }
    public TimeSpan Duration { get; set; }
}