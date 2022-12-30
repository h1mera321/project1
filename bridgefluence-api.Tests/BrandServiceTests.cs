using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using AutoFixture.NUnit3;
using bridgefluence.Services.Brand;
using bridgefluence.Services.Common;
using bridgefluence.Services.Publisher;
using bridgefluence.Tools;
using FluentAssertions;
using NUnit.Framework;

namespace bridgefluence_api.Tests;

public class BrandServiceTests
{
    private Seed Seed { get; set; }
    private BrandService BrandService { get; set; }
    private PublisherService PublisherService { get; set; }
    private CommonService CommonService { get; set; }

    [SetUp]
    public void Setup()
    {
        this.Seed = new Seed();
        this.BrandService = new BrandService(Seed.GetTestContext());
        this.PublisherService = new PublisherService(Seed.GetTestContext());
        this.CommonService = new CommonService(Seed.GetTestContext());
    }

    [Test]
    [AutoData]
    public async Task GetFeed_ActivePosts(int publisherId, PublisherResumeDTO publisherResumeDto,
        int brandId, BrandResumeDTO brandResumeDto, SubmitPostDTO submitPostDto)
    {
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var publisher = await this.PublisherService.Create(publisherId, publisherResumeDto);
        var brand = await this.BrandService.Create(brandId, brandResumeDto);
        var post = await this.PublisherService.SubmitPost(publisher.Id, brand.Id, submitPostDto);

        var result = await BrandService.GetFeed(brand.Id, new FeedConfig()
        {
            Paid = false,
            Postponed = false,
            Rejected = false,
        });

        var expected = new FeedItem()
        {
            PostId = post.PostId,
            DesiredPrice = post.RequestedPrice,
            PublisherId = post.PublisherId,
            Geo = publisher.Geo,
            SubmittedAt = post.SubmittedAt
        };

        result.Single().Should().BeEquivalentTo(expected, opts => opts.Excluding(x => x.Id));
    }

    [Test]
    [AutoData]
    public async Task Create(int brandId, BrandResumeDTO brandResumeDto)
    {
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await BrandService.Create(brandId, brandResumeDto);

        var result = await this.BrandService.Get(brandId);

        var expected = new BrandDTO()
        {
            Id = brandId,
            Title = brandResumeDto.Title,
            Brief = brandResumeDto.Brief,
            Website = brandResumeDto.Website,
            Hashtags = brandResumeDto.Hashtags
        };

        result.Should().BeEquivalentTo(expected);
    }

    [Test]
    [AutoData]
    public async Task PostponePost(int publisherId, PublisherResumeDTO publisherResumeDto,
        int brandId, BrandResumeDTO brandResumeDto, SubmitPostDTO submitPostDto)
    {
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var publisher = await this.PublisherService.Create(publisherId, publisherResumeDto);
        var brand = await this.BrandService.Create(brandId, brandResumeDto);
        var post = await this.PublisherService.SubmitPost(publisher.Id, brand.Id, submitPostDto);

        await this.BrandService.PostponePost(post.Id);

        var feed = await this.BrandService.GetFeed(brand.Id, new FeedConfig()
        {
            Paid = false,
            Postponed = true,
            Rejected = false
        });

        var expected = new FeedItem()
        {
            PostId = post.PostId,
            DesiredPrice = post.RequestedPrice,
            PublisherId = post.PublisherId,
            Geo = publisher.Geo,
            SubmittedAt = post.SubmittedAt,
            PostponedAt = DateTime.UtcNow
        };

        feed.Single().Should().BeEquivalentTo(expected, opts => opts.Excluding(x => x.Id));
    }

    [Test]
    [AutoData]
    public async Task RejectPost(int publisherId, PublisherResumeDTO publisherResumeDto, int brandId,
        BrandResumeDTO brandResumeDto, SubmitPostDTO submitPostDto, EnumsForBrands.PostRejectionReason[] reasons)
    {
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var publisher = await this.PublisherService.Create(publisherId, publisherResumeDto);
        var brand = await this.BrandService.Create(brandId, brandResumeDto);
        var post = await this.PublisherService.SubmitPost(publisher.Id, brand.Id, submitPostDto);

        await BrandService.RejectPost(post.Id, reasons);

        var feed = await this.BrandService.GetFeed(brand.Id, new FeedConfig()
        {
            Paid = false,
            Postponed = false,
            Rejected = true
        });

        var expected = new FeedItem()
        {
            PostId = post.PostId,
            DesiredPrice = post.RequestedPrice,
            PublisherId = post.PublisherId,
            Geo = publisher.Geo,
            SubmittedAt = post.SubmittedAt,
            RejectionReasons = reasons,
            RejectedAt = DateTime.UtcNow
        };

        feed.Single().Should().BeEquivalentTo(expected, opts => opts.Excluding(x => x.Id));
    }

    [Test]
    [AutoData]
    public async Task PayForPost(int publisherId, PublisherResumeDTO publisherResumeDto,
        int brandId, BrandResumeDTO brandResumeDto, SubmitPostDTO submitPostDto, int price)
    {
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var publisher = await this.PublisherService.Create(publisherId, publisherResumeDto);
        var brand = await this.BrandService.Create(brandId, brandResumeDto);
        var post = await this.PublisherService.SubmitPost(publisher.Id, brand.Id, submitPostDto);

        await BrandService.PayForPost(post.Id, price);

        var feed = await this.BrandService.GetFeed(brand.Id, new FeedConfig()
        {
            Paid = true,
            Postponed = false,
            Rejected = false
        });

        var expected = new FeedItem()
        {
            PostId = post.PostId,
            DesiredPrice = post.RequestedPrice,
            PublisherId = post.PublisherId,
            Geo = publisher.Geo,
            SubmittedAt = post.SubmittedAt,
            PaidPrice = price,
            PaidAt = DateTime.UtcNow
        };

        feed.Single().Should().BeEquivalentTo(expected, opts => opts.Excluding(x => x.Id));
    }

    [Test]
    [AutoData]
    public async Task GetPaidPosts(int publisherId, PublisherResumeDTO publisherResumeDto,
        int brandId, BrandResumeDTO brandResumeDto, SubmitPostDTO submitPostDto1,
        SubmitPostDTO submitPostDto2, SubmitPostDTO submitPostDto3, int price1, int price3)
    {
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var publisher = await this.PublisherService.Create(publisherId, publisherResumeDto);
        var brand = await this.BrandService.Create(brandId, brandResumeDto);

        var post1 = await this.PublisherService.SubmitPost(publisher.Id, brand.Id, submitPostDto1);
        var post2 = await this.PublisherService.SubmitPost(publisher.Id, brand.Id, submitPostDto2);
        var post3 = await this.PublisherService.SubmitPost(publisher.Id, brand.Id, submitPostDto3);

        await BrandService.PayForPost(post1.Id, price1);
        await BrandService.PayForPost(post3.Id, price3);

        var result = await this.BrandService.GetPaidPosts(brand.Id);

        var expected = new List<PaidPostDTO>()
        {
            new PaidPostDTO
            {
                PostId = post1.PostId,
                PublisherId = post1.PublisherId,
                PaidPrice = price1,
                PaidAt = DateTime.UtcNow
            },
            new PaidPostDTO
            {
                PostId = post3.PostId,
                PublisherId = post3.PublisherId,
                PaidPrice = price3,
                PaidAt = DateTime.UtcNow
            }
        };
        result.Should().BeEquivalentTo(expected, opts => opts.Excluding(x => x.Id));
    }

    [Test]
    [AutoData]
    public async Task GetFinancialSummary(int publisherId, PublisherResumeDTO publisherResumeDto, int brandId, 
        BrandResumeDTO brandResumeDto, SubmitPostDTO submitPostDto1, SubmitPostDTO submitPostDto2, 
        SubmitPostDTO submitPostDto3, SubmitPostDTO submitPostDto4, int price1, int price2, int price3)
    {
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var publisher = await this.PublisherService.Create(publisherId, publisherResumeDto);
        var brand = await this.BrandService.Create(brandId, brandResumeDto);

        var postSubmittedBeforePaidIn = await this.PublisherService.SubmitPost(publisher.Id, brand.Id, submitPostDto1);

        var start = DateTime.UtcNow;

        await BrandService.PayForPost(postSubmittedBeforePaidIn.Id, price1);
        var postSubmittedInPaidIn = await this.PublisherService.SubmitPost(publisher.Id, brand.Id, submitPostDto2);
        await BrandService.PayForPost(postSubmittedInPaidIn.Id, price2);
        var postSubmittedInPaidAfter = await this.PublisherService.SubmitPost(publisher.Id, brand.Id, submitPostDto3);
        var postSubmittedInNotPaid = await this.PublisherService.SubmitPost(publisher.Id, brand.Id, submitPostDto4);

        var end = DateTime.UtcNow;

        await BrandService.PayForPost(postSubmittedInPaidAfter.Id, price3);

        var result = await this.BrandService.GetFinancialSummary(brand.Id, start, end);

        result.SubmittedPosts.Should().Be(3);
        result.PaidPosts.Should().Be(2);
        result.PaidRatio.Should().BeInRange(66,67);
        result.TotalPaid.Should().Be(price1+price2);
    }
}