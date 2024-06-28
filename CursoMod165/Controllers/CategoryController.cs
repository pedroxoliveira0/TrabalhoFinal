using CursoMod165.Data;
using CursoMod165.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using NToastNotify;   // para poder usar os toastr notifications
using System.Text;

namespace CursoMod165.Controllers
{
    // só os utilizadores logados em acesso ao menu 
    // (esta a dar erro:) [Authorize(Policy = POLICIES.APP_POLICY.NAME)]  // para activar o login nesta vista

    public class CategoryController : Controller
	{
		
		// aqui é definida a ligação para a base de dados
		private readonly ApplicationDbContext _context;  // premir com botão direito a lampada para tnasformar var _context

        // definicao de variaveis 
        private readonly IToastNotification _toastNotification; // Singleton -> ou seja tudo é igual em todo o lado as mesmas regras
        private readonly IHtmlLocalizer<Resource> _sharedLocalizer;
        private readonly IStringLocalizer<Resource> _localizer;
        // faz ligacao à base de dados - original
        //public CategoryController(ApplicationDbContext context)
        //{
        //    _context = context;
        //}

        // aqui defino var Toastr e ligacao à base de dados
        // IToastNotification toastNotification)
        public CategoryController(ApplicationDbContext context,
            IToastNotification toastNotification,
             IHtmlLocalizer<Resource> sharedLocalizer,
            IStringLocalizer<Resource> localizer)

        {
            // ligação à base de dados; emulação da base de dados
            _context = context;
            _toastNotification = toastNotification;
            _sharedLocalizer = sharedLocalizer;
            _localizer = localizer;

        }



       


        // retorna dados base de dados para a view index
        public IActionResult Index()
        {
            IEnumerable<Category> categories = _context.Categories.ToList();  // para ir à base de dados usar "_XXXXX"
            // return View("../Home/Index");
            return View(categories);  // tenho de colocar aqui a tabela de base de dados se não dá erro por retornar Null
        }


        public IActionResult ProductsByCategory(int id)
        {
            Category? category = _context.Categories.Find(id);
            ViewBag.CategoryName=category.Name;
            
            var productsbycategory = _context
                                                .Products
                                                .Include(c => c.Category)
                                                .Where(c => c.Category.ID == id)    
                                                .ToList();  // para ir à base de dados usar "_XXXXX"
                                                            // return View("../Home/Index");

             
            return View(productsbycategory);  // tenho de colocar aqui a tabela de base de dados se não dá erro por retornar Null

        }


        // Http Get - para criar novos registos
        [HttpGet]
		public IActionResult Create()
		{
			// Teste como criar um novo indice de dados:
            // Category Nova_Categoria = new Category();
            // Nova_Categoria.Name = "Especiarias";
            // Nova_Categoria.Description = "Cat. do tipo Especiarias";


            return View();
		}

        
        // Http post - com ligacao à Dbase Category, ou seja retorna para a Dbase category
        // o posto publica ou seja recebe os valores da vista e envia para a base de dados
        [HttpPost]
        public IActionResult Create(Category category)
        {
            
            // aqui vou obter ou ler os campos preenchidos na minha pagina view e passa-los para a base de dados
            // botao right set using ...

            if (ModelState.IsValid)
            {
                // TO Do Criar novo customer caso contrario; return view customer com dados anteriores
                _context.Categories.Add(category);
                _context.SaveChanges();     // tens aqui varios pedido agora grava

                // Toastr.SucessMessage tem de aparecer msg quando criar um novo
                _toastNotification.AddSuccessToastMessage("Category sucessfully created.");
                
                // na saida volto à pagina principal
                // aqui recebo os dados da pagina (view) que depois são enviados para a base de dados
                return RedirectToAction("Index");

            }

            // Toastr.ERRORMessage aparecer msg em caso de falha 
            // _toastNotification.AddErrorToastMessage("Error - Category not created.");
            // Ver diferenca entre category e "Index"
            // Toastr.ErrorMessage - Novo 27-06
		    _toastNotification.AddErrorToastMessage("Check the form again!",
    		new ToastrOptions { 
        	Title = "Appointment Creation Error",
        	TapToDismiss = true,
        	TimeOut = 0
    		});


	    return View(category);
            
        }


        
        // #################################
        //              EDIT 
        // #################################
        // nOVO EDIT
        [HttpGet] // obtem dados da base de  dados, neste caso a partir do padrao
        public IActionResult Edit(int id)
        {
            // int numInt = int.Parse(num);
		// retorna os dados da chave que entra aqui e vem da tabela
            // usa-se ? porque pode não encontrar o dado e não pode ser null, quando colocamos o ? quer dizer que pode existir ou nao existir
            Category? category = _context.Categories.Find(id);  // Chave primaria = id

            if (category == null) // se for diferente de null faz a vista
            {
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [HttpPost]   // envia dados para a base de dados
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Update(category);        // atualiza
                _context.SaveChanges();                     // grava

                // Toastr.SucessMessage tem de aparecer msg quando criar um novo
                //_toastNotification.AddSuccessToastMessage("Category sucessfully updated.");

                // Nova MSG 27-06
                string message =
                    string.Format(_sharedLocalizer["<b>Category {0}</b> successfully edited!"].Value,
                                  category.Name);


                _toastNotification.AddSuccessToastMessage(message, 
    				new ToastrOptions { Title = _sharedLocalizer["Success"].Value,
        			TimeOut = 0,
        			TapToDismiss = true
    				});

				return RedirectToAction(nameof(Index));   // volta para a pagina principal, para o cliente saber que gravou, mostra lista
            }

			// Toastr.ERRORMessage aparecer msg em caso de falha 
			_toastNotification.AddErrorToastMessage("Error - Category not updated.");
			return View();
        }

        // #################################
        //              DELETE
        // #################################
        [HttpGet] //METODO QUE VAI DEVOLVER A VISTA DE DELETE 
        //[Authorize(Policy = POLICIES.APP_POLICY_ADMIN.NAME)]  //  so o admin é que poode aceder ao delete, para evitar que pelo lado da vista se chame a pagina delete por URL
        public IActionResult Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Category? category = _context.Categories.Find(id);

            if (category == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        // para apagar o customer só preciso do ID não preciso dos dados do customer
        // aqui a vista mantem o nome delete, mas a acção é delete confirm
        // Testar isto : http s://localhost:8000/Customer/Delete/1
        //[HttpPost, ActionName("Delete")]
        //[Authorize(Policy = POLICIES.APP_POLICY_ADMIN.NAME)]  //  delete so p/ admin , no lado do controlador para evitar que pelo lado da vista se chame a pagina delete por URL
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Category? category = _context.Categories.Find(id);

            if (category != null)
            {

                _context.Categories.Remove(category);        // atualiza
                _context.SaveChanges();                     // grava
				
                // Toastr.SucessMessage tem de aparecer msg quando criar um novo
				//_toastNotification.AddSuccessToastMessage("Category sucessfully deleted.");
                // Nova MSG 27-06
                string message1 =
                    string.Format(_sharedLocalizer["<b>Category</b> Deleted"].Value,
                                  category.Name);


                _toastNotification.AddSuccessToastMessage(message1,
                    new ToastrOptions
                    {
                        Title = _sharedLocalizer["Success"].Value,
                        TimeOut = 0,
                        TapToDismiss = true
                    });


                return RedirectToAction(nameof(Index));
            }

			// Toastr.ERRORMessage aparecer msg em caso de falha 
			// _toastNotification.AddErrorToastMessage("Error - Category not deleted.");
            // Nova MSG 27-06
            string message2 =
                string.Format(_sharedLocalizer["<b>Category</b> Not Deleted"].Value,
                              category.Name);


            _toastNotification.AddErrorToastMessage(message2,
                new ToastrOptions
                {
                    Title = _sharedLocalizer["Error"].Value,
                    TimeOut = 0,
                    TapToDismiss = true
                });


            return View(category);
        }



        

    }
}
