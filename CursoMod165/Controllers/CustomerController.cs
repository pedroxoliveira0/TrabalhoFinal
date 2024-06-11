using CursoMod165.Data; 
using CursoMod165.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json.Linq;
using static CursoMod165.CursoMod165Constants;

namespace CursoMod165.Controllers
{

    // [Authorize]  // para activar o login nesta vista
    // [Authorize(Roles = "ADMINISTRATIVE;ADMIN")]
    [Authorize(Policy = POLICIES.APP_POLICY.NAME)]
    public class CustomerController : Controller
    {
        
        private readonly ApplicationDbContext _context;  // premir com botão direito a lampada para tnasformar var _context

        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            IEnumerable<Customer> customers= _context.Customers.ToList();  // para ir à base de dados usar "_XXXXX"
            // return View("../Home/Index");
            return View(customers);  // tenho de colocar aqui a tabela de base de dados se não dá erro por retornar Null
        }

        // para poder ser executado no google  usar https:// 
        //** https:// localhost:8000/Customer/Create
        // para ciriar um view tenho de criar uma pasta com o meu metodo create -> logo vista create
        [HttpGet]
        public IActionResult Create() 
        {
            return View();

        }
        // 
        [HttpPost]
        public IActionResult Create(Customer customer) 
        {
            if (ModelState.IsValid)
            {
                // TO Do Criar novo customer caso contrario; return view customer com dados anteriores
                _context.Customers.Add(customer);
                _context.SaveChanges();     // tens aqui varios pedido agora grava

                return RedirectToAction("Index");

            }

            // Esta linha não esta julgo no projeto
            // return RedirectToAction("Index");
            return View(customer);
        }

        // porque este não tem get ou post ???? R. Pk nao vai recolher dados
        // acção details
        public IActionResult Details(int id) 
        {
            // retorna os dados da chave que entra aqui e vem da tabela
            // usa-se ? porque pode não encontrar o dado e não pode ser null, quando colocamos o ? quer dizer que pode existir ou nao existir
            // o ? permite validar a entrada não deixando entrar erros
            Customer? customer = _context.Customers.Find(id);  // Chave primaria = id  

            if (customer == null) // se for diferente de null faz a vista
            {
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // Edit NUm -> não sei se é para colocar isto
        public IActionResult EditNum(int num)
        {
            // retorna os dados da chave que entra aqui e vem da tabela
            // usa-se ? porque pode não encontrar o dado e não pode ser null, quando colocamos o ? quer dizer que pode existir ou nao existir
            // o ? permite validar a entrada não deixando entrar erros
            Customer? customer = _context.Customers.Find(num);  // Chave primaria = id  

            if (customer == null) // se for diferente de null faz a vista
            {
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }


        // Fim Edit Num

        // nOVO EDIT
        [HttpGet] // obtem dados da base de  dados, neste caso a partir do padrao
        public IActionResult Edit(int id)
        {
            // retorna os dados da chave que entra aqui e vem da tabela
            // usa-se ? porque pode não encontrar o dado e não pode ser null, quando colocamos o ? quer dizer que pode existir ou nao existir
            Customer? customer = _context.Customers.Find(id);  // Chave primaria = id

            if (customer == null) // se for diferente de null faz a vista
            {
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        [HttpPost]   // envia dados para a base de dados
        public IActionResult Edit(Customer customer) 
        {
            if (ModelState.IsValid) 
            { 
                _context.Customers.Update(customer);        // atualiza
                _context.SaveChanges();                     // grava
                return RedirectToAction(nameof(Index));   // volta para a pagina principal, para o cliente saber que gravou, mostra lista
            }
            
            return View();
        }

        [HttpGet] //METODO QUE VAI DEVOLVER A VISTA DE DELETE 
        [Authorize(Policy = POLICIES.APP_POLICY_ADMIN.NAME)]  //  so o admin é que poode aceder ao delete, para evitar que pelo lado da vista se chame a pagina delete por URL
        public IActionResult Delete(int id) 
        { 
            if (id == null) 
            {
                return NotFound();   
            }

            Customer? customer = _context.Customers.Find(id);

            if (customer == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(customer); 
        }


        // para apagar o customer só preciso do ID não preciso dos dados do customer
        // aqui a vista mantem o nome delete, mas a acção é delete confirm
        // Testar isto : http s://localhost:8000/Customer/Delete/1
        [HttpPost, ActionName("Delete")]
        [Authorize(Policy = POLICIES.APP_POLICY_ADMIN.NAME)]  //  delete so p/ admin , no lado do controlador para evitar que pelo lado da vista se chame a pagina delete por URL
        public IActionResult DeleteConfirmed(int? id) 
        {
            if (id == null)
            {
                return NotFound();
            }

            Customer? customer = _context.Customers.Find(id);

            if (customer !=null)
            {
                
                _context.Customers.Remove(customer);        // atualiza
                _context.SaveChanges();                     // grava
                return RedirectToAction(nameof(Index));
            }

            return View(customer);
        }
    }
}





//
//< div class= "form-group" >
//    < input type = "submit" value = "Save" class= "btn btn-primary" />
//</ div >