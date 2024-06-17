using CursoMod165.Data;
using CursoMod165.Models;
using Microsoft.AspNetCore.Authorization;
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
            // Passar List Customer para a view
            ViewBag.CustomerList = new SelectList(_context.Customers, "ID", "Name");



           

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
                _context.SaveChanges();

                // So atualizo gravo depois de ter criado a venda  DateTime.Now
                ViewBag.id = sale.ID;
                
                string ViewBagStr = "";
                ViewBagStr = ViewBag.id.ToString();
                ViewBagStr = ViewBagStr.PadLeft(3, '0');

                // Falta Alterar a letra quando chego ao 999
                // ViewBag.codVenda = String.Format("V{0}A" + sale.ID.ToString(),DateTime.Now.Year); // sale.ID.ToString()

                ViewBag.codVenda = String.Format("V{0}A" + ViewBagStr,DateTime.Now.Year); // sale.ID.ToString()
                sale.CodVenda = ViewBag.codVenda;
                // tem de ser V + Date Now + A + Number
                //Sale? sale = _context.Sales.Find(id);
                //GetCodigoVenda(out startDate);

                _context.Update(sale);     // tens aqui varios pedido agora grava
                _context.SaveChanges();
                // Obter as consultas
                //ViewBag.CodVenda = startDate;


                return RedirectToAction("Index");
                // return RedirectToAction(nameof(index)); -> outra forma de apresentar igual

            }

            ViewBag.CustomerList = new SelectList(_context.Customers, "ID", "Name");
            return View(sale);



        }


        public IActionResult Details(int id)
        {
            Sale? sale = _context.Sales.Find(id);

            ViewBag.SaleID = id;
            ViewBag.NumVenda = sale.CodVenda;
            //ViewBag.Nome = sale.Customer.Name;
            ViewBag.Obs = sale.Observations;    
            ViewBag.ClienteID = sale.CustomerID;

            // DAdos do cliente
            // retornar uma Lista com os Dados do cliente, para preencher cabecalho
            Customer? clienteData = _context.Customers.Find(sale.CustomerID);
            ViewBag.ClienteName = clienteData.Name;
            ViewBag.City = clienteData.City;
            ViewBag.Address = clienteData.Address;
            ViewBag.Email= clienteData.Email;
            ViewBag.Zipcode= clienteData.ZipCode;



            // DAdos do cliente
            // retornar uma Lista com os Dados do cliente, para preencher cabecalho
            //var clienteData = _context
            //                    .Sales
            //                    .Include(s => s.Customer)
            //                    .Where(s => s.CodVenda == sale.CodVenda)
            //                    .ToList();


            //ViewBag.Cliente = clienteData;




            // retornar uma Lista com os Dados do cliente, para preencher cabecalho
            //var clienteProductLists = _context
            //                                 .ProductLists
            //                                 .Include(p => p.Sale.Customer)  // ???? .Category
            //                                 .Include(p => p.Sale)
            //                                 .Where(p => p.SaleID == sale.ID)
            //                                 .ToList();  // para ir à base de dados usar "_XXXXX"

            //ViewBag.ClienteName = clienteProductLists;



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



        // delete 1

        [HttpGet] //METODO QUE VAI DEVOLVER A VISTA DE DELETE 
        [Authorize(Policy = POLICIES.APP_POLICY_ADMIN.NAME)]  //  so o admin é que poode aceder ao delete, para evitar que pelo lado da vista se chame a pagina delete por URL
        public IActionResult Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewBag.CustomerList = new SelectList(_context.Customers, "ID", "Name");

            Sale? sale = _context.Sales.Find(id);

            if (sale == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(sale);
        }


        // para apagar o sale só preciso do ID não preciso dos dados do sale
        // aqui a vista mantem o nome delete, mas a acção é delete confirm
        // Testar isto : http s://localhost:8000/Sale/Delete/1
        [HttpPost, ActionName("Delete")]
        [Authorize(Policy = POLICIES.APP_POLICY_ADMIN.NAME)]  //  delete so p/ admin , no lado do controlador para evitar que pelo lado da vista se chame a pagina delete por URL
        public IActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Sale? sale = _context.Sales.Find(id);

            if (sale != null)
            {

                _context.Sales.Remove(sale);        // atualiza
                _context.SaveChanges();                     // grava _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(sale);
        }

        // delete 1






        // So é possivel obter a vista se fizer return da Vista:
        public IActionResult AddProductToList()
        {
            IEnumerable<Product> products = _context
                                                .Products
                                                .Include(c => c.Category)
                                                .ToList();  // para ir à base de dados usar "_XXXXX"
                                                            // return View("../Home/Index");
            return View(products);

            // return View();
        }



        // aqui apenas tenho de usar o ID da Sale, que se mantem e adicionar Produto ID
        // dentro da minha ProductList
        [HttpGet]
        public IActionResult AddToList(int id)  // private static void
        {


            // Passar List Products para a view
            ViewBag.ProductList = new SelectList(_context.ProductLists, "ID", "Name");



            // Aqui vou retornar ProductLista com COD.Venda fixo
            var orderProductLists = _context
                                              .ProductLists
                                              .Include(p => p.Product)  // ???? .Category
                                              .Include(p => p.Sale)
                                              .Where(p => p.SaleID == id)
                                              .ToList();  // para ir à base de dados usar "_XXXXX"

            return View(orderProductLists);
            // return View();
            // aqui tenho de criar um registo no Product List, c/ n Venda 

            // inicio
            //Sale? sale = _context.Sales;  // _context.Sales.Find(id)

            //_context.Sales.Add(sale);
            //_context.SaveChanges();     // tens aqui varios pedido agora grava





            // fim

            //return View();
        }




        [HttpPost]
        public IActionResult AddToList(ProductList productList)  // private static void
        {
            if (ModelState.IsValid)
            {
                // Se retora falso algo está mal preenchido, deve aparecer uma info e nao sair da pagina
                // TO Do Criar novo Staff caso contrario;
                // return view customer com dados anteriores
                _context.ProductLists.Add(productList);
                _context.SaveChanges();

                return RedirectToAction("Index");

            }

            // Codigo de venda
            // ViewBag.SaleList = new SelectList(_context.Sales, "ID", "CodVenda");
            // Passar List Products para a view
            //ViewBag.ProductList = new SelectList(_context.ProductLists, "ID", "Name");
            ViewBag.ProductList = new SelectList(_context.Products, "ID", "Description");
            return View();
        }



        // Adiciona produtos à ordem de venda
        [HttpGet]
        public IActionResult AddProductToOrder()
        {
            

            // envia p vista lista Produtos e vendas
            this.SetupProductList();
            return View();
        }




        [HttpPost]
        public IActionResult AddProductToOrder(ProductList productList)
        {



            if (ModelState.IsValid)
            {

                _context.ProductLists.Add(productList);
                // Procurar preço com ProdutoID
                Product? product = _context.Products.Find(productList.ProductID);
                ViewBag.Price = product.Price; 

                // Carrega valores automaticos , preço e cod de venda
                productList.Price= ViewBag.Price;    // =0;  ViewBag.ClienteID # Esta a funcionar OK
                
                // productList.SaleID= 3;      

                _context.SaveChanges();     // tens aqui varios pedido agora grava
                // ViewBag.CodVenda=productList.SaleID;

                // Toastr.SucessMessage tem de aparecer msg quando criar um novo
                _toastNotification.AddSuccessToastMessage("Product sucessfully Added to Order.");

                // Aqui passar com Details/{0},id ... parecido com isto 
                return RedirectToAction("Index");  //  ("Details/{0}", ViewBag.CodVenda
                // return RedirectToAction(nameof(index)); -> outra forma de apresentar igual

            }

            // Toastr.ERRORMessage aparecer msg em caso de falha 
            _toastNotification.AddErrorToastMessage("Error - Product not Added to Order.");

            // Envia Listas  para a vista
            //this.SetupProductList();

            return View(productList);
        }

        // Funcoes diversas aqui :
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


        // Funcao para retorno do Codigo de venda
        private static void GetCodigoVenda(out int codigoVenda)
        {
            int x = 1;
            //if (DateTime.Today.DayOfWeek != DayOfWeek.Sunday)
            //{
            //    x = 8 - (int)DateTime.Today.DayOfWeek;
            //}
            
            codigoVenda = (x + 4);
        }


        // END
    }

}





