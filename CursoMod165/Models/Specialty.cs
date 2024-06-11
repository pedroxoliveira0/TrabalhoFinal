namespace CursoMod165.Models
{
    // Em vez de uma tabela vamos fazer uma "list com descrição fixa"
    public enum Specialty  // é um enumeravel em vez de class
    {
        Generalist = 1,
        Orthodontist = 2,
        Pediatric = 3,
        Surgeon = 4,

    }


    // agora é preciso fazer o seguinte:
    // ADD-MIGRATION AddedSpecialtyToStaff 
    // depois
    // Update-database
}
