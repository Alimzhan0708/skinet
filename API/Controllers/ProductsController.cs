using System.Collections.Generic;
using System.Threading.Tasks;
using API.Dtos;
using API.Helpers;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IGenericRepository<Product> _productsRepo;
        private readonly IMapper _mapper;
        public ProductsController(IGenericRepository<Product> productsRepo, IMapper mapper)
        {
            _mapper = mapper;
            _productsRepo = productsRepo;
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
        public async Task<ProductToReturnDto> GetProduct(int id)
        {
            var spec = new ProductWithTypeAndBrandSpecification(id);
            var product = await _productsRepo.GetEntityWithSpec(spec);
            return _mapper.Map<ProductToReturnDto>(product);
        }
    }
}