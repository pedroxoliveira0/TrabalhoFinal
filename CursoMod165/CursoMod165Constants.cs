﻿namespace CursoMod165
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
            public readonly struct ADMINISTRATIVE
            {
                public static readonly string USERNAME = "administrative";
                public static readonly string PASSWORD = "12345678";
            }
            public readonly struct DRIVER
            {
                public static readonly string USERNAME = "driver";
                public static readonly string PASSWORD = "01012024";  // nao pode ser pass repetidas ex:20242024
            }

        }
        // ver como copiar role administrative tempo 2 -> 00:36; 00:58
        // existe um administrativo mas este não tem role, como fazere, copiar diretamente na DB
        public readonly struct ROLES
        {
            public const string ADMIN = "ADMIN";
            public static readonly string ADMINISTRATIVE = "ADMINISTRATIVE";
            public static readonly string DRIVER = "DRIVER";
            public static readonly string HEALTH_STAFF = "HEALTH_STAFF";
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
                    ROLES.ADMINISTRATIVE,
                    ROLES.DRIVER,
                    ROLES.HEALTH_STAFF,
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
