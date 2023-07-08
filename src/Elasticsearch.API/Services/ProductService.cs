using System.Net;
using Elastic.Clients.Elasticsearch;
using Elasticsearch.API.DTOs;
using Elasticsearch.API.Repositories;

namespace Elasticsearch.API.Services
{
    public class ProductService
    {
        private readonly ProductRepository _productRepository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(ProductRepository productRepository, ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<ResponseDto<ProductDto>> SaveAsync(ProductCreateDto request)
        {
            var response = await _productRepository.SaveAsync(request.CreateProduct());

            if (response == null)
            {
                return ResponseDto<ProductDto>
                    .Fail(new List<string> { "Kayıt esnasında bir hata meydana geldi." },
                          HttpStatusCode.InternalServerError);
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

        public async Task<ResponseDto<bool>> UpdateAsync(ProductUpdateDto updateProduct)
        {
            var response = await _productRepository.UpdateAsync(updateProduct);

            if (!response!.IsValidResponse)
            {
                if (response!.Result == Result.NotFound)
                {
                    return ResponseDto<bool>.Fail("Product was not found!", HttpStatusCode.NotFound);
                }

                response.TryGetOriginalException(out Exception? exception);
                _logger.LogError(exception, response.ElasticsearchServerError.Error.ToString());
                return ResponseDto<bool>.Fail("Product was not updated!", HttpStatusCode.InternalServerError);
            }

            return ResponseDto<bool>.Success(true, HttpStatusCode.NoContent);
        }

        public async Task<ResponseDto<bool>> DeleteAsync(string id)
        {
            var response = await _productRepository.DeleteAsync(id);

            if (!response!.IsValidResponse)
            {
                if (response.Result == Result.NotFound)
                {
                    return ResponseDto<bool>.Fail("Product was not found!", HttpStatusCode.NotFound);
                }

                response.TryGetOriginalException(out Exception? exception);
                _logger.LogError(exception, response.ElasticsearchServerError.Error.ToString());
                return ResponseDto<bool>.Fail("Product was not deleted!", HttpStatusCode.InternalServerError);
            }

            return ResponseDto<bool>.Fail("Product was not deleted!", HttpStatusCode.InternalServerError);
        }
    }
}

