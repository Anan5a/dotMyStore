using Azure.Core;
using DataAccess;
using DataAccess.IRepository;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.DTOs;
using Utility;
using System.Linq;


namespace MyStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductImageRepository _productImageRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };


        private static long MAX_UPLOAD_SIZE = 3 * 1024 * 1024;

        public ProductController(IProductRepository productRepository,
            IProductImageRepository productImageRepository,
            IWebHostEnvironment webHostEnvironment)
        {
            _productRepository = productRepository;
            _webHostEnvironment = webHostEnvironment;
            _productImageRepository = productImageRepository;
        }
        // GET: api/<ProductController>
        [HttpGet]
        public async Task<IEnumerable<Product>> Get()
        {
            IEnumerable<Product> productList = _productRepository.GetAll(includeProperties: "true");
            if (productList == null)
            {
                return Enumerable.Empty<Product>();
            }
            return productList;
        }

        // GET api/<ProductController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> Get(int id)
        {
            Dictionary<string, dynamic> condition = new Dictionary<string, dynamic>();
            condition["Id"] = id;
            //condition["Email"] = EmailAddressAttribute;
            Product? product = _productRepository.Get(condition, includeProperties: "true");
            if (product == null)
            {
                return NotFound(ErrorResponse.NotFound());
            }
            return product;
        }

        // POST api/<ProductController>
        [HttpPost]
        public async Task<ActionResult<Product>> Post([FromForm] ProductCreateDto productCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            //first create product, then images
            Product product = new Product
            {
                Description = productCreateDto.Description,
                Price = productCreateDto.Price,
                SKU = productCreateDto.SKU,
                Title = productCreateDto.Title,
                Weight = productCreateDto.Weight,
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now,
            };
            int id = _productRepository.Add(product);

            //check if success, otherwise no point in handling images
            if (id == 0)
            {
                return UnprocessableEntity(ErrorResponse.ErrorCustom("Could not create product.", "Product creation failed. Something catastrophically went wrong!"));
            }
            //initialize image urls
            product.ImageUrls = new List<string>();
            //process images

            if (productCreateDto.Images != null)
            {
                foreach (var imageFile in productCreateDto.Images)
                {
                    // Process and save each image
                    if (imageFile == null || imageFile.Length == 0)
                    {
                        continue; // Skip empty files
                    }
                    var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
                    if (!AllowedExtensions.Contains(fileExtension))
                    {
                        //skip invalid images
                        continue;
                    }


                    if (imageFile.Length > MAX_UPLOAD_SIZE)
                    {
                        continue;
                    }

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);

                    // Determine the directory to save the image
                    string baseFilePath = _webHostEnvironment.WebRootPath;
                    string filePathDb = Path.Combine("images", fileName);
                    var filePath = Path.Combine(baseFilePath, filePathDb);
                    // Save the image to the server
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    //add to database
                    ProductImage productImage = new ProductImage
                    {
                        ImageUrl = filePathDb,
                        ProductId = id,
                    };
                    _productImageRepository.Add(productImage);
                    product.ImageUrls.Add(productImage.ImageUrl);
                }

            }

            string uri = $"/api/product/{id}";
            product.Id = id;
            return Created(uri, product);
        }

        // PUT api/<ProductController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Product>> Put(int id, [FromBody] ProductUpdateDto productUpdateDto)
        {
            Dictionary<string, dynamic> condition = new Dictionary<string, dynamic>();
            condition["Id"] = id;

            Product? existingProduct = _productRepository.Get(condition);
            if (existingProduct == null)
            {
                return NotFound(ErrorResponse.ErrorCustom("Not Found", "No product was found"));
            }

            int modificationCounter = 0;
            if (productUpdateDto.Title != null)
            {
                modificationCounter++;
                existingProduct.Title = productUpdateDto.Title;
            }
            if (productUpdateDto.Description != null)
            {
                modificationCounter++;
                existingProduct.Description = productUpdateDto.Description;
            }
            if (productUpdateDto.SKU != null)
            {
                modificationCounter++;
                existingProduct.SKU = productUpdateDto.SKU;
            }
            if (productUpdateDto.Price != null)
            {
                modificationCounter++;
                existingProduct.Price = (decimal)productUpdateDto.Price;
            }
            if (productUpdateDto.Weight != null)
            {
                modificationCounter++;
                existingProduct.Weight = (decimal)productUpdateDto.Weight;
            }
            if (modificationCounter > 0)
            {
                existingProduct.ModifiedAt = DateTime.Now;
            }
            //check if password is valid for updating password




            if (_productRepository.Update(existingProduct) == null)
            {
                return UnprocessableEntity(ErrorResponse.ErrorCustom("Entity not updated", "No property was updated."));
            }
            return existingProduct;
        }


        // DELETE api/<ProductController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            Dictionary<string, dynamic> condition = new Dictionary<string, dynamic>();
            condition["Id"] = id;
            Product? product = _productRepository.Get(condition, includeProperties: "true");
            if (product == null)
            {
                return NotFound(ErrorResponse.NotFound());
            }
            //check if exist and delete images if any

            if (product.ImageUrls != null && product.ImageUrls.Count > 0)
            {
                condition = new Dictionary<string, dynamic>();
                condition["ProductId"] = id;

                var allImages = _productImageRepository.GetAll(condition);
                var imageIdsToRemove = new List<int>();

                if (allImages != null)
                {
                    foreach (var image in allImages)
                    {
                        imageIdsToRemove.Add((int)image.Id);
                        //delete files from disk

                        var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, image.ImageUrl);

                        if (!System.IO.File.Exists(fullPath))
                        {
                            continue;
                        }
                        // Attempt to delete the file
                        System.IO.File.Delete(fullPath);
                    }
                    _productImageRepository.RemoveRange(imageIdsToRemove);
                }
            }
            //now remove product
            int status = _productRepository.Remove(id);

            if (status == 0)
            {
                return UnprocessableEntity(ErrorResponse.ErrorCustom("Failed to process id", "Nothing was deleted."));
            }
            return Ok(new
            {
                message = $"Successfully deleted {status} items."
            });
        }
    }
}
