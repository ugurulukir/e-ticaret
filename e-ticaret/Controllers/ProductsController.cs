using e_ticaret.Models;
using e_ticaret.Services;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace e_ticaret.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;

        public ProductsController(ApplicationDbContext context,IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }
        public IActionResult Index()
        {
            var products = context.Products.ToList();
            return View(products);
        }

        public IActionResult Create() 
        { 
            return View(); 
        }

        [HttpPost]
        public IActionResult Create(ProductDto productDto)
        {
            if(productDto.ImageFile == null) {
                ModelState.AddModelError("ImageFile", "Ürün görseli zorunludur!");
               }

            if(!ModelState.IsValid)
            {
                return View(productDto);
            }
            

            //resim kaydetme

            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName +=Path.GetExtension(productDto.ImageFile!.FileName);
            string imageFullPath = environment.WebRootPath + "/products/" + newFileName;
            using(var stream= System.IO.File.Create(imageFullPath)) 
            {
                productDto.ImageFile.CopyTo(stream);
            }

            //veritabanına yeni ürün kaydetme
            Product product = new Product()
            {
                   Name = productDto.Name,
                   Brand =productDto.Brand,
                   Category =productDto.Category,
                   Price =productDto.Price,
                   Description = productDto.Description,
                   ImageFileName = newFileName,
                   CreatedAt = DateTime.Now,
            };

            context.Products.Add(product);
            context.SaveChanges();

            return RedirectToAction("Index","Products") ;
        }

        public IActionResult Edit(int id)
        {
            var product = context.Products.Find(id);

            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            //create productDto from product
            var productDto = new ProductDto()
            {
                Name = product.Name,
                Brand = product.Brand,
                Category = product.Category,
                Price = product.Price,
                Description = product.Description,
            };

            ViewData["ProductId"] = product.Id;
            ViewData["ImageFileName"] = product.ImageFileName;
            ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy");
            return View(productDto);
        }

            [HttpPost]
            public IActionResult Edit(int id, ProductDto productDto)
            {
               var product=context.Products.Find(id);
               if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

               if(!ModelState.IsValid)
            {
                ViewData["ProductId"] = product.Id;
                ViewData["ImageFileName"] = product.ImageFileName;
                ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy");
                return View(productDto);
            }


               //update the image file if we have a new image file
               string newFileName=product.ImageFileName;
            if(productDto.ImageFile != null)
                {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                newFileName += Path.GetExtension(productDto.ImageFile.FileName);

                string imageFullPath = environment.WebRootPath + "/Products/" + newFileName;
                using(var stream=System.IO.File.Create(imageFullPath))
                {
                    productDto.ImageFile.CopyTo(stream);
                }

                //delete the old image
                string oldImageFullPath = environment.WebRootPath + "/Products" + product.ImageFileName;
                System.IO.File.Delete(oldImageFullPath);
            }

            //update the product in the database
            product.Name = productDto.Name;
            product.Brand= productDto.Brand;
            product.Category = productDto.Category;
            product.Price = productDto.Price;
            product.Description = productDto.Description;
            product.ImageFileName= newFileName;

            context.SaveChanges();
            return RedirectToAction("Index", "Products");
            }

        public IActionResult Delete(int id)
        {
            var product = context.Products.Find(id);
            if(product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            string ImageFullPath = environment.WebRootPath + "/Products" + product.ImageFileName;
            System.IO.File.Delete(ImageFullPath);

            context.Products.Remove(product);
            context.SaveChanges(true);
            return RedirectToAction("Index", "Products");
        }
        }
    }

