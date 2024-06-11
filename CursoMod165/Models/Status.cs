// aqui não temos nenhum abase de dados mas sim uma lista enumerada
// Não é necessario criar schema
namespace CursoMod165.Models
{
	public enum Status  // é um enumeravel em vez de class
	{
		Ordered = 1,   // On_Order
		Purchase_Process = 2,
		Package = 3,
		Sent = 4,
		
	}
}
