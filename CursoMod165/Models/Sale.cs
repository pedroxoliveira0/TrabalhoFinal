using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CursoMod165.Models
{
	public class Sale
	{

		// Inicio da definicao dos campos
		public int ID { get; set; }

		// Codigo da venda formatacao do tipo V2024A001 ,Usar o Reg. ID para criar o numero automaticamente
		// julgo que a ideia é ter um mesmo codVenda referente a uma serie de produtos vendidos
		
		//[StringLength(30), Range(0, 999999999)]  // ou regular expression  ou display format valorMax=99999
		//[DisplayFormat(DataFormatString = "{0:d}")]
		[StringLength(30)]
		[DataType(DataType.Text)]
        [Required]
        [Display(Name = "Cod. Venda")]  
		public string CodVenda { get; set; }


		// Cliente - lista de cliente - chave estrangeira
		[ValidateNever]         // exclude Attribute  from validation
		[ForeignKey("CustomerID")]
		public Customer Customer { get; set; }

		[Display(Name = "Customer")]
		[Required]
		public int CustomerID { get; set; }


        // campo que tem o estado da encomenda ()
        [Display(Name = "Status")]
        [Required]
        public Status? Status { get; set; }


        // Lista de produtos .... não sei bem como fazer
		// posso ter uma encomenda sem produtos
		// no entanto nao posso um registo na lista de produtos
		// sem ter uma encomenda/venda aberta
        //	[ValidateNever]         // exclude Attribute  from validation
        //	[ForeignKey("ProductListID")]
        //	public Customer ProductList { get; set; }

        //	[Display(Name = "ProductList")]
        //	public int ProductListID { get; set; }
		

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date")]
        public DateOnly Date { get; set; }  // = DateTime.Today;


        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Time")]
        public TimeOnly Time { get; set; } // = DateTime.Now;


        [DataType(DataType.Currency)]
		[Required]
		// [Column(typeName = "decimal(18,2)")]
		[Precision(18, 2)]  //  O precision vai truncar os numeros abaixo e acima de 18 e 2  
		[Display(Name = "Totals")]  // este é o total por produto e não o total de todos os produtos
		public decimal TotalPrice { get; set; }   // usado para dinheiro com valor real




		[DataType(DataType.MultilineText)]
		[Display(Name = "Observations")]
		public string? Observations { get; set; }   // ? para ter sempre algo preenchido alternativa ... { get; set;} = "" =string.Empty;





        // Var para indicar se o valor da encomenda está pago 
        [Required]
		[Display(Name = "Is Paid")]
		public bool IsPaid { get; set; } = false;  // por defeito não está pago


		// Não vou associar por enquanto o vendedor à venda realizada (esta tabela nao é pedida no enunciado, nao vou criar)
		//[ForeignKey("StaffID")]
		//[ValidateNever]
		//public Staff Staff { get; set; }

		//[Display(Name = "Staff")]
		//[Required]
		//public int StaffID { get; set; }




	}
}


// 				"Identificador da venda (ex: V2024/A/001)",
//"Data",
//				"Hora",
//				"Cliente",
//				"Produtos vendidos (lista de produtos adicionar, qtd e preço unit.)",
//				"Observações",
//				"Valor final",
//				"Estado [Encomendada, Em processamento, Processada, Enviada]",
//				"Paga [Sim ou Não]"

