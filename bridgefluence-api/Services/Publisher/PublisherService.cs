using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using bridgefluence.Tools;
using bridgefluence_api;
using bridgefluence.Services.Common;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace bridgefluence.Services.Publisher
{
    public class PublisherService
    {
        private DataContext db { get; set; }

        public PublisherService(DataContext context)
        {
            db = context;
        }

        public async Task<bridgefluence_api.Publisher> Create(int publisherId, PublisherResumeDTO dto)
        {
            var existingPublisher = db.Publishers.FirstOrDefault(x => x.Id == publisherId);

            if (existingPublisher != null)
            {
                existingPublisher.Geo = dto.Geo;
                existingPublisher.SubscriberCount = dto.SubscriberCount;

                await db.SaveChangesAsync();

                return existingPublisher;
            }

            var publisher = new bridgefluence_api.Publisher()
            {
                Id = publisherId,
                Name = dto.Name,
                Geo = dto.Geo,
                SubscriberCount = dto.SubscriberCount,
            };

            var createdPublisher = await db.Publishers.AddAsync(publisher);

            await db.SaveChangesAsync();

            return createdPublisher.Entity;
        }

        public async Task<List<BrandSearchDTO>> FindBrand(string brandName)
        {
            var matchingBrands = await db.Brands
                .Where(x => EF.Functions.ILike(x.Title, $"%{brandName}%"))
                .Select(x => new BrandSearchDTO
                {
                    Title = x.Title,
                    BrandId = x.Id
                })
                .ToListAsync();

            return matchingBrands;
        }
        
        public async Task<List<int>> GetExistingPosts(int publisherId)  
        {
            var existingPostIds = await db.Posts
                .Where(x => x.PublisherId == publisherId)
                .Select(x =>  x.PostId)
                .ToListAsync();

            return existingPostIds;
        }
        
        public async Task<bridgefluence_api.Post> SubmitPost(int publisherId, int brandId, SubmitPostDTO submitPost)
        {
            var publisher = await db.Publishers.FindAsync(publisherId);

            if (publisher == null)
            {
                throw new Exception($"Publisher with Id '{publisherId}' is not found");
            }

            var brand = await db.Brands.FindAsync(brandId);

            if (brand == null)
            {
                throw new Exception($"Brand with Id '{brandId}' is not found");
            }

            var alreadySubmitted = await db.Posts
                .FirstOrDefaultAsync(x => x.PublisherId == publisherId
                                          && x.BrandId == brandId
                                          && x.PostId == submitPost.PostId);

            if (alreadySubmitted != null)
            {
                throw new Exception($"Post already submitted");
            }

            var submittedPost = await db.Posts.AddAsync(new Post()
            {
                PostId = submitPost.PostId,
                RequestedPrice = submitPost.RequestedPrice,
                PublisherId = publisherId,
                BrandId = brandId,
                SubmittedAt = DateTime.UtcNow
            });

            await db.SaveChangesAsync();

            return submittedPost.Entity;
        }
    }
}