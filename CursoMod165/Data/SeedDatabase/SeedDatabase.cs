using CursoMod165;
using Microsoft.AspNetCore.Identity;
using static CursoMod165.CursoMod165Constants.USERS;

namespace CursoMod165.Data.SeedDatabase
{
    // este file é o de inicializações, vars, variaveis definicoes etc.
    public class SeedDatabase
    {

            public static void Seed(ApplicationDbContext context,
                                    UserManager<IdentityUser> userManager,
                                    RoleManager<IdentityRole> roleManager)
            {
                SeedRoles(roleManager).Wait();
                SeedUsers(userManager).Wait(); // faz uma pausa/espera  - este metodo é assincrono ...
            }
            private static async Task SeedUsers(UserManager<IdentityUser> userManager) // metodo assincrono "async"
            {
                var dbAdmin = await userManager.FindByNameAsync(CursoMod165Constants.USERS.ADMIN.USERNAME); // era admin para ao que está existente vamos ver se a base de dados tem algum user admin

                if (dbAdmin == null)
                {
                    // aqui vamos fazer ciclo para criar registos de utilizadores
                    IdentityUser userAdmin = new IdentityUser()
                    {
                        UserName = CursoMod165Constants.USERS.ADMIN.USERNAME  // Admin passa para CursoMod165Constants para não haver erros de typing, deve-se ter as constantes corresponentes aos users 

                    };
                    var result = await userManager.CreateAsync(userAdmin, CursoMod165Constants.USERS.ADMIN.PASSWORD);  // await espera, se admin nao existe entao cria.

                    if (result.Succeeded == true)
                    {
                        dbAdmin = await userManager.FindByNameAsync(CursoMod165Constants.USERS.ADMIN.USERNAME);
                        await userManager.AddToRoleAsync(dbAdmin!, CursoMod165Constants.ROLES.ADMIN); // ! pk sabemos que existe, foi criado no passo anterior
                    }

            }


                var dbAdministrative = await userManager.FindByNameAsync(CursoMod165Constants.USERS.ADMINISTRATIVE.USERNAME); // era admin para ao que está existente vamos ver se a base de dados tem algum user admin

                if (dbAdministrative == null)
                {
                    // aqui vamos fazer ciclo para criar registos de utilizadores
                    IdentityUser userAdministrative = new IdentityUser()
                    {
                        UserName = CursoMod165Constants.USERS.ADMINISTRATIVE.USERNAME  // Admin passa para CursoMod165Constants para não haver erros de typing, deve-se ter as constantes corresponentes aos users 
                    };
                    var result = await userManager.CreateAsync(userAdministrative, CursoMod165Constants.USERS.ADMINISTRATIVE.PASSWORD);  // await espera, se admin nao existe entao cria.
                
                    // aqui cria utilizador e atribui um role ...
                    if (result.Succeeded== true)
                    {
                        dbAdministrative = await userManager.FindByNameAsync(CursoMod165Constants.USERS.ADMINISTRATIVE.USERNAME);
                        await userManager.AddToRoleAsync(dbAdministrative!, CursoMod165Constants.ROLES.ADMINISTRATIVE); // ! pk sabemos que existe, foi criado no passo anterior
                    }
                }


                var dbDriver = await userManager.FindByNameAsync(CursoMod165Constants.USERS.DRIVER.USERNAME);

                if (dbDriver == null)
                {

                    // aqui vamos fazer ciclo para criar registos de utilizadores
                    IdentityUser userDriver = new IdentityUser()
                    {
                        UserName = CursoMod165Constants.USERS.DRIVER.USERNAME  // Admin passa para CursoMod165Constants para não haver erros de typing, deve-se ter as constantes corresponentes aos users 
                    };
                    var result = await userManager.CreateAsync(userDriver, CursoMod165Constants.USERS.DRIVER.PASSWORD);  // await espera, se admin nao existe entao cria.
                

                    if (result.Succeeded == true)
                    {
                        dbDriver = await userManager.FindByNameAsync(CursoMod165Constants.USERS.DRIVER.USERNAME);
                        await userManager.AddToRoleAsync(dbDriver!, CursoMod165Constants.ROLES.DRIVER); // ! pk sabemos que existe, foi criado no passo anterior
                    }
                }



        }
        
        // ADMIN
        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            // não queremos usar strings diretamente "ADMIN" para protecao, passamos para cosntantes 
            var roleCheck = await roleManager.RoleExistsAsync(CursoMod165Constants.ROLES.ADMIN);  // lOGO COLOcamos nas constantes

            if (!roleCheck) 
            {
                var adminRole = new IdentityRole
                {
                    Name = CursoMod165Constants.ROLES.ADMIN
                };

                await roleManager.CreateAsync(adminRole);
            }
        

        // ADMINISTRATIVE
       
            // não queremos usar strings diretamente "ADMIN" para protecao, passamos para cosntantes 
            roleCheck = await roleManager.RoleExistsAsync(CursoMod165Constants.ROLES.ADMINISTRATIVE);  // lOGO COLOcamos nas constantes

            if (!roleCheck)
            {
                var administrativeRole = new IdentityRole
                {
                    Name = CursoMod165Constants.ROLES.ADMINISTRATIVE
                };

                await roleManager.CreateAsync(administrativeRole);
            }
        

        // DRIVER
        
            // não queremos usar strings diretamente "ADMIN" para protecao, passamos para cosntantes 
            roleCheck = await roleManager.RoleExistsAsync(CursoMod165Constants.ROLES.DRIVER);  // lOGO COLOcamos nas constantes

            if (!roleCheck)
            {
                var driverRole = new IdentityRole
                {
                    Name = CursoMod165Constants.ROLES.DRIVER
                };

                await roleManager.CreateAsync(driverRole);
            }
        

        // HEALTH_STAFF
       
            // não queremos usar strings diretamente "ADMIN" para protecao, passamos para cosntantes 
            roleCheck = await roleManager.RoleExistsAsync(CursoMod165Constants.ROLES.HEALTH_STAFF);  // lOGO COLOcamos nas constantes

            if (!roleCheck)
            {
                var health_staffRole = new IdentityRole
                {
                    Name = CursoMod165Constants.ROLES.HEALTH_STAFF
                };

                await roleManager.CreateAsync(health_staffRole);  // no main project é "healthstaffRole"
            }

        }


       

    }
}

