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
        private readonly IHtmlLocalizer<Resource> _sharedLocalizer;
        private readonly IStringLocalizer<Resource> _localizer;
        private readonly IEmailSender _emailSender;
        private object htmlBody;


        // faz ligacao à base de dados - original
        // aqui defino var Toastr e ligacao à base de dados
        // IToastNotification toastNotification)
        public SaleController(ApplicationDbContext context,
            IToastNotification toastNotification,
            IHtmlLocalizer<Resource> sharedLocalizer,
            IStringLocalizer<Resource> localizer,
            IEmailSender emailSender)
            {
            // ligação à base de dados; emulação da base de dados
            _context = context;
            _toastNotification = toastNotification;
            _sharedLocalizer = sharedLocalizer;
            _localizer = localizer;
            _emailSender = emailSender;
            }





        public IActionResult Index()
        {

            var salesif = _context
                                                .Sales
                                                .Include(s => s.Customer)
                                                .ToList();
            // Resultados Vendor
            if (User.IsInRole(CursoMod165Constants.ROLES.VENDOR)) // (c)
            {
                var salesifvendor = _context
                                                .Sales
                                                .Include(s => s.Customer)
                                                .Where(s=>s.Status==Status.Ordered)
                                                .ToList();
                salesif = salesifvendor;
            }

            // Resultados WareHouse
            if (User.IsInRole(CursoMod165Constants.ROLES.WAREHOUSEMAN)) // (c)
            {
                var salesifwarehouse = _context
                                                .Sales
                                                .Include(s => s.Customer)
                                                .Where(s => s.Status != Status.Ordered)
                                                .Where(s => s.Status != Status.Sent)
                                                .ToList();
                salesif = salesifwarehouse;
            }

            // Resultados se for Admin:
            //if (User.IsInRole(CursoMod165Constants.ROLES.ADMIN) // (c)
            //{
             //   var salesifadmin = _context
             //                                   .Sales
             //                                   .Include(s => s.Customer)
             //                                   .ToList();
            // }

            // @if (User.IsInRole(CursoMod165Constants.ROLES.VENDOR) || (User.IsInRole(CursoMod165Constants.ROLES.ADMIN))) // (c)


            return View(salesif);  // (sales) tenho de colocar aqui a tabela de base de dados se não dá erro por retornar Null
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




        // ##############################
        //  Metodos: Ordered, Processed, Sent, IsPaid
        // #############################
        [Authorize(Policy = POLICIES.APP_POLICY_VENDOR.NAME)]
        public IActionResult Ordered()
        {
            // tranco as chaves estrangeiras e com condição date == data de amanhã
            var PurchaseOrdered = _context
                                                .Sales
                                                .Include(p => p.Customer)
                                                .Where(p => p.Status == Status.Ordered)  // Ordered = 1
                                                .ToList();

            
            return View(PurchaseOrdered);
        }

        [Authorize(Policy = POLICIES.APP_POLICY_WAREHOUSEMAN.NAME)]
        public IActionResult Processed()
        {
            // tranco as chaves estrangeiras e com condição date == data de amanhã
            var ProcessedOrdered = _context
                                                .Sales
                                                .Include(p => p.Customer)
                                                .Where(p => p.Status == Status.Purchase_Process)  // In-process = 2
                                                .ToList();


            return View(ProcessedOrdered);
        }

        [Authorize(Policy = POLICIES.APP_POLICY_WAREHOUSEMAN.NAME)]
        public IActionResult Sent()
        {
            // tranco as chaves estrangeiras e com condição date == data de amanhã
            var SentOrdered = _context
                                                .Sales
                                                .Include(p => p.Customer)
                                                .Where(p => p.Status == Status.Sent)  // Sent = 4
                                                .ToList();


            return View(SentOrdered);
        }

        [Authorize(Policy = POLICIES.APP_POLICY_WAREHOUSEMAN.NAME)]
        public IActionResult IsPaid()
        {
            // tranco as chaves estrangeiras e com condição date == data de amanhã
            var IsPaidOrdered = _context
                                                .Sales
                                                .Include(p => p.Customer)
                                                .Where(p => p.IsPaid == true)  // Packadged = 3
                                                .ToList();


            return View(IsPaidOrdered);
        }

        // ###################################################
        //
        //      Edit
        //
        // #################################################
        // Adiciona produtos à ordem de venda...

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
            // Envia Listas  para a vista
            this.SetupProductList();
            ViewBag.CustomerList = new SelectList(_context.Customers, "ID", "Name");
            return View(sale);


        }



        [HttpPost]   // envia dados para a base de dados
        public IActionResult Edit(Sale sale)  // (ProductList productList
        {
            if (ModelState.IsValid)
            {
                _context.Sales.Update(sale);        // atualiza
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
            ViewBag.CustomerList = new SelectList(_context.Customers, "ID", "Name");
            return View();
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
            // Order status:
            ViewBag.Status = sale.Status;

            // DAdos do cliente
            // retornar uma Lista com os Dados do cliente, para preencher cabecalho
            Customer? clienteData = _context.Customers.Find(sale.CustomerID);
            ViewBag.ClienteName = clienteData.Name;
            ViewBag.City = clienteData.City;
            ViewBag.Address = clienteData.Address;
            ViewBag.Email= clienteData.Email;
            ViewBag.Zipcode= clienteData.ZipCode;
            


            // Envio do preço total para a vista
            // retorna lista com CodVenda como parametro de entrada
            var produtListBySetCodVenda = _context
                                                .ProductLists
                                                .Include(p => p.Sale)
                                                .Include(p => p.Sale.Customer)
                                                .Include(p => p.Product)
                                                .Include(p => p.Product.Category)
                                                .Where(p => p.SaleID == id)
                                                .ToList();
                                                
            var TotalSum=produtListBySetCodVenda.Sum(p=>p.Quantity*p.Price);
            ViewBag.TotalSum = TotalSum;
            var num = Math.Round(TotalSum,2);

            

            ViewBag.Totals = TotalSum; //
            ViewBag.Totals = num; //

            // Atualiza campo Valor total(preço) no mod. vendas
            sale.TotalPrice= TotalSum;
            
            _context.Sales.Update(sale);
            _context.SaveChanges();

            // Atualiza Valores na Base de dados



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

            ViewBag.CustomerList = new SelectList(_context.Customers, "ID", "Name");
            return View(sale);
        }

        // delete 1





        public IActionResult ChoiceFromAllProductList(int id)
        {
            // IEnumerable<Product> products = _context
            // 
            // Esta é a minha Lista de produtos ...
            var ProductOrderList = _context
                                      .ProductLists
                                      .Include(c => c.Product)
                                      .Include(c => c.Sale)
                                      .Include(c => c.Product.Category)
                                      .ToList();  // para ir à base de dados usar "_XXXXX"
                                                  // return View("../Home/Index");

            var Allproducts = _context
                                .Products
                                .Include(c => c.Category)
                                .ToList();
            
            ViewBag.Quantity = 0;
            ViewBag.OrderID = id;
            ViewBag.AllProductsList = Allproducts;
            return View(ProductOrderList);
            // return View();

        }


        // ##############################
        //  So é possivel obter a vista se fizer return da Vista:
        //  Aqui adiciona produto à encomenda
        // #######################
        // 1º Devolve a Lista de produtos 
        [HttpGet]
        public IActionResult AddProductToList(int id)
        {

            // IEnumerable<Product> products = _context
            var products = _context
                                      .Products
                                      .Include(c => c.Category)
                                      .ToList();  // para ir à base de dados usar "_XXXXX"
                                                  // return View("../Home/Index");

            ViewBag.Quantity = 0;
            ViewBag.OrderID = id;
            ViewBag.AllProductsList = products;
            return View(products);
            // return View();
        }

        // ###################
        // Aqui vou adicionar o produto à venda na qtd escrita no campo
        // mas nao saio da mesma vista qd primo botao ADD
        // #################
        [HttpPost]
        public IActionResult AddProductToList(Product product, int id) //devolve ID do Produto a adicionar
        {

           


            if (ModelState.IsValid)
            {

                ProductList? productList = _context.ProductLists.Find(id);
                
                //_context.ProductLists.Add(productList);
                // Procurar preço com ProdutoID
                // Dados do produto em Stock
                Product? productx = _context.Products.Find(ViewBag.OrderID);  //  productList.ProductID
                ViewBag.Price = productx.Price;

                // Quantidade é igual ao valor da celula
                productList.Quantity=0;

                ViewBag.Quantity = productList.Quantity;

                // Atualizar quantidade no Stock
                // O stock é so atualizado no Package product.Quantity = product.Quantity - ViewBag.Quantity;

                // Vai buscar Classe Sale
                // Sale? sale = _context.Sales.Find(productList.SaleID);
                // ViewBag.Sale = sale.ID;
                // sale.Observations = " ";

                // Vai buscar Classe Sale
                // Customer? customer = _context.Customers.Find(sale.CustomerID);
                // ViewBag.Customer = customer.ID;

                // Carrega valores automaticos , preço e cod de venda
                productList.Price = ViewBag.Price;    // =0;  ViewBag.ClienteID # Esta a funcionar OK
                //productList.SaleID = ViewBag.Saleid; // Novo teste 18-06
                // productList.SaleID= 3;
                productList.SaleID = ViewBag.Sale;

                // Atualiza TotalPrice da Encomenda
                //ViewBag.TotalPrice = sale.TotalPrice;
                //sale.TotalPrice = ViewBag.TotalPrice + (ViewBag.Price * ViewBag.Quantity);

                Category? category = _context.Categories.Find(product.CategoryID);
                ViewBag.Category = category.ID;

                // Coloca a Zero para poder criar Novo
                productList.ID = 0;
                //_context.ProductLists.Update(productList);  // update ???
                _context.ProductLists.Add(productList);
                _context.SaveChanges();     // tens aqui varios pedido agora grava
                // ViewBag.CodVenda=productList.SaleID;

                
         

                return RedirectToAction(nameof(Index)); //Index
            }

            

            // Envia Listas  para a vista
            // this.SetupProductList();

            //return View(productList);
        



           

            return View(ViewBag.AllProductsList);
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
        // este metodo transporta o SaleID
        [Authorize(Policy = POLICIES.APP_POLICY_VENDOR.NAME)]  //  so o admin e vendedor é que poode aceder
        [HttpGet]
        public IActionResult AddProductToOrder(int id)  // (int id)
        {

            ViewBag.Saleid = id;



            // envia p vista lista Produtos e vendas
            this.SetupProductList();
            return View();
        }



        [Authorize(Policy = POLICIES.APP_POLICY_VENDOR.NAME)]  //  so o admin e vendedor é que poode aceder
        [HttpPost]
        public IActionResult AddProductToOrder(ProductList productList)
        {



            if (ModelState.IsValid)
            {

                //_context.ProductLists.Add(productList);
                // Procurar preço com ProdutoID
                Product? product = _context.Products.Find(productList.ProductID);
                ViewBag.Price = product.Price; 
                ViewBag.Quantity = productList.Quantity;

                // Atualizar quantidade no Stock
                // O stock é so atualizado no Package product.Quantity = product.Quantity - ViewBag.Quantity;

                // Vai buscar Classe Sale
                Sale? sale = _context.Sales.Find(productList.SaleID);
                ViewBag.Sale = sale.ID;
                sale.Observations = " ";

                // Vai buscar Classe Sale
                Customer? customer = _context.Customers.Find(sale.CustomerID);
                ViewBag.Customer = customer.ID;

                // Carrega valores automaticos , preço e cod de venda
                productList.Price= ViewBag.Price;    // =0;  ViewBag.ClienteID # Esta a funcionar OK
                //productList.SaleID = ViewBag.Saleid; // Novo teste 18-06
                // productList.SaleID= 3;
                productList.SaleID= ViewBag.Sale;

                // Atualiza TotalPrice da Encomenda
                ViewBag.TotalPrice = sale.TotalPrice;
                sale.TotalPrice = ViewBag.TotalPrice + (ViewBag.Price * ViewBag.Quantity);

                Category? category = _context.Categories.Find(product.CategoryID);
                ViewBag.Category = category.ID;

                // Coloca a Zero para poder criar Novo
                productList.ID = 0;  
                //_context.ProductLists.Update(productList);  // update ???
                _context.ProductLists.Add(productList);
                _context.SaveChanges();     // tens aqui varios pedido agora grava
                // ViewBag.CodVenda=productList.SaleID;

                // Toastr.SucessMessage tem de aparecer msg quando criar um novo
                _toastNotification.AddSuccessToastMessage("Product sucessfully Added to Order.");

                // Aqui passar com Details/{0},id ... parecido com isto 
                //return RedirectToAction("Index");  //  ("Details/{0}", ViewBag.CodVenda
                // return RedirectToAction(nameof(index)); -> outra forma de apresentar igual


                // envia p vista lista Produtos e vendas
                // this.SetupProductList();
                // Quero manter-me na mesma Vista
                // return View();   "Index"
                return RedirectToAction(nameof(Index)); //Index
            }

            // Toastr.ERRORMessage aparecer msg em caso de falha 
            _toastNotification.AddErrorToastMessage("Error - Product not Added to Order.");

            // Envia Listas  para a vista
            this.SetupProductList();

            return View(productList);
        }


        // ##############################
        //  Metodos do ProductList
        // ##############################
        public IActionResult OrderList()
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

        // Seleciona cod Venda e mostra produtos dessa venda
        // ###################################################      
        //      SetCodVenda
        public IActionResult SetCodVenda(int id)
        {
            // Procura Cod de venda a partir do indice da Lista de produto
            ProductList? productList = _context.ProductLists.Find(id);
            ViewBag.VendaID = productList.SaleID;
            // Category? category = _context.Categories.Find(id);
            // ViewBag.CategoryName = category.Name;

            // retorna lista com CodVenda como parametro de entrada
            var produtListBySetCodVenda = _context
                                                .ProductLists
                                                .Include(p => p.Sale)
                                                .Include(p => p.Sale.Customer)
                                                .Include(p => p.Product)
                                                .Include(p => p.Product.Category)
                                                .Where(p => p.SaleID == id)
                                                .ToList();



            return View(produtListBySetCodVenda);

        }

        // #############################
        // Create L 
        // Adiciona produtos à ordem de venda...
        [HttpGet]
        public IActionResult CreateL()
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
        public IActionResult CreateL(ProductList productList)
        {



            if (ModelState.IsValid)
            {

                _context.ProductLists.Add(productList);
                _context.SaveChanges();     // tens aqui varios pedido agora grava

                // Toastr.SucessMessage tem de aparecer msg quando criar um novo
                _toastNotification.AddSuccessToastMessage("Product sucessfully Added to Order.");

                // return RedirectToAction("Index");
                // return RedirectToAction(nameof(index)); -> outra forma de apresentar igual

                // Teste
                return View(productList);
            }

            // Toastr.ERRORMessage aparecer msg em caso de falha 
            _toastNotification.AddErrorToastMessage("Error - Product not Added to Order.");

            //ViewBag.SaleList = new SelectList(_context.Sales, "ID", "CodVenda");
            // Passar List Products para a view
            //ViewBag.ProductList = new SelectList(_context.ProductLists, "ID", "Description");
            return View(productList);
        }


        // ###################################################
        //
        //      EditL
        //
        // #################################################
        // Adiciona produtos à ordem de venda...
        [HttpGet]
        public IActionResult EditL(int id)
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
        public IActionResult EditL(ProductList productList)
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


        // #################################
        //  TotalEquity
        // #########################
        [Authorize(Policy = POLICIES.APP_POLICY_ADMIN.NAME)]
        public IActionResult TotalEquity()
        {

            // retorna lista de total vendas
            var AllSales = _context
                                               .Sales
                                               .Include(s => s.Customer)
                                               .ToList();  // para ir à base de dados usar "_XXXXX"




            var customerPurchasesProductsIsPaid2 = _context
                                               .Sales
                                               .Where(p => p.IsPaid == true)
                                               .ToList();

            var customerPurchasesProducts2 = _context
                                                .Sales
                                                .ToList();

            
            // Soma de todas as Vendas/encomenda (pagas e nao pagas) 
            var TotalSumAll = customerPurchasesProducts2.Sum(p => p.TotalPrice);  // p.Quantity * p.Price

            // Soma das vendas/encomenda pagas
            var TotalSumIsPaid = customerPurchasesProductsIsPaid2.Sum(p => p.TotalPrice);  //p.Quantity * p.Price

            // Procura Cod de venda a partir do indice da Lista de produto
            ViewBag.TotalEquity =  Math.Round(TotalSumAll - TotalSumIsPaid, 2);

            return View(AllSales);

        }

        // ###########################
        //  Order Process
        // ########################
        // 1- Colocar a venda em processamento,
        // 2- Verificar a existencia do produto (corrigir a subtracao da qtd na criacao da venda), colocar obs se nao existir e abater preço
        // 3- depois de validada, passa encomenda a processada, ver como fazer isto
        public async Task<IActionResult> OrderProcess(ProductList productList, int id)  // (ProductList productList, int id)
        {
            Sale? sale = await _context.Sales.FindAsync(id);

            //  sale == null || sale.Status!=Status.Package && (sale.Status == Status.Ordered || sale.Status == Status.Purchase_Process)
            if (sale != null && (sale.Status == Status.Ordered))
            {
                // identificacao das variaveis venda e customer
                ViewBag.SaleID = id;
                ViewBag.CustomerID = sale.CustomerID;
                ViewBag.NumVenda = sale.CodVenda;
                // inicia var
                ViewBag.Obs = sale.Observations;
                ViewBag.SoldOut = false;

                // Verifica existencia em stock e caso não exista, adiciono comentario nas observacoes
                // criar uma string, para incluir todos os produtos
                // Retorna a Lista de produtos pelo ID venda 
                var produtListBySetCodVenda = _context
                                        .ProductLists
                                        .Include(p => p.Sale)
                                        .Include(p => p.Sale.Customer)
                                        .Include(p => p.Product)
                                        .Include(p => p.Product.Category)
                                        .Where(p => p.SaleID == id)
                                        .ToList();


                // Verifca existencia em stock e regista em caso de falta 
                foreach (var p in produtListBySetCodVenda)
                {
                    // Atualizar Status
                    p.Sale.Status = Status.Purchase_Process;

                    //p.Product.Quantity = p.Product.Quantity - p.Quantity;
                    if (p.Product.Quantity < p.Quantity)
                    {
                        p.Price = 0;
                        ViewBag.Obs = "Attention : " + p.Product.Description + " near Sold Out. " + ViewBag.Obs;
                        // + "Produto, {0} com rutura de stock", p.Product.Description.ToString();
                        ViewBag.SoldOut = true;
                    }

                    // A informacao mais antiga fica no fim
                    p.Sale.Observations = ViewBag.Obs;



                }
                if (ViewBag.Soldout)
                {
                    // Toastr.ERRORMessage aparecer msg em caso de falha 
                    _toastNotification.AddErrorToastMessage("Sold Out.");

                }


                // Toastr.SucessMessage tem de aparecer msg quando criar um novo
                _toastNotification.AddSuccessToastMessage("Order sucessfully updated.");



                // atualiza DBase
                _context.Sales.Update(sale);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");


            }

            // Toastr.ERRORMessage aparecer msg em caso de falha 
            _toastNotification.AddErrorToastMessage("Error - Order not updated.");

            //return NotFound();
            return View();


        }
        // ###########################
        //  Validation to Package
        // ########################           
        // Verificado pelo operador e passa para embalado
        public async Task<IActionResult> PackageOrder(ProductList productList, int id)  // (ProductList productList, int id)
        {
            Sale? sale = await _context.Sales.FindAsync(id);

            //  sale == null || sale.Status!=Status.Package
            if (sale != null && sale.Status == Status.Purchase_Process)
            {
                // identificacao das variaveis venda e customer
                ViewBag.SaleID = id;
                ViewBag.CustomerID = sale.CustomerID;
                ViewBag.NumVenda = sale.CodVenda;

                // passa para Pachage (embalado) por accao do operador
                sale.Status = Status.Package;


                // atualiza DBase
                _context.Sales.Update(sale);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            // Toastr.ERRORMessage aparecer msg em caso de falha 
            _toastNotification.AddErrorToastMessage("Error - Order not updated.");

            //return NotFound();
            return View();


        }



            // ##########################
            // Sent Order
            // ##########################
            // 1 - O operador vai buscar os produtos da encomenda
            // 2 - os stock são atualizados, só aqui é que o stock é subtraido
            // 3 - a encomenda é empacotada e enviada para empresa de transportes
            // 4 - è enviado email cliente




            public async Task<IActionResult> SentOrder(ProductList productList, int id)  // (ProductList productList, int id)
            {


            Sale? sale = await _context.Sales.FindAsync(id);

                 //  sale == null || sale.Status!=Status.Package
                if (sale != null && sale.Status == Status.Package)  
                {
                    // identificacao das variaveis venda e customer
                    ViewBag.SaleID = id;
                    ViewBag.CustomerID = sale.CustomerID;
                    ViewBag.NumVenda = sale.CodVenda;

                    // Atualiza Stock, tenho de correr cada um dos produtos da encomenda e atualizar stock
                    //sale.TotalPrice = sale.TotalPrice + (productList.Price * productList.Quantity);
                    // Retorna a Lista de produtos pelo ID venda 
                    var produtListBySetCodVenda = _context
                                        .ProductLists
                                        .Include(p => p.Sale)
                                        .Include(p => p.Sale.Customer)
                                        .Include(p => p.Product)
                                        .Include(p => p.Product.Category)
                                        .Where(p => p.SaleID == id)
                                        .ToList();

                    // Atualiza quantidades em stock
                    foreach (var p in produtListBySetCodVenda)
                    {
                        p.Product.Quantity = p.Product.Quantity - p.Quantity;
                    }

                    // var TotalSumAll = produtListBySetCodVenda.Sum(p => p.TotalPrice);

                    // Passa Venda para Sent (Apenas se está em Package, confirmado no inicio)
                    sale.Status = Status.Sent;
                    // atualiza DBase
                    _context.Sales.Update(sale);
                    await _context.SaveChangesAsync();     // tens aqui varios pedido agora grava

                    // Envia email para cliente
                    Customer? customer = await _context.Customers.FindAsync(sale.CustomerID);
                    ViewBag.CustomerName = customer.Name;
                    ViewBag.CustomerEmail = customer.Email;

                    // como obter a lingua selecionada na view do Browser do cliente :
                    var culture = Thread.CurrentThread.CurrentUICulture;


                    // agora temos de carregar o template do email, que não é mais do que uma string enorme, a raiz da dir é CursoMod165
                    // vai á diretoria do projecto "EmailTemplates"
                    string template = System.IO.File.ReadAllText(
                    Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "EmailTemplates",
                        $"sent_order.{culture.Name}.html"
                        )
                    );


                    StringBuilder htmlBody = new StringBuilder(template);
                    htmlBody.Replace("##CUSTOMER_NAME##", customer.Name);
                    htmlBody.Replace("##COD_ORDER##", sale.CodVenda);
                    htmlBody.Replace("##TOTAL_PRICE##", sale.TotalPrice.ToString());  // sale.TotalPrice


                    //Aqui enviamos um email ...
                    // (customer.Email,
                    // Obtem email do cliente no regisot de cliente do mesmo
                    _emailSender.SendEmailAsync(ViewBag.CustomerEmail, "Sent Order",
                        htmlBody.ToString());



                    // Toastr.SucessMessage tem de aparecer msg quando criar um novo
                    _toastNotification.AddSuccessToastMessage("Order sucessfully updated.");

                    // Mostra Toast a informar que enviou emails
                    _toastNotification.AddSuccessToastMessage($" {sale.CodVenda} Emails sucessfully sent.");

                    return RedirectToAction("Index");

                }


                // Toastr.ERRORMessage aparecer msg em caso de falha 
                _toastNotification.AddErrorToastMessage("Error - Order not updated.");

                //return NotFound();
                return View();


            }


        // ###########################
        // BOTAO - example-Teste async   if (ModelState.IsValid) / if (product == null)
        // #########################
        
        //public async Task<IActionResult> botao(ProductList productList,int id)
        //{
        //
        //
        //    Sale? sale = await _context.Sales.FindAsync(id);
            

         //   if (sale == null)
         //   {
         //       return View(); 
         //   }

            // teste muda estado encomenda para sent
         //   ViewBag.SaleID = id;
         //   sale.Status = Status.Sent;
            //sale.TotalPrice = sale.TotalPrice + (productList.Price * productList.Quantity);

            // atualiza DBase
         //   _context.Sales.Update(sale);
         //   await _context.SaveChangesAsync();     // tens aqui varios pedido agora grava
            

         //   return RedirectToAction("Index"); // tenho de retornar uma vista ...
            // return RedirectToAction(nameof(index)); -> outra forma de apresentar igual


        // }

        // ##########################
        // Create do Basket (Botao)
        // ###################

        // #############################
        // Create 
        // Adiciona produtos à ordem de venda...
        [HttpGet]
        public IActionResult Botao()
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



            // NOVO Envia Dbase par a avista
            var productLists = _context
                                                .ProductLists
                                                .Include(p => p.Sale)
                                                .Include(p => p.Sale.Customer)
                                                .Include(p => p.Product)
                                                .Include(p => p.Product.Category)
                                                .OrderBy(p => p.SaleID)
                                                .ToList();  // para ir à base de dados usar "_XXXXX"



            // Teste codigo :






            // Teste cofigo - FIM

            //return View(productLists);

            // Fim NOVO

            // ViewBag.Categories = new SelectList(_context.Categories, "ID", "Name");
            return View();  
        }

        [HttpPost]
        public async Task<IActionResult> Botao(ProductList productList)
        {

           

            if (ModelState.IsValid)
            {

                // Ler preço do produto escolhido
                // productList.Price = 111;
                
                Product? product = await _context.Products.FindAsync(productList.ProductID);
                productList.Price = product.Price;


                // Atualizar quantidades em stock; colocar if (product.Quantity>=productList.Quantity)
                // isto nao é feito aqui so qd for para embalar ...
                // product.Quantity = product.Quantity - productList.Quantity;

                //  Atualizar Valor total da encomenda
                Sale? sale = await _context.Sales.FindAsync(productList.SaleID);
                sale.TotalPrice = sale.TotalPrice + (productList.Price * productList.Quantity);


                productList.ID = 0;  // Ver o que dá ...o ID devia aparecer a Zero e aparece com valor 
                _context.ProductLists.Add(productList);
                await _context.SaveChangesAsync();     // tens aqui varios pedido agora grava

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





        // ########################
        // Fim - Create do Basket (botao)
        // ######################




        // ##################################
        // Funcoes diversas aqui :
        // #############################
               
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



        // #########################
        //    Teste metod
        // ######################



        public IActionResult ButtonClick()
        {
            return View();
        }
        public IActionResult check(string button)
        {
            if (!string.IsNullOrEmpty(button))
            {
                TempData["ButtonValue"] = string.Format("{0} button clicked.", button);
            }
            else
            {
                TempData["ButtonValue"] = "No button click!";
            }
            return RedirectToAction("Botao");  // "ButtonClick" 
        }





    }

}





