// Aqui tenho o sistema de unidades indexado, Kg, metro, Unidades, Litros, etc
// a cada produto na loja é associado uma unidade usada para a transação do mesmo
// por exemplo os caracois podem ser vendidos na vida quotidiana ao litro ou ao Kilo
// na loja só podem ser vendido usando uma unica unidade de medida definida quando se cria o produto
// ...
namespace CursoMod165.Models
{
	// o system Unity funciona como uma tabela
	// mas como queremos restringir a quantidade de unidades 
	// faz-se a mesma aqui, não sendo possivel a qualquer utilizador adicionar, apagar, etc
	public enum SystemUnity
	{
		Unity = 1,
		Kilo = 2,
		Meter = 3,
		Liter = 4,
	}
}
