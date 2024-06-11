using CursoMod165.Data;
using CursoMod165.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace CursoMod165.Controllers
{
	public class ProductController : Controller
	{

		// aqui é definida a ligação para a base de dados
		private readonly ApplicationDbContext _context;  // premir com botão direito a lampada para tnasformar var _context

		// definicao de variaveis 
		private readonly IToastNotification _toastNotification; // Singleton -> ou seja tudo é igual em todo o lado as mesmas regras

		// faz ligacao à base de dados - original
		//public CategoryController(ApplicationDbContext context)
		//{
		//    _context = context;
		//}

		// aqui defino var Toastr e ligacao à base de dados
		// IToastNotification toastNotification)
		public ProductController(ApplicationDbContext context,
			IToastNotification toastNotification)

		{
			// ligação à base de dados; emulação da base de dados
			_context = context;
			_toastNotification = toastNotification;

		}



		// retorna dados base de dados para a view index
		// aqui tenho de associar a tabela categorias à lista/tabela de produtos
		// .include(category => category.Category)
		public IActionResult Index()
		{
			IEnumerable<Product> products = _context
												.Products
												.Include(c => c.Category)
												.ToList();  // para ir à base de dados usar "_XXXXX"
																			  // return View("../Home/Index");
			return View(products);  // tenho de colocar aqui a tabela de base de dados se não dá erro por retornar Null


            


        }


        // ##########################################
        //                CREATE
        // ##########################################
        // apresenta formuladrio para preencher
        [HttpGet]
        public IActionResult Create()
        {

            // aqui passamos dados entre tabela Staffroles e Staff
            // ViewBag.StaffRoles = new SelectList(_context.StaffRoles, "ID", "Name");
            // ViewBag.MedicStaffRoleID = _context.StaffRoles.First(sr => sr.Name == "Médico").ID;

            // ViewBag.Categories = new SelectList(_context.Categories, "ID", "Name", product.categoryID); direto, nao se pode fazer pk n temos id do produto ainda
            // ViewBag.Categories = new SelectList(_context.Categories, "ID", "Name");
            // foi criado este metodo e assim tenho de tirar as linhas de codigo em cima que já estão no metodo
            this.SetupProductModel();

            return View();
            // return View();
        }


        // envia dados preenchidos no formulario
        [HttpPost]
        public IActionResult Create(Product product)
        {
            // ver exemplo Appointment, rever aula
            // com os dados preenchidos no formlario Create Staff, criar registo na Dbase
            if (ModelState.IsValid)
            {
                // Se retora falso algo está mal preenchido, deve aparecer uma info e nao sair da pagina
                // TO Do Criar novo Staff caso contrario;
                // return view customer com dados anteriores
                _context.Products.Add(product);
                _context.SaveChanges();     // tens aqui varios pedido agora grava

                return RedirectToAction("Index");
                // return RedirectToAction(nameof(index)); -> outra forma de apresentar igual

            }
            // Vai buscar os dados à vista que foram preenchidos pelo utilizador
            // Copiado do IAction Create() ....
            // ViewBag.StaffRoles = new SelectList(_context.StaffRoles, "ID", "Name");
            // temos de definir a variavel que vai passar entre controlador e form create
            // ViewBag.MedicStaffRoleID = _context.StaffRoles.First(sr => sr.Name == "Médico").ID;  // a palavra tem de ser igual na dBase a Médico

            // foi criado este metodo e assim tenho de tirar as linhas de codigo em cima que já estão no metodo
            this.SetupProductModel();   // this.SetupStaffModel()

            // Apresenta a Vista do product index

            return View(product);
        }


        // #################################
        //              EDIT 
        // #################################
        // nOVO EDIT
        [HttpGet] // obtem dados da base de  dados, neste caso a partir do padrao
        public IActionResult Edit(int id)
        {
            ViewBag.Titulo2 = "9 de Junho de 2024";


            //ViewBag["CategoryList"] = new SelectList(_context.Customers, "ID", "Name");
            ViewBag.CategoryList = new SelectList(_context.Categories, "ID", "Name");
            //ViewBag.CategoryList = new SelectList(_context.Customers, "ID", "Name", "Description");

            //ViewData["Enunciado"] = Enunciado;


            //ViewBag["testID"] = new SelectList (_context.Products,"ID","Name");

            // retorna os dados da chave que entra aqui e vem da tabela
            // usa-se ? porque pode não encontrar o dado e não pode ser null, quando colocamos o ? quer dizer que pode existir ou nao existir
            Product? product = _context.Products.Find(id);  // Chave primaria = id


            if (product == null) // se for diferente de null faz a vista
            {
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        [HttpPost]   // envia dados para a base de dados
        public IActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Update(product);        // atualiza
                _context.SaveChanges();                     // grava

                // Toastr.SucessMessage tem de aparecer msg quando criar um novo
                _toastNotification.AddSuccessToastMessage("Product sucessfully updated.");

                return RedirectToAction(nameof(Index));   // volta para a pagina principal, para o cliente saber que gravou, mostra lista
            }

            // Toastr.ERRORMessage aparecer msg em caso de falha 
            _toastNotification.AddErrorToastMessage("Error - Product not updated.");
            return View();
        }

        // ####################################### end EDIT


        // Funcao complementar, para troca de dados entre vista e controlador
        // a ideia é retornar duas Listas (com ID e Nome), uma das categorias e outra do sistema de unidades,
        // que vão estar diponiveis nos campos categoria e unity a preencher

        private void SetupProductModel()
        {
            // aqui passamos dados entre tabela Staffroles e Staff
            // acesso a tabelas para obter valores ??!! era: ViewBag.Categories
            //       ViewBag.CategoriesList = new SelectList(_context.Categories, "ID", "Name");
            //ViewBag.MedicStaffRoleID = _context.StaffRoles.First(sr => sr.Name == "Médico").ID;


            // aqui passamos dados entre tabela Staffroles e Staff
            ViewBag.Categories = new SelectList(_context.Categories, "ID", "Name");
            // ViewBag.FrutoCategoriesID = _context.Categories.First(c => c.Name == "Fruto").ID;


            // Aqui passamos a lista de Categorias para a celula da view
            //var categoryList = _context.Products
            //                           .Include(c => c.ID)   // s são todos os elementos que estão no dB set
            //                           .Select(c => new {
            //                                            ID = c.ID,
            //                                            Name = $"{c.Name}"
            //                                            });
            //
            //ViewBag.CategoryList = new SelectList(categoryList, "ID", "Name");




            // codigo APP Begin
            //   ViewBag.CustomerList = new SelectList(_context.Customers, "ID", "Name");

            // uma possibilidade o role ser condicionado só tem acesso que nos queremos
            //_context.Staffs.Where(s => s.StaffRoleID != 1);     // esta solução é provisoria por nao ser escalável


            //    esta é a solução correta ver staff roles models
            //   var staffList = _context.Staffs
            //                           .Include(s => s.StaffRole)   // s são todos os elementos que estão no dB set
            //                           .Where(s => s.StaffRole.CanDoAppointments == true)
            //                           .Select(s => new {
            // coloca role à frente do nome na lista
            //                               ID = s.ID,
            //                               Name = $"{s.Name} [{s.StaffRole.Name}]"
            //                           });

            //   ViewBag.StaffList = new SelectList(staffList, "ID", "Name");

            // codigo APP FIm

        }


    }
}
