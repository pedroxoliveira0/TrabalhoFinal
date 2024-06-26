using CursoMod165.Data;
using CursoMod165.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace CursoMod165.Controllers
{
    public class ShoppingBasketController : Controller
    {

        // aqui é definida a ligação para a base de dados
        private readonly ApplicationDbContext _context;  // premir com botão direito a lampada para tnasformar var _context

        // definicao de variaveis 
        private readonly IToastNotification _toastNotification; // Singleton -> ou seja tudo é igual em todo o lado as mesmas regras

        // faz ligacao à base de dados - original
        // aqui defino var Toastr e ligacao à base de dados
        // IToastNotification toastNotification)
        public ShoppingBasketController(ApplicationDbContext context,
            IToastNotification toastNotification)
        {
            // ligação à base de dados; emulação da base de dados
            _context = context;
            _toastNotification = toastNotification;
        }



        public IActionResult Index()   // OrderList()
        {
            //IEnumerable<ProductList> productLists = _context.ProductLists.ToList();    var productLists = _context
            var productLists = _context
                                                .ProductLists
                                                .Include(p => p.Sale)
                                                .Include(p => p.Sale.Customer)
                                                .Include(p => p.Product)
                                                .Include(p => p.Product.Category)
                                                .OrderBy(p => p.SaleID)
                                                .ToList();  // para ir à base de dados usar "_XXXXX"



            return View(productLists);
        }



        // #############################
        // Create 
        // Adiciona produtos à ordem de venda...
        [HttpGet]
        public IActionResult Create()
        {

            //ViewBag.SaleList = new SelectList(_context.Sales, "ID", "CodVenda");
            // Passar List Products para a view
            // ViewBag.ProductList = new SelectList(_context.ProductLists, "ID", "Name");
            //ViewBag.ProductList = new SelectList(_context.Products, "ID", "Description");  // Description
            // fazer uma lista nova incluindo acesso à base de dados categorias
            var ProductListx = _context.Products
                                               .Include(p => p.Category)  //inner join => a partir da chave estrangueiro quero o nome               
                                               .Select(p => new
                                               {
                                                   // coloca role à frente do nome na lista
                                                   // funcao select [aparece nome do medico  combinado com a sua profissao]
                                                   ID = p.ID,  // = p.ID
                                                   Name = $"{p.Description} [{p.Category.Name}]"
                                               });



            ViewBag.ProductList = new SelectList(ProductListx, "ID", "Name");  //  ProductListx, "ID", "Name"


            // ViewData["Titulo1"] = "Trabalho Final Curso ASP.NET 165";
            ViewBag.SaleList = new SelectList(_context.Sales, "ID", "CodVenda");





            // ViewBag.Categories = new SelectList(_context.Categories, "ID", "Name");
            return View();
        }

        [HttpPost]
        public IActionResult Create(ProductList productList)
        {



            if (ModelState.IsValid)
            {




                // Ler preço do produto escolhido
                // productList.Price = 111;
                Product ? product = _context.Products.Find(productList.ProductID);
                productList.Price = product.Price;


                // Atualizar quantidades em stock; colocar if (product.Quantity>=productList.Quantity)
                // isto nao é feito aqui so qd for para embalar ...
                // product.Quantity = product.Quantity - productList.Quantity;

                // Atualizar Valor total da encomenda
                Sale? sale = _context.Sales.Find(productList.SaleID);
                sale.TotalPrice=sale.TotalPrice+(productList.Price * productList.Quantity);
        


                _context.ProductLists.Add(productList);
                _context.SaveChanges();     // tens aqui varios pedido agora grava

                // Toastr.SucessMessage tem de aparecer msg quando criar um novo
                _toastNotification.AddSuccessToastMessage("Product sucessfully Added to Order.");

                return RedirectToAction("Index");
                // return RedirectToAction(nameof(index)); -> outra forma de apresentar igual

                
            }

            // Toastr.ERRORMessage aparecer msg em caso de falha 
            _toastNotification.AddErrorToastMessage("Error - Product not Added to Order.");

            

            // Envia Listas  para a vista
            this.SetupProductList();

            return View(productList);
        }


        // ###################################################
        //
        //      EditL
        //
        // #################################################
        // Adiciona produtos à ordem de venda...
        [HttpGet]
        public IActionResult Edit(int id)
        {
            ProductList? productList = _context.ProductLists.Find(id);  // Chave primaria = id


            if (productList == null) // se for diferente de null faz a vista
            {


                return RedirectToAction(nameof(Index));
            }



            // Envia Listas  para a vista
            this.SetupProductList();
            // ViewBag.Categories = new SelectList(_context.Categories, "ID", "Name");
            return View(productList);
        }

        [HttpPost]   // envia dados para a base de dados
        public IActionResult Edit(ProductList productList)
        {
            if (ModelState.IsValid)
            {
                _context.ProductLists.Update(productList);        // atualiza
                _context.SaveChanges();                     // grava

                // Toastr.SucessMessage tem de aparecer msg quando criar um novo
                _toastNotification.AddSuccessToastMessage("Product order sucessfully updated.");

                return RedirectToAction(nameof(Index));   // volta para a pagina principal, para o cliente saber que gravou, mostra lista
            }

            // Toastr.ERRORMessage aparecer msg em caso de falha 
            _toastNotification.AddErrorToastMessage("Error - Product order not updated.");
            //ViewBag.CategoryList = new SelectList(_context.Categories, "ID", "Name");

            // Envia Listas  para a vista
            this.SetupProductList();

            return View();
        }

        // ####################################### end EDIT




        // ###################################################
        //
        //      Delete
        //
        // #################################################
        // Adiciona produtos à ordem de venda...
        [HttpGet]
        public IActionResult Delete(int id)
        {
            ProductList? productList = _context.ProductLists.Find(id);  // Chave primaria = id


            if (productList == null) // se for diferente de null faz a vista
            {


                return RedirectToAction(nameof(Index));
            }



            // Envia Listas  para a vista
            this.SetupProductList();
            // ViewBag.Categories = new SelectList(_context.Categories, "ID", "Name");
            return View(productList);
        }

        [HttpPost]   // envia dados para a base de dados
        public IActionResult Delete(ProductList productList)
        {
            if (ModelState.IsValid)
            {
                _context.ProductLists.Remove(productList);        // Apaga
                _context.SaveChanges();                     // grava

                // Toastr.SucessMessage tem de aparecer msg quando criar um novo
                _toastNotification.AddSuccessToastMessage("Product order sucessfully updated.");

                // return RedirectToAction(nameof(Index));   // volta para a pagina principal, para o cliente saber que gravou, mostra lista
                // return Redirect("~/Sale/Index.cshtml");  
                return RedirectToAction(actionName: "Index", controllerName: "Sale");
            }

            // Toastr.ERRORMessage aparecer msg em caso de falha 
            _toastNotification.AddErrorToastMessage("Error - Product order not updated.");
            //ViewBag.CategoryList = new SelectList(_context.Categories, "ID", "Name");

            // Envia Listas  para a vista
            this.SetupProductList();

            return View();
        }

        // ####################################### end Delete




        // ###########################
        //  Metodos
        // #######################
        private void SetupProductList()
        {
            // retorna Listas dos produtos e Num. Venda para as Vistas
            var ProductListx = _context.Products
                                    .Include(p => p.Category)  //inner join => a partir da chave estrangueiro quero o nome               
                                    .Select(p => new
                                    {
                                        // coloca role à frente do nome na lista
                                        // funcao select [aparece nome do medico  combinado com a sua profissao]
                                        ID = p.ID,  // = p.ID
                                        Name = $"{p.Description} [{p.Category.Name}]"
                                    });


            ViewBag.ProductList = new SelectList(ProductListx, "ID", "Name");  //  ProductListx, "ID", "Name"


            // ViewData["Titulo1"] = "Trabalho Final Curso ASP.NET 165";
            ViewBag.SaleList = new SelectList(_context.Sales, "ID", "CodVenda");

        }

    }
}
