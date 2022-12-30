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

public class CommonServiceTests
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
    public async Task Init_Publisher(int publisherId, PublisherResumeDTO publisherResumeDto, 
        int brandId, BrandResumeDTO brandResumeDto)
    {
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
    
        var publisher = await this.PublisherService.Create(publisherId, publisherResumeDto);
        var brand = await this.BrandService.Create(brandId, brandResumeDto);

        var result = await CommonService.Init(publisher.Id);
        
        var expected = new WelcomeDTO()
        {
            isBrand = false,
            isPublisher = true
        };
        
        result.Should().BeEquivalentTo(expected);
    }
  
    [Test]
    [AutoData]
    public async Task Init_Brand(int publisherId, PublisherResumeDTO publisherResumeDto, 
        int brandId, BrandResumeDTO brandResumeDto)
    {
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
    
        var publisher = await this.PublisherService.Create(publisherId, publisherResumeDto);
        var brand = await this.BrandService.Create(brandId, brandResumeDto);

        var result = await CommonService.Init(brand.Id);
        
        var expected = new WelcomeDTO()
        {
            isBrand = true,
            isPublisher = false
        };
        
        result.Should().BeEquivalentTo(expected);
    }
  
    [Test]
    [AutoData]
    public async Task Init_Unknown(int id)
    {
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var result = await CommonService.Init(id);
        
        var expected = new WelcomeDTO()
        {
            isBrand = false,
            isPublisher = false
        };
        
        result.Should().BeEquivalentTo(expected);
    }
}