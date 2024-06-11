using CursoMod165.Data;
using CursoMod165.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using NToastNotify;

using System.Text;
using Microsoft.AspNetCore.Authorization;    // para activar login
using Microsoft.AspNetCore.Identity.UI.Services;  // servico email

namespace CursoMod165.Controllers
{

    public class WareHouse : Controller
    {
        // cria logo aqui o construtor
        private readonly ApplicationDbContext _context;
        // Nova 23-04

       

        // -- Index
        public IActionResult Index()
        {

            IEnumerable<ProductList> productLists = _context
                                               .ProductLists
                                               .Include(p => p.Product)
                                               .ToList();
            //.Include(p => p.Product)
            //.Include(p => p.Sale)
            //.Include(p => p.Sale.Customer)
            // para ir à base de dados usar "_XXXXX"
            // return View("../Home/Index");
            // deu erro:  .Include(p => p.Product.Category)

            return View(productLists);
            
        }
    }
}
