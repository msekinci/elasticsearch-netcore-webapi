using System.Net;
using Elasticsearch.API.DTOs;
using Elasticsearch.API.Repositories;

namespace Elasticsearch.API.Services
{
    public class ProductService
	{
		private readonly ProductRepository _productRepository;

        public ProductService(ProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ResponseDto<ProductDto>> SaveAsync(ProductCreateDto request)
        {
            var response = await _productRepository.SaveAsync(request.CreateProduct());

            if (response == null)
            {
                return ResponseDto<ProductDto>
                    .Fail(new List<string> { "Kayıt esnasında bir hata meydana geldi." },
                          System.Net.HttpStatusCode.InternalServerError);
            }

            return ResponseDto<ProductDto>.Success(response.CreateDto(), HttpStatusCode.Created);
        }

        public async Task<ResponseDto<List<ProductDto>>> GetAllAsync()
        {
            var products = await _productRepository.GetAllAsync();


            var productListDto = products
                                    .Select(x => new ProductDto(
                                        x.Id,
                                        x.Name,
                                        x.Price,
                                        x.Stock,
                                        x.Feature is null ? null : new ProductFeatureDto(x.Feature!.Width, x.Feature!.Height, x.Feature!.Color.ToString())))
                                    .ToList();

            return ResponseDto<List<ProductDto>>.Success(productListDto, HttpStatusCode.OK);
        }

        public async Task<ResponseDto<ProductDto>> GetByIdAsync(string id)
        {
            var response = await _productRepository.GetByIdAsync(id);

            if (response == null)
            {
                return ResponseDto<ProductDto>.Fail("Product was not found!", HttpStatusCode.NotFound);
            }

            return ResponseDto<ProductDto>.Success(response.CreateDto(), HttpStatusCode.OK);
        }
    }
}

