using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CursoMod165.Models
{
    // Modelo para agendamento das consultas
    // Definir a Dbase no application content e depois fazer migração
    public class Appointment
    {
        public int ID { get; set; }


        [Required]
        [Display(Name = "Appointment #")]  // tem de se colocar display para fazer tradução aos titulos/campos; fazer o mesmo para s outras classes alem da appointment
        public string Number { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date")]
        public DateTime Date { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Time")]
        public DateTime Time { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Informations")]
        public string? Informations { get; set; }   // ? para ter sempre algo preenchido alternativa ... { get; set;} = "";


        // Var para indicar se a consulta foi realizada ou nao
        [Required]
        [Display(Name = "Is Done")]
        public bool IsDone { get; set; }


        [ForeignKey("StaffID")]
        [ValidateNever]
        public Staff Staff { get; set; }
        
        [Display(Name = "Staff")]
        [Required]
        public int  StaffID {  get; set; }



        [ForeignKey("CustomerID")]
        [ValidateNever]         // exclude Attribute  from validation
        public Customer Customer { get; set; }

        [Display(Name = "Customer")]
        [Required]
        public int CustomerID {  get; set; }    

    }
}
