using CursoMod165.Data;
using CursoMod165.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Localization;
using NToastNotify;   // para poder usar os toastr notifications
using System.Text;



namespace CursoMod165.Controllers
{
    public class ProductListController : Controller
    {


        // aqui é definida a ligação para a base de dados
        private readonly ApplicationDbContext _context;


        private readonly IToastNotification _toastNotification; // Singleton -> ou seja tudo é igual em todo o lado as mesmas regras

        // faz ligacao à base de dados - original
        //public CategoryController(ApplicationDbContext context)
        //{
        //    _context = context;
        //}

        // aqui defino var Toastr e ligacao à base de dados
        // IToastNotification toastNotification)
        public ProductListController(ApplicationDbContext context,
            IToastNotification toastNotification)

        {
            // ligação à base de dados; emulação da base de dados
            _context = context;
            _toastNotification = toastNotification;

        }


        public IActionResult Index()
        {
            //IEnumerable<ProductList> productLists = _context.ProductLists.ToList();    var productLists = _context
            var productLists = _context
                                                .ProductLists
                                                .Include(p => p.Sale)
                                                .Include(p => p.Sale.Customer)
                                                .Include(p => p.Product)
                                                .Include(p => p.Product.Category)
                                                .ToList();  // para ir à base de dados usar "_XXXXX"



            return View(productLists);
        }




        // ###################################################
        //
        //      Create
        //
        // #################################################
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
                                               .Select(p => new {
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

                _context.ProductLists.Add(productList);
                _context.SaveChanges();     // tens aqui varios pedido agora grava

                // Toastr.SucessMessage tem de aparecer msg quando criar um novo
                _toastNotification.AddSuccessToastMessage("Product sucessfully Created.");

                return RedirectToAction("Index");
                // return RedirectToAction(nameof(index)); -> outra forma de apresentar igual

            }
            //ViewBag.SaleList = new SelectList(_context.Sales, "ID", "CodVenda");
            // Passar List Products para a view
            //ViewBag.ProductList = new SelectList(_context.ProductLists, "ID", "Description");
            return View(productList);
        }



    }
}

