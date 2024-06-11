using CursoMod165.Data;
using CursoMod165.Models;
using Microsoft.AspNetCore.Authorization;    // para activar login
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;  // não usado no do diogo
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using NToastNotify;
using System.Text;
using static CursoMod165.CursoMod165Constants;        // não usado no do diogo
// using Microsoft.EntityFrameworkCore;
//using System.Linq;

namespace CursoMod165.Controllers
{
    // só o administrador é que pode entrar na criacao de staffs
    [Authorize(Policy = POLICIES.APP_POLICY.NAME)]  // para activar o login nesta vista
    // [Authorize(StaffRole = "ADMIN")] -> não porque é uma falha de segurança esta passa para um db incriptada
    //[Authorize(Roles = CursoMod165Constants.ROLES.ADMIN)]  // vamos buscar o user às constantes CAPS para constantes
    public class AppointmentController : Controller
    {
        // cria logo aqui o construtor
        private readonly ApplicationDbContext _context;
        // Nova 23-04
        private readonly IToastNotification _toastNotification; // Singleton -> ou seja tudo é igual em todo o lado as mesmas regras
        private readonly IHtmlLocalizer<Resource> _sharedLocalizer;
        private readonly IStringLocalizer<Resource> _localizer;
        private readonly IEmailSender _emailSender;
        private object htmlBody;  // nao vi esta linha na pagina dum colega .. aula 20
        // o Ihtmllocalizer permite o uso de texto formatado em html... e interpreta este como html e nao como texto
        // o IEmailSender permite o envio de emails; teos de criar 1º o serviço de envio de emails
        // o Iemailsender e Ihtmllocalizer são interfaces

        // IToastNotification toastNotification)
        public AppointmentController(ApplicationDbContext context, 
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

        // -- Index
        public IActionResult Index()
        {
            // tenho de incluir StaffRole no fim da linha para aparecer a coluna staff role preenchida


            // tenho de referenciar para nao aparecer o nome duplicano no index
            // o diogo usa a letra inicial de cada do appointment, para apontar para o nome
            // "include" corresponde à chave estrangeira, ou tambem chamado de join
            IEnumerable<Appointment> appointments = _context.Appointments
                                                            .Include(a => a.Staff.StaffRole) // a = appointment
                                                            .Include(a => a.Customer) // relacionamento (inner join) usa-se chave estrangeira
                                                            .ToList();

            return View(appointments);
        }

        // agora vou buscar a lista das consultas de amanha
        // aqui indico com o campo data que só vou ver os dados que têm a data de amanha
        // no controlador é onde dou os comando e defino as condições que quero visualizar IActionResult
        public IActionResult TomorrowsAppointments()
        {
            // tranco as chaves estrangeiras e com condição date == data de amanhã
            var tomorrowsAppointments = _context.Appointments
                                               .Include(a => a.Staff)  //inner join => a partir da chave estrangueiro quero o nome
                                               .Include(a => a.Customer)  //inner join => a partir da chave estrangueiro quero o nome
                                               .Where(a => a.Date == DateTime.Today.AddDays(1))    // Adddays(1) adiciona 1 dia ao dia de hoje
                                               .ToList();  // apresenta a lista na frame 
            return View(tomorrowsAppointments);
        }


        // Fazer a listagem da proxima semana parecido com dia seguinte:
        public IActionResult NextWeekAppointments()
        {
            // foi criada uma funcao para obtermos data de inicio e fim da semana
            DateTime startDate, endDate;
            GetNextWeeksDates(out startDate, out endDate);

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            List<Appointment> nextWeekAppointments = GetAppointmentsBetweenDates(startDate, endDate);
            return View(nextWeekAppointments);
        }


        private List<Appointment> GetAppointmentsBetweenDates(DateTime startDate, DateTime endDate)
            { 

            // tranco as chaves estrangeiras e com condição date == data de amanhã
            return _context.Appointments
                             .Include(a => a.Staff.StaffRole)  //inner join => a partir da chave estrangueiro quero o nome
                             .Include(a => a.Customer)  //inner join => a partir da chave estrangueiro quero o nome
                             .Where(a => a.Date >= startDate && a.Date <= endDate)    // Adddays(1) adiciona 1 dia ao dia de hoje
                             .ToList();  // apresenta a lista na frame 
            }



        // funcao "GetNextWeeksDates" Criado com extract Method (Right Click)
        private static void GetNextWeeksDates(out DateTime startDate, out DateTime endDate)
        {
            int x = 1;
            if (DateTime.Today.DayOfWeek != DayOfWeek.Sunday)
            {
                x = 8 - (int)DateTime.Today.DayOfWeek;
            }
            startDate = DateTime.Today.AddDays(x);
            endDate = DateTime.Today.AddDays(x + 4);
        }



        // teoricamente deve ir para uma vista ... tem de ir para uma vista ...
        // Aqui vou enviar emails para todos os que tem consulta na paroxima semana
        // Aqui vamos definir o codigo para envio de email para todos os utentes que tem consulta na proxima semana (Next Week)
        public IActionResult EmailReminderNextWeekAppointments()
        {
            // copia do Nextweek appointment
            // logo vamos criar uma funcao ou metodo para termos as datas da proxima semana
            // ou seja a lista das consultas e a data de inicio e a data de fim


            // EU nao quero que retorne para a vista apenas quero que envie emails
            // enventualmente posso enviar aqui uma mensagem de sucesso
            // (codigo desta linha foi retirado pk nao se pretende retornar nada para a vista) return View(nextWeekAppointments);



            // Fluxo a criar para fazer o envio de emails para as consultas da proxima semana
            // 1 - Obter data de inicio e fim da proxima semana
            // 2 - Obter as consultas da proxima semana
            // 3 - Obter a lingua e o respectivo template de e-mail
            // 4 - Adaptar o template a cada consulta e enviar o e-mail


            // Obter data de inicio e fim da proxima semana
            DateTime startDate, endDate;
            GetNextWeeksDates(out startDate, out endDate);

            // Obter as consultas
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            List<Appointment> nextWeekAppointments = GetAppointmentsBetweenDates(startDate, endDate);

            // Obter a lingua e o respectivo template de e-mail
            var culture = Thread.CurrentThread.CurrentCulture;

            string template = System.IO.File.ReadAllText(
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "EmailTemplates",
                    $"next_week_appointment.{culture.Name}.html"
                 )
            );





            //  Adaptar o template a cada consulta e enviar o e-mail
            foreach (var appointment in nextWeekAppointments)
            {
                StringBuilder htmlBody = new StringBuilder(template);
                htmlBody.Replace("##CUSTOMER_NAME##", appointment.Customer.Name);
                htmlBody.Replace("##APPOINTMENT_DATE##", appointment.Date.ToShortDateString());
                htmlBody.Replace("##APPOINTMENT_TIME##", appointment.Time.ToShortTimeString());
                htmlBody.Replace("##STAFF_ROLE##", appointment.Staff.StaffRole.Name);
                htmlBody.Replace("##STAFF_NAME##", appointment.Staff.Name);

                _emailSender.SendEmailAsync(appointment.Customer.Email, "Reminder of Scheduled Appointment",
                   htmlBody.ToString());
            }

            // Mostra Toast a informar que enviou emails
            _toastNotification.AddSuccessToastMessage($" {nextWeekAppointments.Count} Emails sucessfully sent.");


            return RedirectToAction(nameof(NextWeekAppointments));
            
        }







        // OK; Get e Post são metodos, o Get refere-e à aquisição,
        // exemplo colocar um URL no browser e premir enter
        // e o Post ao envio de dados depois de preenchido um formulário
        // A requisição GET é enviado como string anexada ao URL
        // post é encapsulada junto ao coprto http 
        [HttpGet]
        public IActionResult Create()
        {

            this.SetupAppointments();
            return View();
        }

        // OK
        [HttpPost]
        public IActionResult Create(Appointment appointment)

        {
            if (ModelState.IsValid)
            {
                _context.Appointments.Add(appointment);
                _context.SaveChanges();

                // Toastr.SucessMessage tem de aparecer msg quando criar um novo
                _toastNotification.AddSuccessToastMessage("Appointment sucessfully created.");  // nao sei se uso ;
                // _toastNotification.AddWarningToastMessage("Appointment warning.");  // basta colocar ponto para aparecer as opcoes possiveis ;

                
                Customer? customer = _context.Customers.Find(appointment.CustomerID);
                Staff? staff = _context.Staffs
                                        .Include(s => s.StaffRole)
                                        .Where(s => s.ID == appointment.StaffID)
                                        .Single();              // com o single, rebenta se existir mais do que um, em vez disso posso fazer querys à base de dados
                // se entretanto alguem apagou o registo faz isto :
                if (customer == null || staff == null)    
                {
                    return NotFound();
                }

                // como obter a lingua selecionada na view do Browser do cliente :
                var culture = Thread.CurrentThread.CurrentUICulture;


                // agora temos de carregar o template do email, que não é mais do que uma string enorme, a raiz da dir é CursoMod165
                // vai á diretoria do projecto "EmailTemplates"
                string template = System.IO.File.ReadAllText(
                    Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "EmailTemplates", 
                        $"create_appointment.{culture.Name}.html"
                    )
                 );


                StringBuilder htmlBody = new StringBuilder(template);
                htmlBody.Replace("##CUSTOMER_NAME##", customer.Name);
                htmlBody.Replace("##APPOINTMENT_DATE##", appointment.Date.ToShortDateString());
                htmlBody.Replace("##APPOINTMENT_TIME##", appointment.Time.ToShortTimeString());
                htmlBody.Replace("##STAFF_ROLE##", staff.StaffRole.Name);
                htmlBody.Replace("##STAFF_NAME##", staff.Name);



                //Aqui enviamos um email ...
                // Retirado TMP-- 
                _emailSender.SendEmailAsync(appointment.Customer.Email, "Appointment Scheduled",
                    htmlBody.ToString());

                return RedirectToAction(nameof(Index));
            }

            // Toastr.SucessMessage
            _toastNotification.AddErrorToastMessage("Check the form again!",
                new ToastrOptions {
                    Title = "Appointment Creation Error",
                    TapToDismiss = true,
                    TimeOut = 0
                });
            this.SetupAppointments();  // este metodo esta definido no controlador appointment
            return View(appointment);

        }



        // estas linhas de codigo correspondem ao codigo que é executado quando primo o botão create na view appointment
        // ok
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Appointment? appointment = _context.Appointments.Find(id);

            if (appointment == null)
            {
                return RedirectToAction(nameof(Index)); 
            }

            this.SetupAppointments();
            return View(appointment);

        }
        // ok
        [HttpPost]  
        public IActionResult Edit(Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                _context.Appointments.Update(appointment);
                _context.SaveChanges();

                // em C# tudo é uma classe, ou seja o nivel de abstracao é enorme
                // temos de traduzir caso contrario não aparece na lingua selecionada
                // vai aparecer uma mensagem "Toast" de sucesso na edicao
                // temos de fazer um template para poder traduzir varios numeros
                // colocar a negrito <b> ... </b>
                // podiamos usar string.concat para não martelar o resources com <b> ...
                string message =
                    string.Format(_sharedLocalizer["<b>Appointment # {0}</b> sucessfully edited!"].Value,
                                  appointment.Number);  // aqui traduz
                //string message2 = "Appointment Number #" + appointment.Number + "successfully edited!";

                // temos de tirar a hora 00:00 da data apresentada para ficar so o dia
                // agora quero que a palavra data aparece na linha seguinte
                // usar "\r\n; temos de fazer dentro do comando HTML logo temos de por <br /> que é uma mudanca de linha
                message += "<br />" + string.Format(_sharedLocalizer["Date: <b>{0}</b> at <b>{1}</b>"].Value,
                                  appointment.Date.ToShortDateString(), 
                                  appointment.Time.ToShortTimeString());

                _toastNotification.AddSuccessToastMessage(message,
                    new ToastrOptions { Title = _sharedLocalizer["Success"].Value ,
                        TimeOut = 0,
                        TapToDismiss = true
                    });



                // Mensagem 2 teste  <prep> permite dar espaços
                // ver qual o que está a funcionar _localizer ou _sharedlocalizer
                string message2 =
                   string.Format(_localizer["<prep>  <b>Appointment2 # {0}</b> sucessfully edited!"].Value,
                                 appointment.Number);  // aqui traduz
                //string message2 = "Appointment Number #" + appointment.Number + "successfully edited!";

                // temos de tirar a hora 00:00 da data apresentada para ficar so o dia
                // agora quero que a palavra data aparece na linha seguinte
                // usar "\r\n; temos de fazer dentro do comando HTML logo temos de por <br /> que é uma mudanca de linha
                message2 += "<br />" + string.Format(_localizer["Date: <b>{0}</b> at <b>{1}</b>"].Value,
                                  appointment.Date.ToShortDateString(),
                                  appointment.Time.ToShortTimeString());

                _toastNotification.AddSuccessToastMessage(message2,
                    new ToastrOptions { Title = _localizer["Success"].Value, TimeOut=0, TapToDismiss=true });

                // estava oliveiraxpedro@gmail.com
                // talvez usar aqui pedroxoliveira
                // retirado tmp --
                _emailSender.SendEmailAsync("oliveiraxpedro@gmail.com",
                    "Edit Appointment", "Edited Successfully");
                
                return RedirectToAction(nameof(Index));
            }

            this.SetupAppointments();
            return View(appointment);

        }



        




        // defenir que só os enfermeiros e medicos tem acesso aos dados das consultas dos clientes
        private void SetupAppointments()
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
   
    // Aqui vou criar o codigo que vai ser executado quando premir o botão send emails appointment next week
    
    
    }

}
