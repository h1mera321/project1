using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using AutoFixture.NUnit3;
using bridgefluence.Services;
using bridgefluence.Services.Brand;
using bridgefluence.Services.Common;
using bridgefluence.Services.Publisher;
using bridgefluence.Tools;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace bridgefluence_api.Tests;

public class PublisherServiceTests
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
    public async Task FindBrand(int brandId1, BrandResumeDTO brandResumeDto1, int brandId2, BrandResumeDTO brandResumeDto2)
    {
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        brandResumeDto1.Title = "SamduRak";
        brandResumeDto2.Title = "Durak";

        var brand1 = await this.BrandService.Create(brandId1, brandResumeDto1);

        var brand2 = await this.BrandService.Create(brandId2, brandResumeDto2);

        var result = await this.PublisherService.FindBrand("durak");

        var expected = new List<BrandSearchDTO>()
        {
            new BrandSearchDTO()
            {
                BrandId = brand1.Id,
                Title = brand1.Title
            },
            new BrandSearchDTO()
            {
                BrandId = brand2.Id,
                Title = brand2.Title
            }
        };

        result.Should().BeEquivalentTo(expected);
    }
    
    [Test]
    [AutoData]
     public async Task Create_PublisherExists(int publisherId, PublisherResumeDTO publisherResumeDto, 
         PublisherResumeDTO newPublisherResumeDto)
     {
         using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
    
         var publisher = await this.PublisherService.Create(publisherId, publisherResumeDto);

         var result = await this.PublisherService.Create(publisher.Id, newPublisherResumeDto);

         var expected = new Publisher()
         {
             Name = publisher.Name,
             Geo = newPublisherResumeDto.Geo,
             SubscriberCount = newPublisherResumeDto.SubscriberCount
         };
         
         result.Should().BeEquivalentTo(expected, opts => opts.Excluding(x => x.Id));
     } 
   
    [Test]
    [AutoData]
     public async Task Create_PublisherNotExists(int publisherId, PublisherResumeDTO publisherResumeDto)
     {
         using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

         var publisher = await this.PublisherService.Create(publisherId, publisherResumeDto);

         var expected = new Publisher()
         {
             Name = publisher.Name,
             Geo = publisher.Geo,
             SubscriberCount = publisher.SubscriberCount
         };
         
         publisher.Should().BeEquivalentTo(expected, opts => opts.Excluding(x => x.Id));
     }
     
    [Test]
    [AutoData]
     public async Task GetExistingPosts(int publisherId, PublisherResumeDTO publisherResumeDto, 
         int brandId, BrandResumeDTO brandResumeDto, SubmitPostDTO submitPostDto)
     {
         using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
         
         var publisher = await this.PublisherService.Create(publisherId, publisherResumeDto);
         var brand = await this.BrandService.Create(brandId, brandResumeDto);
         var post = await this.PublisherService.SubmitPost(publisher.Id, brand.Id, submitPostDto);

         var result = await PublisherService.GetExistingPosts(publisher.Id);

         var expected = new List<int>(new[] { post.PostId });
         
         result.Should().BeEquivalentTo(expected);
     } 
   
     [Test]
     [AutoData]
     public async Task SubmitPost(int publisherId, PublisherResumeDTO publisherResumeDto, 
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
             Rejected = false
         });
         
         result.Count.Should().Be(1);
     } 
}