using System.ComponentModel.DataAnnotations;

namespace CursoMod165.Models
{
	// EntityFramework -> biblioteca de conecxao à base de dados
	public class Category
	{
		public int ID { get; set; }

		// definir formatacao do campo
		[StringLength(50)]
		[Required]
        [Display(Name = "Category")]
        public string Name { get; set; }

		
		[Required]
		public string Description { get; set; }	

	}
}
// Fruta (ou Fruto), parte integrante de algumas plantas. è responsável por proteger e carregar as sementes.
// Alimento Comestível de sabor adocicado. 