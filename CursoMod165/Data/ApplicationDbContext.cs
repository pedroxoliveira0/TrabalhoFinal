using CursoMod165.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


// http s://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_using?view=powershell-7.4
// http     s://dotnettutorials.net/lesson/asp-net-core-command-line-interface/?utm_content=cmp-true
// http s://www.tutorialsteacher.com/core/net-core-command-line-interface
// http s://getbootstrat.com ...
// http s://stackoverflow/questions/?
// http s://learn.microsoft.com/en-us/aspnet/core/mvc/views/overview?view=aspnetcore-8.0
// http s://mvc-grid.azurewebsites.net
// http s://www.tutorialsteacher.com/mvc/viewbag-in-asp.net-mvc
// mvc tutorial; tutorialsteacher.com
// google >> asp.net mvc core ???

namespace CursoMod165.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // aqui é feita a Criacao de tabelas
        // Tables
        public DbSet<Customer> Customers { get; set; } = default!;

        // fazer a tabela Staff 16-03-2024
        public DbSet<Staff> Staffs { get; set;} = default!;     // no codigo Staff passa a staffs - k será a dB
        // staffs é uma propriedade e nao uma variavel, o ! é para obrigar a ter sempre dados
    
        public DbSet<StaffRole> StaffRoles { get; set; } = default!;
        // 1º criamos o modelo, depois 


        public DbSet<Appointment> Appointments { get; set; } = default!;
		// depois de definir a dp (dBase) com esta linha de codigo, fazer no Package manager console
		// ADD-MIGRATION CreateAppointmentSchema, de seguida fazer:
		// UPDATE-DATABASE


		// na Migrations do diogo aparace na informacao da tabela .. "ReferentialAction.NoAction"; na minha ..cascade)


		// #############################################
		// A proximas linhas sao referentes ao trabalho final
		// #############################################
        // a applicationsDB é  a minha ligacao à base de dados


		// Criar tabela Categories
		public DbSet<Category> Categories { get; set; } = default!;

		// Criar tabela Products
		public DbSet<Product> Products { get; set; } = default!;


        // Criar tabela Sales/vendas; criar a Sales só depois da Product List
        public DbSet<Sale> Sales { get; set; } = default!;


        // Criar tabela Product List ou seja a lista de produtos incluido na encomenda
        //
        public DbSet<ProductList> ProductLists { get; set; } = default!;


        // Criar tabela do Identificador das Vendas 
        // No inicio do ano é criado um registo novo,
        // duarante o ano quando é criada uma encomenda
        // o identifier é lido e é atualizado incrementando uma unidade
        public DbSet<Identifier> Identifiers { get; set; } = default!;



    }



}
