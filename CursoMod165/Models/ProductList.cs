using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CursoMod165.Models
{
    public class ProductList
    {

        // Inicio da definicao dos campos *Chave Primaria
        // ###########################################
        //  
        // Esta tabela faz a ligacao entre o cliente, a acao da venda 
        // e os produtos que fazem parte dessa venda
        //
        // ###########################################
        public int ID { get; set; }

        [ValidateNever]         // exclude Attribute  from validation
        [ForeignKey("SaleID")]
        public Sale Sale { get; set; }

        [Display(Name = "Sale")]
        [Required]
        public int SaleID { get; set; }


        // Lista de produtos .... não sei bem como fazer
        [ValidateNever]         // exclude Attribute  from validation
        [ForeignKey("ProductID")]
        public Product Product { get; set; }

        [Display(Name = "Product")]
        [Required]
        public int ProductID { get; set; }


        [DataType(DataType.Currency)]
        [Required]
        // [Column(typeName = "decimal(18,2)")]
        [Precision(18, 2)]  //  O precision vai truncar os numeros abaixo e acima de 18 e 2  
                            // truncado = apagado
        public decimal Price { get; set; }   // usado para dinheiro com valor real


        // Quantidade do produto em Encomenda/Venda
        // que pode ser em Kg
        [Required]
        [Precision(10, 3)]
        public decimal Quantity { get; set; }


        // Adicionar Campo Observaçoes
        // ver se faz mais sentido adicionar as observacoes neste campo ou
        // nas observacoes do formulario venda geral ... 
        //[DataType(DataType.MultilineText)]
        //[Display(Name = "Observations")]
        //public string? Observations { get; set; }

    }
}
