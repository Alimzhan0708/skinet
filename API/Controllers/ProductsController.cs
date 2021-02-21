using System.Collections.Generic;
using System.Threading.Tasks;
using API.Dtos;
using API.Errors;
using API.Helpers;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IGenericRepository<Product> _productsRepo;
        private readonly IGenericRepository<ProductBrand> _brandRepo;
        private readonly IGenericRepository<ProductType> _typeRepo;
        private readonly IMapper _mapper;
        public ProductsController(IGenericRepository<Product> productsRepo, IGenericRepository<ProductBrand> brandRepo, IGenericRepository<ProductType> typeRepo, IMapper mapper)
        {
            _mapper = mapper;
            _productsRepo = productsRepo;
            _brandRepo = brandRepo;
            _typeRepo = typeRepo;
        }

        [HttpGet]
        public async Task<Pagination<ProductToReturnDto>> GetProducts([FromQuery] ProductSpecParams productSpecParams)
        {
            var countSpec = new ProductsWithFiltersAndCountSpecification(productSpecParams);
            var totalItems = await _productsRepo.CountAsync(countSpec);

            var spec = new ProductsWithTypesAndBrandsSpecification(productSpecParams);
            var products = await _productsRepo.GetAllWithSpec(spec);
            
            var data = _mapper.Map<IReadOnlyList<ProductToReturnDto>>(products);
            return new Pagination<ProductToReturnDto>(productSpecParams.PageIndex, productSpecParams.PageSize, totalItems, data);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            var spec = new ProductWithTypeAndBrandSpecification(id);
            var product = await _productsRepo.GetEntityWithSpec(spec);
            if (product is null) return NotFound();
            return _mapper.Map<ProductToReturnDto>(product);
        }

        [HttpGet("brands")]
        public async Task<IReadOnlyList<ProductBrand>> GetBrands()
        {
            return await _brandRepo.GetAll();
        }

        [HttpGet("types")]
        public async Task<IReadOnlyList<ProductType>> GetTypes()
        {
            return await _typeRepo.GetAll();
        }
    }
}