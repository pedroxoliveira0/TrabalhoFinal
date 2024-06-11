using Microsoft.AspNetCore.Mvc;

// StaffController ~ Novo criado por mim ...
using CursoMod165.Data;         // diz para ir ao data ler info para este controlador, usar só qd criar dbase
using CursoMod165.Models;     // diz para ir aos modelos ler info para este controlador

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using static CursoMod165.CursoMod165Constants;



namespace CursoMod165.Controllers
{

    // só o administrador é que pode entrar na criacao de staffs
    //[Authorize]  // para activar o login nesta vista
    // [Authorize(StaffRole = "ADMIN")] -> não porque é uma falha de segurança esta passa para um db incriptada
    //[Authorize(Roles = CursoMod165Constants.ROLES.ADMIN)]  // vamos buscar o user às constantes CAPS para constantes
    // ESTE SÓ É ALTERADO PELO admin
    [Authorize(Policy = POLICIES.APP_POLICY_ADMIN.NAME)]
    public class StaffController : Controller
    {

        // activa acesso à base de dados
        private readonly ApplicationDbContext _context;  // premir com botão direito a lampada para tnasformar var _context


        // Definir var Dbase de uso local 
        public StaffController(ApplicationDbContext context)
         {
             _context = context; 
         }

        // Lista conteudos que estao na base de dados
        public IActionResult Index()
        {
            // apresenta tabela de dados staff
            //IEnumerable<Staff> staffs = _context.Staffs.ToList();  
            IEnumerable<Staff> staffs = _context
                                                .Staffs
                                                .Include(s => s.StaffRole) // faz um join à outra tabela, "s" é cada um dos elementos staff
                                                .ToList();



            return View(staffs);
        }


        // A partir daqui colo as accoes tidas na pagina, como por exemplo o que cada botao faz
        // por outras palavras o esultado quando clico o botao ex: create
        // apresenta formuladrio para preencher
        [HttpGet]
        public IActionResult Create()
        {

            // aqui passamos dados entre tabela Staffroles e Staff
            // ViewBag.StaffRoles = new SelectList(_context.StaffRoles, "ID", "Name");
            // ViewBag.MedicStaffRoleID = _context.StaffRoles.First(sr => sr.Name == "Médico").ID;

            // foi criado este metodo e assim tenho de tirar as linhas de codigo em cima que já estão no metodo
            this.SetupStaffModel();

            return View();

        }


        // envia dados preenchidos no formulario
        [HttpPost]
        public IActionResult Create(Staff staff)
        {

            // com os dados preenchidos no formlario Create Staff, criar registo na Dbase
            if (ModelState.IsValid)
            {
                // Se retora falso algo está mal preenchido, deve aparecer uma info e nao sair da pagina
                // TO Do Criar novo Staff caso contrario;
                // return view customer com dados anteriores
                _context.Staffs.Add(staff);
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
            this.SetupStaffModel();

            // Apresenta a Vista do staff index

            return View(staff);
        }


        // acção details
        public IActionResult Details(int id)
        {
            // retorna os dados da chave que entra aqui e vem da tabela
            // usa-se ? porque pode não encontrar o dado e não pode ser null, quando colocamos o ? quer dizer que pode existir ou nao existir
            // o ? permite validar a entrada não deixando entrar erros
            // vamos à tabela customer procurar o customer com id de entrada
            // aqui "?" é para prevenir o null, basicamente é algo igual a: "if (Staff != null) staff=_context.Staffs.Find(id)"
            // staff vai receber os valores que vao ser obtidos da base de dados
            Staff? staff = _context.Staffs.Find(id);  // Chave primaria = id  

            if (staff == null) // se for diferente de null faz a vista
            {
                // se for null vai para a tabela INDEX
                // exemplo se o registo foi apagado enquando o utilizador estava a ver a tabela, e quando faz detail ele ja nao existe
                return RedirectToAction(nameof(Index));
            }
            // staff dados todos do staff como id de entrada 
            return View(staff);
        }



        // EDIT
        // nOVO EDIT
        [HttpGet] // obtem dados da base de  dados, neste caso a partir do padrao
        public IActionResult Edit(int id)
        {
            // retorna os dados da chave que entra aqui e vem da tabela
            // usa-se ? porque pode não encontrar o dado e não pode ser null, quando colocamos o ? quer dizer que pode existir ou nao existir
            Staff? staff = _context.Staffs.Find(id);  // Chave primaria = id

            if (staff == null) // se for diferente de null faz a vista
            {
                return RedirectToAction(nameof(Index));
            }

            // Metodo
            this.SetupStaffModel();


            // se existir registo na base de dados ou seja se o registo nao for null mostra conteudos do staff  (= id)
            return View(staff);
        }

        [HttpPost]   // envia dados para a base de dados
        public IActionResult Edit(Staff staff)
        {
            if (ModelState.IsValid)
            {
                _context.Staffs.Update(staff);        // atualiza
                _context.SaveChanges();                     // grava
                // era 09-04: return RedirectToAction(nameof(Index));   // volta para a pagina principal, para o cliente saber que gravou, mostra lista
            }

            // Metodo
            this.SetupStaffModel();

            // ver isto se é assim: return View(Staff);
            // era 09-04: return View();
            return View(staff);   // Novo 09-04; nota o meu staff é com "s" minusculo
        }

        // EDIT Fim

        // Aqui trata as opcoes ou accoes de delete para o staff
        public IActionResult Delete(int id)
        {
            // recebe a chave do registo e verifica:
            // se nao existir ou for null devolve nao encontrado
            if (id == null)
            {
                return NotFound();
            }

            Staff? staff = _context.Staffs.Find(id);

            if (staff == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(staff);
        }

        //################################################
        // O delete só tem post assim:
        //###########################################
        // para apagar o customer só preciso do ID não preciso dos dados do customer
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Staff? staff = _context.Staffs.Find(id);

            // verifica se o registo existe a se sim apaga-o
            if (staff != null)
            {

                _context.Staffs.Remove(staff);        // remove registo e atualiza
                _context.SaveChanges();                     // grava
                return RedirectToAction(nameof(Index));
            }

            return View(staff);
        }

        private void SetupStaffModel()
        {
            // aqui passamos dados entre tabela Staffroles e Staff
            ViewBag.StaffRoles = new SelectList(_context.StaffRoles, "ID", "Name");
            ViewBag.MedicStaffRoleID = _context.StaffRoles.First(sr => sr.Name == "Médico").ID;


        }



        // este metodo nao existe  no programa do Diogo, esperar pela atualizacao
        // Edit NUM Botao existente na form details Staff
        public IActionResult EditNum(int? num) 
        { 
        
            Staff? staff = _context.Staffs.Find(num); 

            if (staff == null) 
            { 
                return RedirectToAction(nameof(Index)); 
            } 
            
            return View(staff);    


        }

       


    }

}
