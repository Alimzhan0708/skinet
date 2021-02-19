using System.Collections.Generic;
using System.Threading.Tasks;
using API.Dtos;
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
        public async Task<IEnumerable<ProductToReturnDto>> GetProducts([FromQuery] ProductSpecParams productSpecParams)
        {
            var spec = new ProductsWithTypesAndBrandsSpecification(productSpecParams);
            var products = await _productsRepo.GetAllWithSpec(spec);
            return _mapper.Map<IEnumerable<ProductToReturnDto>>(products);
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