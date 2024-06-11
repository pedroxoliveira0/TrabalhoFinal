using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CursoMod165.Models
{
    // definicao da estrutura de variaveis para o utente
    // o modelo é uma representação da base de dados, nao é a base dados
    // EntityFramework -> Biblioteca de conexão à Base de Dados
    public class Customer
    {
        // primeiro Campo : ID  (Var publica)
        public int ID { get; set; }

        //[PrimaryKey('XPTOid',)]   -> [Forenkey] -> cria constrain
        //public int XPYOid { get; set; } 


        [StringLength(255)]
        [Required]  // de preenchimento obrigatorio
        public string Name { get; set; }

        [StringLength(255)]
        [Required]
        public string Address { get; set; }

        [DataType(DataType.Date)]   // a variavel tem do ser do tipo data
        [Required]
        public DateOnly Birthday { get; set; }  // date only apenas data sem hora

        [StringLength(20)]
        [Display(Name = "Phone Number_is")]  // aparece no campo as palavras separadas
        public string PhoneNumber { get; set; }

        [StringLength(255)]
        [Display(Name = "E-mail")]
        [DataType(DataType.EmailAddress)] // aqui como no html não coloca tipo email temos de forcar essa opção
        [Required]
        public string Email { get; set; }   // aqui como no html não coloca tipo email temos de forcar essa opção

        

        [StringLength(30)]
        [Required]
        [Display(Name = "VAT Number")]
        public string NIF { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.Now;    // atribui valor atual `var data

        
        // ##########################
        // Novos Campos
        // ##########################
        // Localidade = City
        //  [StringLength(255)]
        //  [Required]
        //  public string City { get; set; }

        // Codigo Postal- ZipCode
        //  [StringLength(20)]
        //  [Required]
        //  [Display(Name = "Zip Code")]
        //  public string ZipCode { get; set; }


        // Numero Cliente - CodCustomer
        // so um numero Incrementa uma uni 
        //  [StringLength(30), Range(0, 999999999999)]  // ou regular expression  ou display format
        //  [DisplayFormat(DataFormatString = "{0:d}")]
        //  [Required]
        //  [Display(Name = "Cod. Customer")]
        //  public string CodCustomer { get; set; }



        // ###############################
        // Campos a Nao usar (Apagar)
        // ##############################
        [StringLength(30), Range(0, 999999999999)]  // ou regular expression  ou display format
        [DisplayFormat(DataFormatString = "{0:d}")]
        [Required]
        [Display(Name = "HealthCare User Number")]
        public string USNS { get; set; }
    }
}
