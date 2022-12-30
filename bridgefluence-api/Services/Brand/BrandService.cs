using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using bridgefluence.Tools;
using bridgefluence_api;
using bridgefluence_api.Tools;
using Microsoft.EntityFrameworkCore; 

namespace bridgefluence.Services.Brand;

public class BrandService
{
    private DataContext db { get; set; }

    public BrandService(DataContext context)
    {
        db = context;
    }


    public async Task<BrandDTO> Get(int brandId)
    {
        var brand = await db.Brands.FindAsync(brandId);

        return brand.Map<BrandDTO>();
    }

    public async Task<BrandDTO> Create(int brandId, BrandResumeDTO dto)
    {
        var brand = new bridgefluence_api.Brand()
        {
            Id = brandId,
            Title = dto.Title,
            Brief = dto.Brief,
            Website = dto.Website,
            Hashtags = dto.Hashtags
        };

        await db.Brands.AddAsync(brand);

        await db.SaveChangesAsync();

        return brand.Map<BrandDTO>();
    }

    public async Task<List<FeedItem>> GetFeed(int brandId, FeedConfig config)
    {
        var posts = await db.Posts
            .Where(x => x.BrandId == brandId)
            .Where(x => x.Paid == config.Paid
                        && x.Postponed == config.Postponed
                        && x.Rejected == config.Rejected)
            .Select(x => new FeedItem
            {
                Id = x.Id,
                PostId = x.PostId,
                DesiredPrice = x.RequestedPrice,
                PublisherId = x.PublisherId,
                RejectionReasons = x.RejectionReasons,
                PaidPrice = x.PaidPrice,
                SubmittedAt = x.SubmittedAt,
                PaidAt = x.PaidAt,
                PostponedAt = x.PostponedAt,
                RejectedAt = x.RejectedAt
            })
            .ToListAsync();

        var publisherIds = posts.Select(x => x.PublisherId).ToList();

        var geos = await db.Publishers
            .Where(x => publisherIds.Contains(x.Id))
            .Select(x => new
            {
                x.Geo,
                PublisherId = x.Id
            })
            .ToListAsync();

        posts.ForEach(x => x.Geo = geos.Single(g => g.PublisherId == x.PublisherId).Geo);

        return posts;
    }

    public async Task PostponePost(int id)
    {
        var targetPost = await db.Posts.FirstOrDefaultAsync(x => x.Id == id);

        if (targetPost == null)
        {
            throw new Exception("Id not found");
        }

        targetPost.Postponed = true;
        targetPost.PostponedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
    }

    public async Task RejectPost(int id, EnumsForBrands.PostRejectionReason[] reasons)
    {
        var targetPost = await db.Posts.FirstOrDefaultAsync(x => x.Id == id);

        if (targetPost == null)
        {
            throw new Exception("Id not found");
        }

        targetPost.Rejected = true;
        targetPost.RejectionReasons = reasons;
        targetPost.RejectedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
    }

    public async Task PayForPost(int id, int price)
    {
        var targetPost = await db.Posts.FirstOrDefaultAsync(x => x.Id == id);

        if (targetPost == null)
        {
            throw new Exception("Id not found");
        }

        targetPost.Paid = true;
        targetPost.PaidPrice = price;
        targetPost.PaidAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
    }

    public async Task<List<PaidPostDTO>> GetPaidPosts(int brandId)
    {
        var paidPosts = await db.Posts
            .Where(x => x.BrandId == brandId)
            .Where(x => x.Paid)
            .Select(x => new PaidPostDTO
            {
                Id = x.Id,
                PostId = x.PostId,
                PublisherId = x.PublisherId,
                PaidPrice = x.PaidPrice,
                PaidAt = x.PaidAt
            })
            .OrderByDescending(x => x.PaidAt)
            .ToListAsync();

        return paidPosts;
    }

    public async Task<FinancialSummaryDTO> GetFinancialSummary(int brandId, DateTime start, DateTime end)
    {
        var posts = await db.Posts
            .Where(x => x.BrandId == brandId)
            .Where(x => (x.PaidAt >= start && x.PaidAt <= end) || (x.SubmittedAt >= start && x.SubmittedAt <= end))
            .ToListAsync();

        var submittedPosts = posts.Count(x => x.SubmittedAt >= start && x.SubmittedAt <= end);

        var paidPosts = posts.Count(x => x.PaidAt >= start && x.PaidAt <= end);

        return new FinancialSummaryDTO
        {
            SubmittedPosts = submittedPosts,
            PaidPosts = paidPosts,
            PaidRatio = posts.Any() ? ((decimal)paidPosts / (decimal)submittedPosts) * 100 : 0,
            TotalPaid = posts.Where(x => x.PaidAt >= start && x.PaidAt <= end).Sum(x => x.PaidPrice)
        };
    }
















    // public async Task UpdateBrand(int brandId, string brandName, string brandLogoUrl)
    // { 
    //     var publisher = await db.Publishers.FindAsync(brandId);
    //     publisher.BrandName = brandName;
    //     publisher.BrandLogo = brandLogoUrl;
    //
    //     db.Update(publisher);
    //
    //     await db.SaveChangesAsync();
    // }
    //
    // public async Task<List<CampaignChatMessageDTO>> GetCampaignChat(int campaignId, int brandId, int publisherId)
    // {
    //     var messages = await db.ChatMessages
    //         .Where(x => x.BrandId == brandId)
    //         .Where(x => x.PublisherId == publisherId)
    //         .Where(x => x.CampaignId == campaignId)
    //         .Select(x => new CampaignChatMessageDTO()
    //         {
    //             Timestamp = x.Timestamp,
    //             BrandMessage = x.BrandMessage,
    //             PublisherMessage = x.PublisherMessage
    //         })
    //         .OrderByDescending(x => x.Timestamp)
    //         .Take(500)
    //         .ToListAsync();
    //
    //     return messages;
    // }
    //
    // public async Task SendCampaignChatMessage(int campaignId, int brandId,
    //     int publisherId, string message)
    // {
    //     await db.ChatMessages
    //         .AddAsync(new ChatMessage()
    //         {
    //             BrandMessage = message,
    //             Timestamp = DateTime.UtcNow,
    //             PublisherId = publisherId,
    //             CampaignId = campaignId,
    //             BrandId = brandId
    //         });
    //
    //     await db.SaveChangesAsync();
    // }
    //
    // public async Task<List<BrandCampaignDTO>> GetCampaigns(int brandId)
    // {
    //     var campaigns = await db.Campaigns
    //         .Where(x => x.BrandId == brandId)
    //         .Select(x => new
    //         {
    //             Id = x.Id,
    //             BrandId = x.BrandId,
    //             Name = x.Name,
    //             Starts = x.Starts,
    //             Ends = x.Ends,
    //             Status = x.CampaignStatus,
    //             CreatedAt = x.CreatedAt,
    //             InternalDescription = x.InternalDescription,
    //             MemberCount = x.CampaignMemberships.Count
    //         })
    //         .OrderBy(x => x.Starts)
    //         .ToListAsync();
    //
    //     return campaigns.Select(x => x.Adapt<BrandCampaignDTO>()).ToList();
    // }
    //
    // public async Task<CampaignDetailsDTO> GetCampaign(int brandId, int campaignId)
    // {
    //     var campaign = await db.Campaigns
    //         .AsNoTracking()
    //         .Where(x => x.BrandId == brandId)
    //         .Where(x => x.Id == campaignId)
    //         .Select(x => new
    //         {
    //             Id = x.Id,
    //             Name = x.Name,
    //             Starts = x.Starts,
    //             Ends = x.Ends,
    //             Status = x.CampaignStatus,
    //             CreatedAt = x.CreatedAt,
    //             InternalDescription = x.InternalDescription,
    //             Members = x.CampaignMemberships
    //                 .Select(cm => new CampaignDetailsMemberDTO()
    //                 {
    //                     PublisherId = cm.PublisherId,
    //                     AvatarUrl = cm.Publisher.AvatarUrl,
    //                     DisplayName = cm.Publisher.DisplayName,
    //                     CampaignMembershipStatus = cm.Status,
    //                     InvitedAt = cm.InvitedAt,
    //                     ResponseConsumedAt = cm.ResponseConsumedAt,
    //                     InviteRejectionReason = cm.InviteRejectionReason,
    //                     ResponseRejectionReason = cm.ResponseRejectionReason,
    //                     InviteResponseAt = cm.InviteResponseAt
    //                 })
    //                 .ToList()
    //         })
    //         .FirstOrDefaultAsync();
    //
    //     var dbAssignments = await db.CampaignAssignments
    //         .AsNoTracking()
    //         .Where(x => x.CampaignId == campaignId)
    //         .ToListAsync();
    //
    //     var firstDay = campaign.Starts;
    //     var lastDay = campaign.Ends;
    //
    //     var assignments = new List<CampaignDailyAssignmentsDTO>();
    //
    //     Console.WriteLine("total assignments: " + dbAssignments.Count);
    //
    //     assignments = new List<CampaignDailyAssignmentsDTO>();
    //
    //     while (firstDay <= lastDay)
    //     {
    //         assignments.Add(new CampaignDailyAssignmentsDTO
    //         {
    //             DisplayDate = firstDay.ToString("d MMMM, yyyy"),
    //             Date = firstDay,
    //             Assignments = dbAssignments
    //                 .Where(x => x.PublishDate.ToString("yyyy MMMM dd") == firstDay.ToString("yyyy MMMM dd"))
    //                 .Select(x => x.Adapt<CampaignAssignmentDTO>())
    //                 .ToList()
    //         });
    //
    //         firstDay = firstDay.AddDays(1);
    //     }
    //
    //     assignments.ForEach(x =>
    //     {
    //         x.Assignments.ForEach(ass =>
    //         {
    //             ass.CampaignAssignmentStatus = EnumsForCampaign.CampaignAssignmentStatus.None;
    //
    //             if (ass.RejectionReason != null)
    //             {
    //                 ass.CampaignAssignmentStatus = EnumsForCampaign.CampaignAssignmentStatus.Rejected;
    //                 return;
    //             }
    //
    //             if (ass.ValidationDecisionAt != null)
    //             {
    //                 ass.CampaignAssignmentStatus = EnumsForCampaign.CampaignAssignmentStatus.Approved;
    //                 return;
    //             }
    //
    //             if (ass.ResultSubmittedAt != null && ass.ResultSubmittedAt > ass.PublishDate)
    //             {
    //                 ass.CampaignAssignmentStatus = EnumsForCampaign.CampaignAssignmentStatus.Late;
    //                 return;
    //             }
    //
    //             if (ass.ResultSubmittedAt != null && ass.ResultSubmittedAt < ass.PublishDate)
    //             {
    //                 ass.CampaignAssignmentStatus = EnumsForCampaign.CampaignAssignmentStatus.Submitted;
    //                 return;
    //             }
    //
    //             if (ass.ResultSubmittedAt != null && ass.ResultSubmittedAt < ass.PublishDate)
    //             {
    //                 ass.CampaignAssignmentStatus = EnumsForCampaign.CampaignAssignmentStatus.Premature;
    //                 return;
    //             }
    //
    //             var memberInfo = campaign.Members.Single(y => y.PublisherId == ass.PublisherId);
    //             ass.CampaignAssignmentStatus = memberInfo.CampaignMembershipStatus switch
    //             {
    //                 EnumsForCampaign.CampaignMembershipStatus.Added => EnumsForCampaign.CampaignAssignmentStatus
    //                     .Preliminary,
    //                 EnumsForCampaign.CampaignMembershipStatus.Participating => EnumsForCampaign.CampaignAssignmentStatus
    //                     .Pending,
    //                 _ => ass.CampaignAssignmentStatus
    //             };
    //         });
    //     });
    //
    //     return new CampaignDetailsDTO()
    //     {
    //         Id = campaign.Id,
    //         Name = campaign.Name,
    //         Starts = campaign.Starts,
    //         Ends = campaign.Ends,
    //         CampaignStatus = campaign.Status,
    //         CreatedAt = campaign.CreatedAt,
    //         InternalDescription = campaign.InternalDescription,
    //         Assignments = assignments,
    //         Members = campaign.Members.Select(x =>
    //             {
    //                 var memberAssignments = dbAssignments
    //                     .Where(da => da.PublisherId == x.PublisherId)
    //                     .Select(da => da.Adapt<CampaignAssignmentPublisherResponseDTO>())
    //                     .ToList();
    //
    //                 x.Assignments = memberAssignments;
    //
    //                 return x;
    //             })
    //             .ToList()
    //     };
    // }
    //
    // public async Task<List<SearchResultPublisherDTO>> FindPublishers
    //     (int campaignId, DiscoveryFilterDTO discoveryFilterDTO)
    // {
    //     var campaignParticipantIds = await db.CampaignMemberships
    //         .Where(x => x.CampaignId == campaignId)
    //         .Select(x => x.PublisherId)
    //         .ToListAsync();
    //
    //     var result = await db.Publishers
    //         .AsNoTracking()
    //         .Include(x => x.ResumePosts)
    //         .Include(x => x.PublisherServiceRates)
    //         .Include(x => x.CampaignAssignments.Where(cm => cm.PublishDate > DateTime.UtcNow))
    //         .Include(x => x.CampaignMemberships.Where(cm => cm.InviteResponseAt != null
    //                                                         && cm.InviteRejectionReason == null
    //                                                         && cm.ResponseRejectionReason == null))
    //         .Where(x => x.Subscribers >= discoveryFilterDTO.MinSubscribers
    //                     // Do not use suggested hint, it breaks shit
    //                     && x.Categories.Any(c => discoveryFilterDTO.Categories.Contains(c)))
    //         .Where(x => x.AcceptsDelivery == discoveryFilterDTO.AcceptsDelivery)
    //         .Where(x => !campaignParticipantIds.Contains(x.Id))
    //         .Where(x => x.PublisherServiceRates.Any())
    //         .OrderByDescending(x => x.Subscribers)
    //         .ToListAsync();
    //
    //     var campaign = await db.Campaigns.SingleAsync(x => x.Id == campaignId);
    //
    //     return result.Select(x =>
    //         {
    //             var availabilityMatrix = new List<SearchResultPublisherDailyAvailabilityDTO>();
    //             var iterationDate = campaign.Starts;
    //
    //             var engagedCampaignIds = x.CampaignMemberships.Select(m => m.CampaignId);
    //
    //             while (iterationDate <= campaign.Ends)
    //             {
    //                 availabilityMatrix.Add(new SearchResultPublisherDailyAvailabilityDTO()
    //                 {
    //                     Date = iterationDate.Date,
    //                     Available = !x.CampaignAssignments
    //                         .Any(ca => ca.PublishDate.Date == iterationDate.Date)
    //                 });
    //
    //                 iterationDate = iterationDate.AddDays(1);
    //             }
    //
    //             return new SearchResultPublisherDTO()
    //             {
    //                 DisplayName = x.DisplayName,
    //                 Categories = x.Categories,
    //                 Id = x.Id,
    //                 Subscribers = x.Subscribers,
    //                 YearOfBirth = x.YearOfBirth,
    //                 AvatarUrl = x.AvatarUrl,
    //                 DailyAvailability = availabilityMatrix,
    //                 AcceptsDelivery = x.AcceptsDelivery,
    //                 PhotoPrice = x.PublisherServiceRates
    //                     .FirstOrDefault(p => p.ServiceType == EnumsForPublishers.PublisherServiceType.Photo)
    //                     ?.Price,
    //                 VideoPrice = x.PublisherServiceRates
    //                     .FirstOrDefault(p => p.ServiceType == EnumsForPublishers.PublisherServiceType.Video)
    //                     ?.Price,
    //                 ResumePosts = x.ResumePosts.Select(rp => new SearchResultPublisherResumeItemDTO()
    //                     {
    //                         Id = rp.Id,
    //                         Comments = rp.Comments,
    //                         PostId = rp.PostId,
    //                         PublisherId = rp.PublisherId,
    //                         PostUrl = rp.PostUrl,
    //                         Attachments = rp.Attachments,
    //                         AuthorContent = rp.AuthorContent,
    //                         Forced = rp.Forced,
    //                         Likes = rp.Likes,
    //                         Shares = rp.Shares,
    //                         Views = rp.Views,
    //                         PostText = rp.PostText
    //                     })
    //                     .ToList()
    //             };
    //         })
    //         .ToList();
    // }
}