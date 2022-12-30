using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using bridgefluence.Providers;
using bridgefluence.Tools;
using bridgefluence_api;
using bridgefluence.Services.Brand;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace bridgefluence.Controllers;

[ApiController]
[EnableCors("Allau")]
[Route("[controller]")]
public class BrandController : BaseController
{
    public BrandController(DataContext context, IDateTimeProvider dtProvider) : base(context, dtProvider)
    {
    }

    [HttpPost("create-brand")]
    public async Task<BrandDTO> Create(BrandResumeDTO dto) =>
        await new BrandService(this.db).Create(GetUserId(), dto);

    [HttpGet("get-brand")]
    public async Task<BrandDTO> Get() =>
        await new BrandService(this.db).Get(GetUserId());

    [HttpPost("feed")]
    public async Task<List<FeedItem>> GetFeed(FeedConfig config) =>
        await new BrandService(this.db).GetFeed(GetUserId(), config);

    [HttpPut("postpone-post")]
    public async Task PostponePost(int postId) =>
        await new BrandService(this.db).PostponePost(postId);

    [HttpPut("reject-post")]
    public async Task RejectPost(int postId, EnumsForBrands.PostRejectionReason[] reason) =>
        await new BrandService(this.db).RejectPost(postId, reason);

    [HttpPut("pay-for-post")]
    public async Task PayForPost(int postId, int price) =>
        await new BrandService(this.db).PayForPost(postId, price);

    [HttpGet("get-paid-posts")]
    public async Task<List<PaidPostDTO>> GetPaidPosts() =>
        await new BrandService(this.db).GetPaidPosts(GetUserId());

    [HttpGet("get-financial-summary")]
    public async Task<FinancialSummaryDTO> GetFinancialSummary(DateTime start, DateTime end) =>
        await new BrandService(this.db).GetFinancialSummary(GetUserId(), start, end);
}