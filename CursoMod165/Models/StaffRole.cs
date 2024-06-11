using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CursoMod165.Models
{
    
    // depois disto criar tabela ap db context
    public class StaffRole
    {
        public int ID { get; set; }

        [StringLength(255)]
        public string Name { get; set; }


        [Required]
        [DefaultValue(true)]
        public bool CanDoAppointments { get; set; }


        // QUnado dá erro na migração (Build error) fazer RE.build Solution

        // Para nao ter erro vermelho colocar no codigo da  DB criada:
        // Os dados do Staff tem de ser apagados para criar a tabela StaffRole
        //  onDelete: ReferentialAction.Restrict);

        // Update-database 0 -> remove todas as migrações que foram feitas

        // muito interessante o nome do codigo da tabela secundaria fica com o nome do codigo dado
        // para criar a tabela

        // Vamos criar aqui uma variavel booleana para validar quem pode aceder aos dados dos utentes
        // Agora vamos ter de adicionar este novo campo  na base de dados c

        // Adicionar o campo CanDoAppointments à Tabela StaffRole
        // ADD-MIGRATION StaffRole_AddedField_CanDoAppointments
        // TENHO DE ADICIONAR NA dBContext ... um linha com true
        // UPDATE-DATABASE

    }
}
