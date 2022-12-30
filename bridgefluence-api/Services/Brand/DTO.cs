using System;
using System.Collections.Generic;
using bridgefluence.Tools;
using bridgefluence_api;
using bridgefluence.Services.Publisher;

namespace bridgefluence.Services.Brand;

public class BrandResumeDTO
{  
    public string Title { get; set; }
    public string Brief { get; set; } 
    public string Website { get; set; } 
    public string[] Hashtags { get; set; } 
}

public class FeedItem
{  
    public int Id { get; set; }
    public int PostId { get; set; }
    public int PublisherId { get; set; } 
    public List<GeoItem> Geo { get; set; }
    public int DesiredPrice { get; set; } 
    public int? PaidPrice { get; set; }
    public EnumsForBrands.PostRejectionReason[] RejectionReasons { get; set; }
    public DateTime SubmittedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime? RejectedAt { get; set; }
    public DateTime? PostponedAt { get; set; }
}

public class FeedConfig
{   
    public bool Postponed { get; set; } 
    public bool Paid { get; set; } 
    public bool Rejected { get; set; } 
}

public class BrandDTO
{  
    public int Id { get; set; } 
    public string Title { get; set; }
    public string Brief { get; set; } 
    public string Website { get; set; } 
    public string[] Hashtags { get; set; }
}

public class PaidPostDTO
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public int PublisherId { get; set; }
    public int? PaidPrice { get; set; }
    public DateTime? PaidAt { get; set; }
}

public class FinancialSummaryDTO
{
    public int SubmittedPosts { get; set; }
    public int PaidPosts { get; set; }
    public int? TotalPaid { get; set; }
    public decimal PaidRatio { get; set; }
}