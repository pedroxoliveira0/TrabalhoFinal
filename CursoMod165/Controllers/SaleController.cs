using CursoMod165.Data;
using CursoMod165.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Localization;
using NToastNotify;  // Permite Mostrar mensagens informativas na pagina
using System.Text;
using static CursoMod165.CursoMod165Constants;

namespace CursoMod165.Controllers
{
	public class SaleController : Controller
	{
        // aqui é definida a ligação para a base de dados
        private readonly ApplicationDbContext _context;  // premir com botão direito a lampada para tnasformar var _context

        // definicao de variaveis 
        private readonly IToastNotification _toastNotification; // Singleton -> ou seja tudo é igual em todo o lado as mesmas regras

        // faz ligacao à base de dados - original
        // aqui defino var Toastr e ligacao à base de dados
        // IToastNotification toastNotification)
        public SaleController(ApplicationDbContext context,
            IToastNotification toastNotification)
        {
            // ligação à base de dados; emulação da base de dados
            _context = context;
            _toastNotification = toastNotification;
        }

        
        public IActionResult Index()
        {
            IEnumerable<Sale> sales = _context
                                                .Sales
                                                .Include(s => s.Customer)
                                                .ToList();  // para ir à base de dados usar "_XXXXX"
                                                            // return View("../Home/Index");
            return View(sales);  // tenho de colocar aqui a tabela de base de dados se não dá erro por retornar Null
            // return View();
        }
        
        
        //  Para esta lista funcionar tenho de ter o ID da Venda
        //    public IActionResult Index()
        //  {
        //      IEnumerable<ProductList> productList = _context
        //                                  .ProductLists
        //                                  .Include(p => p.Product)
        //                                  .Include(p => p.Sale)
        //                                  .Include(p => p.Sale.Customer)  // Será que assim consigo obter os dados da tabela customer
        //                                  .ToList();

         //    return View(productList);

        //  }



        // #########################################
        //
        // Details, apresenta apenas informacao, nao permite alterar,
        // logo otimo para utilizadores que nao podem "mexer"
        //
        // ##########################################
        public IActionResult Edit(int id)
        {
            
            // Vou retornar duas Listas uma referente à venda com o cliente e
            // a outra referente aos produtos cuja numero de venda é igual à da Lista anterior
            Sale? sale = _context.Sales.Find(id);  // Chave primaria = id  

            if (sale == null) // se for diferente de null faz a vista
            {
                return RedirectToAction(nameof(Index));
            
                // Esta lista tem re retornar num viewBag
            
            }
            return View(sale);



            // Obetr o ID da venda selecionada
            //  ProductList? productList = _context.ProductLists.Find(id);  // Chave primaria = id  
            // devolve lista de elementos com base no ID

            // if (productList == null) // se for diferente de null faz a vista
            //  {
            //      return RedirectToAction(nameof(Index));
            //  }
            // se não existir erro devolve lista do ID selecionado
            // na view tenho de colocar os dados que quero mostrar
            //  return View(productList);

            // tenho de devolver também a lista de produtos ...
            // em formato tabela

        }





        // ##########################################
        //                CREATE
        // ##########################################
        // apresenta formuladrio para preencher
        [HttpGet]
        public IActionResult Create()
        {

          
            return View();
            // return View();
        }


        // envia dados preenchidos no formulario
        [HttpPost]
        public IActionResult Create(Sale sale)
        {
            // ver exemplo Appointment, rever aula
            // com os dados preenchidos no formlario Create Staff, criar registo na Dbase
            if (ModelState.IsValid)
            {
                // Se retora falso algo está mal preenchido, deve aparecer uma info e nao sair da pagina
                // TO Do Criar novo Staff caso contrario;
                // return view customer com dados anteriores
                _context.Sales.Add(sale);
                _context.SaveChanges();     // tens aqui varios pedido agora grava

                return RedirectToAction("Index");
                // return RedirectToAction(nameof(index)); -> outra forma de apresentar igual

            }
            

            return View(sale);
        }


        public IActionResult Details(int id)
        {
            Sale? sale = _context.Sales.Find(id);

            ViewBag.NumVenda = sale.CodVenda;
            //ViewBag.Nome = sale.Customer.Name;
            ViewBag.Obs = sale.Observations;
            ViewBag.ClienteID = sale.CustomerID;


            //IEnumerable<ProductList> productLists = _context
            //                                  .ProductLists
            //                                  .Include(p => p.Product)
            //                                  .Include(p => p.Sale)
            //                                  .ToList();  // para ir à base de dados usar "_XXXXX"
            //                                              // return View("../Home/Index");

            var orderProductLists = _context
                                              .ProductLists
                                              .Include(p => p.Product)  // ???? .Category
                                              .Include(p => p.Sale)
                                              .Where(p => p.SaleID == sale.ID)
                                              .ToList();  // para ir à base de dados usar "_XXXXX"



            if (orderProductLists == null) // se for diferente de null faz a vista
            {
                return RedirectToAction(nameof(Index));

                // Esta lista tem re retornar num viewBag

            }

            return View(orderProductLists);
            //return View();
        }


        




        private void SetupSales()
        {
            ViewBag.CustomerList = new SelectList(_context.Customers, "ID", "Name");

            // uma possibilidade o role ser condicionado só tem acesso que nos queremos
            //_context.Staffs.Where(s => s.StaffRoleID != 1);     // esta solução é provisoria por nao ser escalável


            //    esta é a solução correta ver staff roles models
            var staffList = _context.Staffs
                                    .Include(s => s.StaffRole)   // s são todos os elementos que estão no dB set
                                    .Where(s => s.StaffRole.CanDoAppointments == true)
                                    .Select(s => new {
                                        // coloca role à frente do nome na lista
                                        ID = s.ID,
                                        Name = $"{s.Name} [{s.StaffRole.Name}]"
                                    });

            ViewBag.StaffList = new SelectList(staffList, "ID", "Name");


            // em SQL faz-se assim: (por convenção é assim)
            //      selectList *
            //      From staff s
            //      INNER JOIN staffRoles sr
            //      ON s.StaffRoleID = sr.ID;

        }




        // END
    }

}





