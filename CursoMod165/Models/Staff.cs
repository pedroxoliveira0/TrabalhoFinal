// tentar perceber o que cada um destas bibiotecas faz:
using Microsoft.AspNetCore.Identity;   // saber o que faz ....
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;    // saber o que faz ....
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;    // saber o que faz ....


namespace CursoMod165.Models
{
    // Fazer para o staff o mesmo que foi feito para o customer, ver videos
    public class Staff
    {
        public int ID { get; set; }

        [StringLength(255)]
        [Required]
        public string Name { get; set; }


        [Required]
        [Display(Name = "Employee Number")]
        [StringLength(30), Range(0, 999999)]    // aceita no maximo 6 numeros
        [DisplayFormat(DataFormatString = "{0:d}")]
        public string EmployeeNumber { get; set; }


        [Required]
        [StringLength (255)]    
        public string Address { get; set; }


        [Required]
        [StringLength(30)]
        [Display(Name = "VAT Number")]
        public string NTF { get; set; }


        [Required]
        [StringLength(255)] 
        [Display(Name="E-Mail")] 
        public string Email { get; set; }

        [Required]
        [StringLength(30)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateOnly Birthday { get; set; }  // Double ou float para numeros

        // ####################################################
        // Ver como colocar em € euros, aparece em euros deve ser o valor de defeito do windows
        // [Range(0, 5000)]  // testar ou regular expression  ou display format
        [DataType(DataType.Currency)]
        [Required]
        // [Column(typeName = "decimal(18,2)")]
        [Precision(18,2)]  //  O precision vai truncar os numeros abaixo e acima de 18 e 2  
        // truncado = apagado
        public decimal Salary { get; set; }   // usado para dinheiro com valor real
        // na tabela substituir dB onDelete: para NoAction


        // VAmos criar regras de negocio:
        // Médico
        // Enfermeiro
        // Administrativo
        // na pratica vamos criar uma tabela ROLE com cada uma das funcoes (ID* ; Name) 

        // aqui estamos a definir a ligação para o objeto staffrole, o staff tem de ter um staffrole 
        // StaffRole.Name
        // procurar informação sobre "Ver ViewModels"
        // Para o no ficar correto tenho de colocar o rato sobre o nome e selecionar a variavel correta, ate aparecer verde
        [ValidateNever]  // isto chama-se TAG; esta a dizer que o campo não tem de ser validado, pk vem vazio
        [ForeignKey("StaffRoleID")] // só necessito desta linha alterar o nome da tabela, por defeito a key é = "nome modelo + ID"
        public StaffRole StaffRole { get; set; }   // o campo Staffrole nao vem preenchido, vem vazio
        // a ausencia da validateNever cria muitos erro de compilação
        // Staffrole é um campo de navegação e devemos fazer chave estrangeira (ForeignKey)

        // definir a chave
        [Display(Name="Role")]
        [Required]
        public int StaffRoleID { get; set; }

        // Para criar a relação entre a tabela Staff e StaffRole fazer:
        // Escrever no PM>   ADD-MIGRATION CreateStaffRole_AddedRelationToStaff
        //  onDelete: ReferentialAction.Restrict);
        // A solução passava só por apagar os registos não era preciso alterar de NOaction para Restrict


        // Para criar uma especialidade; como é no staff temos de a definir no model Staff
        // Speciality
        [Display(Name = "Specialty")]
        [Required]
        public Specialty? Specialty { get; set; }


    }
}
