using System;
using System.Collections.Generic;
using bridgefluence.Tools;
using bridgefluence_api;

namespace bridgefluence.Services.Publisher;
 
 
public class PublisherResumeDTO
{  
    public string Name { get; set; }
    public int SubscriberCount { get; set; } 
    public List<GeoItem> Geo { get; set; }
}
 
public class GeoItem
{ 
    public string Name { get; set; } 
    public int UserCount { get; set; }
}

public class SubmitPostDTO
{  
    public int PostId { get; set; }
    public int RequestedPrice { get; set; } 
}

public class BrandSearchDTO
{  
    public int BrandId { get; set; }
    public string Title { get; set; }
}