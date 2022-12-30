using System.Threading.Tasks; 
using bridgefluence_api; 
using Microsoft.EntityFrameworkCore;

namespace bridgefluence.Services.Common
{
    public class CommonService
    {
        private DataContext db { get; }

        public CommonService(DataContext context)
        {
            db = context;
        } 
        
        public async Task<WelcomeDTO> Init(int userId)
        {
            var dto = new WelcomeDTO();

            var existingPublisher = await db.Publishers.FirstOrDefaultAsync(x => x.Id == userId);

            if (existingPublisher != null)
            {
                dto.isPublisher = true;
            }
            else
            {
                var existingBrand = await db.Brands.FirstOrDefaultAsync(x => x.Id == userId);
                
                if (existingBrand != null)
                {
                    dto.isBrand = true;
                }
            }

            return dto;
        }
    }
}