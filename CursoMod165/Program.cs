using CursoMod165;
using CursoMod165.Data;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Globalization;
using NToastNotify;
using Microsoft.AspNetCore.Identity.UI.Services;
using CursoMod165.Services;
using CursoMod165.Data.SeedDatabase;
using System.Security.Cryptography;
using static CursoMod165.CursoMod165Constants;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// configurar servi�o para ter utilizadores com credenciacao no acesso � base de dados ...
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;

        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireDigit = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 4;
        options.Password.RequiredUniqueChars = 4;
    }
)
    .AddRoles<IdentityRole>()  // aqui vamos criar um role de acessos
    .AddEntityFrameworkStores<ApplicationDbContext>();
// builder.Services.AddControllersWithViews(); // tiramos esta linha pk vamos fazer de forma diferente *1

// aQUI ADICIONAMOS LIgacao as politicas de credenciação/autorização de acessos ao sistema

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(POLICIES.APP_POLICY.NAME, Policy => Policy.RequireRole(POLICIES.APP_POLICY.APP_POLICY_ROLES));
    options.AddPolicy(POLICIES.APP_POLICY_ADMIN.NAME, Policy => Policy.RequireRole(POLICIES.APP_POLICY_ADMIN.APP_POLICY_ROLES));
    // New
    options.AddPolicy(POLICIES.APP_POLICY_VENDOR.NAME, Policy => Policy.RequireRole(POLICIES.APP_POLICY_VENDOR.APP_POLICY_ROLES));

    options.AddPolicy(POLICIES.APP_POLICY_WAREHOUSEMAN.NAME, Policy => Policy.RequireRole(POLICIES.APP_POLICY_WAREHOUSEMAN.APP_POLICY_ROLES));

});




// Tradu��es - Dicionario dos titulos para varias linguas
builder.Services.AddLocalization(options => options.ResourcesPath="Resources");

// a tradu��o � feita para a lingua em que est� o keyboard
// Aqui coloco qual a lingua de defeito da pagina pt ou en
const string defaultculture = "pt";  // primeiro lingua depois pais; exemplo pt-br

CultureInfo ptCI = new CultureInfo(defaultculture);

var supportedCultures = new[]  // � um array
{
    ptCI,
    new CultureInfo("en"),   // era: en-UK"
    //new CultureInfo("es")
};

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture(defaultculture);     //  suporta di lado fornt e backend-->
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});



builder.Services    // esta � a outra forma de fazer *1
    .AddMvc()       // aqui quer dizer que vamos ter um servi�o de localizacao
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)   // famos fazer os sufixos (uk,pt,es) 
    .AddDataAnnotationsLocalization(options =>
    // toast � um bal�o com informa��o se foi realizado o registo ou nao, neste caso colocado do lado direito
    {
        options.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(Resource));
    })
    .AddNToastNotifyToastr(new ToastrOptions()    // ToastrOptions() ???
    {
        ProgressBar = true,
        PositionClass = ToastPositions.TopRight

    });

// vamos adicionar um servi�o que permite o envio de emails:
// temos de fazer a liga��o do IemailSender com uma funcionalidade de ASP
// temos de criar uma diretoria onde vamos ter os nossos servi�o tais como envio de emails
builder.Services.AddTransient<IEmailSender, EmailSender>();



// installar
// Install-Package NToastNotify

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see http s://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();


// Ver esta site: http s://sweetalert2.github.io/
// aplicar tradu�oes -> aqui temos de dizer para usar o dicionario
// isto pode ser usado no site ASP.NET Core MVC, starting with translations
app.UseRequestLocalization(
    app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value
);

app.UseNToastNotify();

app.MapControllerRoute(
    name: "default",
    // aqui esta a maneira como s�o contruidas as strings para as chamada de paginas via URL
    // por outras palavras url:.../nomecontroller/metodo (ou accao)
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();


// Aqui mando executar o seed
SeedDB(); 

app.Run();


// o identity user/manager � que cria os utilizadores
// a ideia � criar varios utilizadores que tenham acesso � base de dados
// o File program.cs � que "arranca" com o projeto
void SeedDB()
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    // seed � uma inicializacao da base de dados
    var dbContext = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService <UserManager<IdentityUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    // o seed permite o preenchimento da DB de forma automatica
    // TODO - proxima semana
    SeedDatabase.Seed(dbContext, userManager, roleManager);
    // Site para autenticar SItes https gratuito: CERTbot.eff.org

    // PARA EVITAR PROBLEMAS CRIAR USER AUTOMATICO A PARTIR DO seed
}
/**/