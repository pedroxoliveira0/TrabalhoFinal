using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CursoMod165.Models
{
	public class Product
	{

		public int ID { get; set; }

		// definir formatacao dos campos
		[StringLength(255)]
		[Required]
		public string Description { get; set; }




		[DataType(DataType.Currency)]
		[Required]
		// [Column(typeName = "decimal(18,2)")]
		[Precision(18, 2)]  //  O precision vai truncar os numeros abaixo e acima de 18 e 2  
							// truncado = apagado
		public decimal Price { get; set; }   // usado para dinheiro com valor real
											 //[DisplayFormat(DataFormatString = "{0:#,###.00}")]
											 //public float? Price { get; set; }


		// Quantidade do produto em Stock
		// que pode ser em Kg
		[Required]
		[Precision(10, 3)]
		public decimal Quantity { get; set; }



		// peso do produto não faz muito sentido ...
		[Precision(10, 3)]
		public decimal Weight { get; set; }



		// Chave estramgeira para ligar com tabela das categorias
		[ForeignKey("CategoryID")]
		[ValidateNever]
		public Category Category { get; set; }

		[Display(Name = "Category")]
		[Required]
		public int CategoryID { get; set; }


		[StringLength(255)]
		// nao é de preenchimento obrigatorio
		public string LinkFoto { get; set; }

		// criar campo unidades onde coloco qual a unidade usado para transacao do produto
		// kg, un, litros, arrobas etc
		// eventualmente o melhor seria criar uma tabela com as unidades
		
		// vou usar antes 
		// um modelo do tipo speciality para as unidades

		// Para criar uma Unidade de medida; como nos produtos temos de a definir no model SystemUnity
		// Unidades de medida
		[Display(Name = "Unity")]
		[Required]
		public SystemUnity? SystemUnity { get; set; }

	}
}
