// ###################################
// Esta tabela tem gravado o ultimo numero da venda, no incio de cada ano
// é criado um registo novo com 0 no inicio de cada ano
// num sistema mais rubusto a data é obtida de uma fonte fidedigna
//
// ###################################
//
// Campo do tipo 2024A001 - Data + MM + Number Count (incremental)
//
// Os campos a usar ou incluir na tabela também dependem das funcionalidades que
// queremos, para essa tabela. 
// esta tabela apenas tem um contador, ou seja é uma especia de guardador e criador do 
// identificador da Encomenda/venda criada no sistema
using System.ComponentModel.DataAnnotations;

namespace CursoMod165.Models
{
    public class Identifier
    {
        // chave da tabela
        public int ID { get; set; }

        // ANO
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Year")]
        public DateOnly Year { get; set; }  // = DateTime.Today;

        // COD.Letter inicio A
        // Passa para a letra seguinte quando chegar a 999
        [StringLength(10)]  // varia de 0 a 999 ou regular expression  ou display format
        public string COD { get; set; } = "A"; // = string.Equals('A');


        // contador - correspondente à ultima venda feita se 0, não foi feita
        // nenhuma venda até à data 
        // Quando chega a 999 o COD é alterado, para a proxima letra
        [StringLength(30), Range(0, 999)]  // varia de 0 a 999 ou regular expression  ou display format
        [DisplayFormat(DataFormatString = "{0:d}")]
        [Required]
        [Display(Name = "Contador Venda")]
        public string Number { get; set; } 

    }
}
