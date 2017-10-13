using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ApiApp.Services;
using ApiApp.DTO;
using ApiApp.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;

namespace ApiApp.Controllers
{
    //[Produces("application/json")]
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        private IProductService _productService;
        private ILogger<ProductController> _logger; // net core内置日志

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        public IActionResult GetProducts()
        {
            return Ok(_productService.GetList());
        }

        [Route("{id}", Name = "GetProduct")]
        public IActionResult GetProduct(int id)
        {
            var product = _productService.Get(id);
            if (product == null)
            {
                _logger.LogInformation($"Id为{id}的产品不存在");
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        public IActionResult Post([FromBody]ProductCreation product)
        {
            if (product == null)
            {
                _logger.LogInformation($"product参数为空");
                return BadRequest();
            }

            if (product.Name.Equals("产品"))
            {
                ModelState.AddModelError("Name", "产品的名称不能是\"产品\"二字");
            }

            // validation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newProduct = new Product
            {
                Id = _productService.NewId,
                Name = product.Name,
                Price = product.Price
            };
            _productService.Add(newProduct);

            return CreatedAtRoute("GetProduct", new { id = newProduct.Id }, newProduct);
        }

        // 全部修改
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] ProductModification product)
        {
            if (product == null)
            {
                _logger.LogInformation($"product参数为空");
                return BadRequest();
            }

            if (product.Name.Equals("产品"))
            {
                ModelState.AddModelError("Name", "产品的名称不能是\"产品\"二字");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var p = _productService.Get(id);
            if (p == null)
            {
                _logger.LogInformation($"Id为{id}的产品不存在");
                return NotFound();
            }

            p.Name = product.Name;
            p.Price = product.Price;

            // validation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _productService.Update(p);

            return NoContent();
        }

        // 部分修改
        // [{"op":"replace或remove", "path":"/name", "value":"Patched new name"},{"op":"replace","path":"/price","value":'1230"}]
        [HttpPatch("{id}")]
        public IActionResult Patch(int id, [FromBody] JsonPatchDocument<ProductModification> patchDoc)
        {
            if (patchDoc == null)
            {
                _logger.LogInformation($"patchDoc参数为空");
                return BadRequest();
            }

            var model = _productService.Get(id);
            if (model == null)
            {
                _logger.LogInformation($"Id为{id}的产品不存在");
                return NotFound();
            }

            var toPatch = new ProductModification
            {
                Name = model.Name,
                Price = model.Price
            };
            patchDoc.ApplyTo(toPatch, ModelState);

            // validation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TryValidateModel(toPatch);

            // validation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            model.Name = toPatch.Name;
            model.Price = toPatch.Price;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var product = _productService.Get(id);
            if (product == null)
            {
                _logger.LogInformation($"Id为{id}的产品不存在");
                return NotFound();
            }
            _productService.Delete(id);

            return NoContent();
        }
    }
}