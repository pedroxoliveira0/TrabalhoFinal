using Azure;
using CursoMod165.Models;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Experimental.FileAccess;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Diagnostics;
using System.Net;
using System.Runtime.Intrinsics.X86;


// Codigo ver RoundTheCode.com
// http s://www.youtube.com/watch?v=JdwpJcyuFSo
// http s://www.youtube.com/watch?v=C5cnZ-gZy2I

// GitHub Professor:
// http.s://github.com/DiogoDaSilva/CursoMod165/

// Site a ver:
// http s://duminio.com/

// Site para instala��o do GRID:
// http s://mvc-grid.azurewebsites.net/
// Local onde est� instalado o grid -> C:\Users\poliveira\.nuget\packages\nonfactors.grid.mvc6\7.2.0\content\Views\Shared
// copiar para o projeto pasta Shared

// Sistema de gestao de versoes
// MY GitHub
// pedroxoliveira0 (GitHub)
// GitHub#1234

namespace CursoMod165.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        // coisas privadas as variaveis come�am por "under scoore" (_x); ou mais tecnicas da propria linguagem (ou frame work)
        // definicao de uma variavel que vai contar o n� de vezes que a pagina foi visitada
        // Contador de visualiza��es da p�gina Mypage
        static int Counter { get; set; } = 0;


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


		// JOB STATEMENT
        // controlo da pagina enunciado do trabalho
        public IActionResult JobStatement()
        {
            HomeController.Counter = HomeController.Counter + 1; // this.Counter++; nome "this" significa a propria classe; � este mesmo counter
            
            // usados para enviar e receber dados do View (vista) de e para o controlador
            TempData["Author"] = "Pedro Oliveira";       // dicionario de chaves 
            
			TempData["Data"]= DateTime.Now;     // Os dados permanecem se sair da pagina (exemplo preencher formulario

			// dados so permanecem enquanto estiver na pagina 
			ViewData["Titulo1"] = "Trabalho Final Curso ASP.NET 165";        

			ViewBag.Titulo2 = "1 de Mar�o de 2024";      // as variaveis s�o do tipo dinamico ou seja podem ser string ou numero
												// Exercicio mostrar data
												//TempData["Date"] = DateOnly;


			// ViewBag s�o usados para transferir dados tempor�rios (n�o incluidos no modelo),
			// que permitem transferir ddos entre o controlador e a vista (view)
			// � dinamica e est� est� definida na class do controlador

			// Lista enunciado
			List<string> Enunciado = new List<string>()
			{
				"Categorias",
				"Produtos",
				"Clientes",
                "Vendas"

			};
			ViewData["Enunciado"] = Enunciado;


			// Lista Categorias
			List<string> Categorias = new List<string>()
			{
				"Nome",
				"Descri��o"

			};
            // passa para a view como tipo "tempData"
			TempData["Categorias"] = Categorias;

			// Lista Produtos
			List<string> Produtos = new List<string>()
			{
				"Descri��o",
				"Pre�o Final (ignorem IVAs)",
				"Quantidade em stock",
				"Peso",
				"Categoria"

			};
			// passa para a view como tipo "tempData"
			TempData["Produtos"] = Produtos;
			ViewBag.Produtos = Produtos;

			// Clientes
			List<string> Clientes = new List<string>()
			{
				"Nome completo",
				"E-mail",
				"Data de nascimento",
				"Morada",
				"Localidade",
				"C�digo-postal",
				"NIF",
				"N�mero de cliente"

			};
			ViewData["Clientes"] = Clientes;

			// Vendas
			List<string> Vendas = new List<string>()
			{
				"Identificador da venda (ex: V2024/A/001)",
				"Data",
				"Hora",
				"Cliente",
				"Produtos vendidos (lista de produtos adicionar, qtd e pre�o unit.)",
				"Observa��es",
				"Valor final",
				"Estado [Encomendada, Em processamento, Processada, Enviada]",
				"Paga [Sim ou N�o]"

			};
			ViewData["Vendas"] = Vendas;


			// Areas de Categorias
			List<string> ACategorias = new List<string>()
			{
				"2.1.1. Listar as categorias.",
				"2.1.2. Criar/Editar/Apagar uma categoria.",
				"2.1.3. Ver a lista de produtos que existem numa categoria."

			};
			ViewData["ACategorias"] = ACategorias;

			// Areas de Produtos
			List<string> AProdutos = new List<string>()
			{
				"2.2.1. Listar os produtos.",
				"2.2.2. Criar/Editar/Ver/Apagar um produto."

			};
			ViewData["AProdutos"] = AProdutos;


			// Areas de Clientes
			// "\r\n" ser� que passa para a linha seguinte a frase ??!!
			List<string> AClientes = new List<string>()
			{
				"2.3.1. Listar os clientes. Filtrar por nome e NIF.",
				"2.3.2. Criar/Editar/Ver/Apagar um cliente.",
				"2.3.3. Ver o hist�rico de vendas do cliente.",
				"2.3.4. Ver o saldo corrente do cliente (somat�rio de vendas menos o somat�rio\r\nde vendas pagas)."

			};
			ViewData["AClientes"] = AClientes;


			// Sub grupo area de Vendas
			// 1� Passo encomenda
			List<string> IPasso = new List<string>()
			{
				"2.4.2.1. Seleccionar o cliente;",
				"2.4.2.2.Seleccionar os produtos;",
				"2.4.2.3.Calcular o valor final de acordo com os produtos,\r\n\t\t\tquantidades e pre�os;",
				"2.4.2.4.Preencher o identificador e as observa��es;"

		};
			ViewData["IPasso"] = IPasso;

			// 2� Passo encomenda
			List<string> IIPasso = new List<string>()
			{
				"2.4.2.5. Seleccionar a venda;",
				"2.4.2.6.Colocar a encomenda em processamento;",
				"2.4.2.7.Verificar a exist�ncia de um produto;",
				"2.4.2.7.1.Se n�o existir, registar nas observa��es a\r\n\t\t\tinexist�ncia do produto e abater o pre�o;",
				"2.4.2.8.Depois de validada, colocar a encomendada como\r\n\t\t\tprocessada;"

		};
			ViewData["IIPasso"] = IIPasso;

			// 3� Passo encomenda
			List<string> IIIPasso = new List<string>()
			{
				"2.4.2.9. O Trabalhador do armaz�m vai buscar os produtos;",
				"2.4.2.10.H� uma actualiza��o dos stocks dos produtos;",
				"2.4.2.11.A encomenda � empacotada e enviada para a empresa\r\nde transportes;",
				"2.4.2.12.� enviado um e - mail ao cliente final."

			};
			ViewData["IIIPasso"] = IIIPasso;

			// tem de existir as seguintes vistas
			// 4� Passo encomenda
			List<string> IVPasso = new List<string>()
			{
				"2.4.3.1. Vendas �Encomendadas�;",
				"2.4.3.2.Vendas �Em Processamento�;",
				"2.4.3.3.Vendas �Processadas�",
				"2.4.3.4.Vendas �Enviadas�",
				"2.4.3.5.Vendas por pagar"

			};
			ViewData["IVPasso"] = IVPasso;


			// Internacionaliza�ao

			// Papeis
			// tipos de papeis
			List<string> TipoPapeis = new List<string>()
			{
				"Vendedor",
				"Log�stica",
				"Administrador"

			};
			ViewData["TipoPapeis"] = TipoPapeis;


			// Lista de regras dos papeis
			// regras dos papeis
			List<string> RegrasPapeis = new List<string>()
			{
				"4.1. Todos os trabalhadores t�m de estar logados para aceder � aplica��o.",
				"4.2. Os Vendedores t�m acesso � �rea dos produtos e � �rea das vendas, mas s� podem consultar uma venda existente que esteja no estado:",
				"�Encomendada�;",
				"4.3.Os trabalhadores da Log�stica t�m acesso � �rea de vendas, mas s� podem consultar uma venda que esteja nos estados: �Em processamento�",
				"e �Processada�.",
				"4.4.O administrador tem acesso a tudo."

			};
			ViewData["RegrasPapeis"] = RegrasPapeis;


			


			return View(HomeController.Counter);  // apago os dados e volto a escrever para aparecer as varias opcoes do help
        
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
                // mante informa��o cookie durante 1 ano
            );

            return LocalRedirect(returnUrl);
        }

    }
}
