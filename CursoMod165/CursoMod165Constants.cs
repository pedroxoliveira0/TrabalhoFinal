namespace CursoMod165
{
    public static class CursoMod165Constants
    {
        public readonly struct USERS
        {
            public readonly struct ADMIN
            {
                public static readonly string USERNAME = "admin";
                public static readonly string PASSWORD = "xpto1234";
            }
            public readonly struct WAREHOUSEMAN
            {
                public static readonly string USERNAME = "warehouse";
                public static readonly string PASSWORD = "12345678";
            }
            public readonly struct VENDOR
            {
                public static readonly string USERNAME = "vendor";
                public static readonly string PASSWORD = "01012024";  // nao pode ser pass repetidas ex:20242024
            }

        }
        // ver como copiar role administrative tempo 2 -> 00:36; 00:58
        // existe um administrativo mas este não tem role, como fazere, copiar diretamente na DB
        public readonly struct ROLES
        {
            public const string ADMIN = "ADMIN";
            public static readonly string WAREHOUSEMAN = "WAREHOUSEMAN";
            public static readonly string VENDOR = "VENDOR";
            public static readonly string ASSISTANT = "ASSISTANT";
        }
    
    // PARA EVITAR FAZER MUITAS CONSTANTES STRING, CRIAMOS UMA POLITICA DE STRINGS
    public readonly struct POLICIES 
        {

            public readonly struct APP_POLICY
            {
                public const string NAME = "APP_POLICY";
                public static readonly string[] APP_POLICY_ROLES =
                {
                    ROLES.ADMIN,
                    ROLES.VENDOR,   //  Testar se desaparece duplo sale 
                    ROLES.WAREHOUSEMAN,
                    ROLES.ASSISTANT,
                };
            }
            public readonly struct APP_POLICY_VENDOR
            {
                public const string NAME = "APP_POLICY_VENDOR";
                public static readonly string[] APP_POLICY_ROLES =
                {
                    ROLES.ADMIN, 
                    ROLES.VENDOR,
                };
            }

            public readonly struct APP_POLICY_WAREHOUSEMAN
            {


                public const string NAME = "APP_POLICY_WAREHOUSEMAN";
                public static readonly string[] APP_POLICY_ROLES =
                {
                    ROLES.ADMIN,
                    ROLES.WAREHOUSEMAN,

                };

            }
            public readonly struct APP_POLICY_ADMIN
            {

            
                public const string NAME = "APP_POLICY_ADMIN";
                public static readonly string[] APP_POLICY_ROLES =
                {
                    ROLES.ADMIN,
                    
                };

            }
        }
    
    }
}
