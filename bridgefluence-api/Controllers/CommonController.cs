using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using bridgefluence.Providers;
using bridgefluence.Tools;
using bridgefluence_api;
using bridgefluence.Services.Common; 
using Microsoft.AspNetCore.Mvc;

namespace bridgefluence.Controllers;
 
public class CommonController : BaseController
{  
    public CommonController(DataContext context, IDateTimeProvider dtProvider): base(context, dtProvider)
    { 
    }
   
    [HttpGet("init")]
    public async Task<WelcomeDTO> Init() =>
        await new CommonService(this.db).Init(this.GetUserId()); 
}