using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using bridgefluence.Providers;
using bridgefluence.Services.Publisher;
using bridgefluence.Tools;
using bridgefluence_api;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace bridgefluence.Controllers;

[ApiController]
[EnableCors("Allau")]
[Route("[controller]")]
public class PublisherController : BaseController
{  
    public PublisherController(DataContext context, IDateTimeProvider dtProvider): base(context, dtProvider)
    { 
    }
    
    [HttpPost("create-publisher")]
    public async Task Create(PublisherResumeDTO dto) =>
        await new PublisherService(this.db).Create(GetUserId(), dto);
    
    [HttpGet("discovery")]
    public async Task<List<BrandSearchDTO>> FindBrand(string brandName) =>
        await new PublisherService(this.db).FindBrand( brandName);
    
    [HttpGet("existing-posts")]
    public async Task<List<int>> GetExistingPosts( ) =>
        await new PublisherService(this.db).GetExistingPosts( GetUserId());
    
    [HttpPost("submit-post")]
    public async Task SubmitPost(int brandId, SubmitPostDTO submitPost) =>
        await new PublisherService(this.db).SubmitPost(GetUserId(), brandId, submitPost);
     
}