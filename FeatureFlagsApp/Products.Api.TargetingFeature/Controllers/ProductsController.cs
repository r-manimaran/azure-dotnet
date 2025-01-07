using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;
using Products.Api.TargetingFeature.Data;
using Products.Api.TargetingFeature.Features;
using Products.Api.TargetingFeature.Models;

namespace Products.Api.TargetingFeature.Controllers
{
    [Route("api/{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    [ApiVersion("2")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductDbContext _dbContext;
        private readonly ILogger<ProductsController> _logger;
        private readonly IFeatureManager _featureManager;

        public ProductsController(ProductDbContext dbContext, 
                                  ILogger<ProductsController> logger,
                                  IFeatureManager featureManager)
        {
            _dbContext = dbContext;
            _logger = logger;
            _featureManager = featureManager;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductResponseV1>>> GetProducts()
        {
            var response = await _dbContext.Products
                            .Select(p=> new ProductResponseV1
                            {
                                Id = p.Id,
                                Name = p.Name,
                                Price = p.Price,
                            }).ToListAsync();

            return Ok(response);
        }

        [HttpGet("{id:guid}")]
        [MapToApiVersion("1")]
        [FeatureGate(FeatureFlags.UseV1ProductApi)]
        public async Task<ActionResult<ProductResponseV1>> GetProductV1(Guid Id)
        {
            // Replace this with FeatureGate Attribute
            //if(!await _featureManager.IsEnabledAsync(FeatureFlags.UseV1ProductApi))
            //{
            //    return NotFound();
            //}
            var response = await _dbContext.Products
                            .Where(p => p.Id == Id)
                            .Select(p => new ProductResponseV1
                            {
                                Id = p.Id,
                                Name = p.Name,
                                Price = p.Price,
                            }).FirstOrDefaultAsync();

            if (response == null)
            {
                return NotFound();
            }
            _logger.LogInformation("Retrived the Product for Id {Id} using V1 endpoint",Id);

            return Ok(response);
        }

        [HttpGet("{id:guid}")]
        [MapToApiVersion("2")]
        [FeatureGate(FeatureFlags.UseV2ProductApi)]
        public async Task<ActionResult<ProductResponseV2>> GetProductV2(Guid Id)
        {
            // Replac the below with Feature Gate
            //if (!await _featureManager.IsEnabledAsync(FeatureFlags.UseV2ProductApi))
            //{
            //    return NotFound();
            //}

            var response = await _dbContext.Products
                            .Where(p => p.Id == Id)
                            .Select(p => new ProductResponseV2
                            {
                                Id = p.Id,
                                Product = new ProductInfoV2
                                {
                                    Name = p.Name,
                                    DisplayName = p.DisplayName,
                                    Pricing = new PricingInfoV2
                                    {
                                        Amount = p.Price,
                                        Currenncy = p.Currency,
                                        Discounted = p.Discounted,
                                    }
                                },
                                Inventory = new InventoryInfoV2
                                {
                                    Quantity = p.Quantity,
                                    InStock = p.Quantity > 0
                                }
                            }).FirstOrDefaultAsync();

            if (response == null)
            {
                return NotFound();
            }

            _logger.LogInformation("Retrived the Product for Id {Id} using V2 endpoint", Id);
            return Ok(response);
        }
    }
}
