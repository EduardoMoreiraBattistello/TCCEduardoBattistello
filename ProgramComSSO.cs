using Microsoft.Identity.Client;
using System;
using System.Threading.Tasks;

class Program
{
    // ***** ATENÇÃO: PARA FINS DE TESTE APENAS *****
    // Em um ambiente de produção, não incorpore Client IDs e outros segredos diretamente no código.
    // Use variáveis de ambiente, Azure Key Vault ou outras soluções de gerenciamento de segredos.
    private const string ClientId = "7da6e3fe-10eb-4665-96dd-18b36165b896"; // <-- COLOQUE SEU CLIENT ID AQUI!
    private const string TenantId = "13a1be0f-2450-4521-8f23-0b6c875aef62";
    private static readonly string[] Scopes = { "openid", "offline_access", "User.Read" }; // Scopes necessários
    // ***********************************************

    static async Task Main(string[] args)
    {
        Console.WriteLine("Escolha o modo de autenticação:");
        Console.WriteLine("1 - Entra External ID");
        Console.WriteLine("2 - Login Local");
        Console.Write("Opção: ");
        var opcao = Console.ReadLine();

        if (opcao == "1")
        {
            await AutenticacaoEntraExternalID();
        }
        else if (opcao == "2")
        {
            // Assumo que LoginHandler.LoginLocal() está em outro arquivo ou classe
            // e não precisa de ajustes para esta parte da autenticação externa.
            LoginHandler.LoginLocal(); 
        }
        else
        {
            Console.WriteLine("❌ Opção inválida.");
        }
    }

    static async Task AutenticacaoEntraExternalID()
    {
        // As variáveis de ambiente não são mais necessárias pois os valores estão embutidos acima.
        // var clientId = Environment.GetEnvironmentVariable("AZURE_CLIENT_ID");
        // var scopeRaw = Environment.GetEnvironmentVariable("AZURE_SCOPE");

        // O ClientId já está definido como constante acima
        if (string.IsNullOrWhiteSpace(ClientId)) 
        {
            Console.WriteLine("❌ O ClientId não foi configurado no código. Por favor, preencha a constante 'ClientId'.");
            return;
        }

        // Authority ajustada para o seu Tenant ID principal
        var authority = $"https://login.microsoftonline.com/{TenantId}/";

        var app = PublicClientApplicationBuilder.Create(ClientId) // Usando a constante ClientId
            .WithAuthority(authority)
            // Removendo o WithRedirectUri para o Device Code Flow
            // .WithRedirectUri("http://localhost") 
            .WithLogging((level, message, containsPii) =>
            {
                if (!containsPii)
                    Console.WriteLine($"[MSAL:{level}] {message}");
            }, LogLevel.Verbose, enablePiiLogging: false, enableDefaultPlatformLogging: true)
            .Build();

        // Os scopes já estão definidos como um array de string constante acima.
        // string[] scopes = scopeRaw.Split(" ", StringSplitOptions.RemoveEmptyEntries);

        try
        {
            var result = await app.AcquireTokenWithDeviceCode(Scopes, callback => // Usando o array Scopes
            {
                Console.WriteLine($"🔐 Para autenticar, acesse: {callback.VerificationUrl}");
                Console.WriteLine($"🔑 Digite o código: {callback.UserCode}");
                return Task.CompletedTask;
            }).ExecuteAsync();

            Console.WriteLine($"✅ Autenticação bem-sucedida!");
            Console.WriteLine($"✅ Token de Acesso (primeiros 10 caracteres): {result.AccessToken.Substring(0, 10)}..."); 
            Console.WriteLine($"✅ Nome do usuário: {result.Account.Username}");
            Console.WriteLine($"✅ ID do Objeto do usuário: {result.Account.HomeAccountId.ObjectId}");
        }
        catch (MsalServiceException ex)
        {
            Console.WriteLine($"❌ MSAL Service Error: {ex.Message}");
            Console.WriteLine($"❌ Error Code: {ex.ErrorCode}"); 
            Console.WriteLine($"❌ Status HTTP: {ex.StatusCode}");
            Console.WriteLine($"❌ Response Body: {ex.ResponseBody}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro geral: {ex.Message}");
        }
    }
}

// Assumindo que LoginHandler.cs existe ou você irá criá-lo
/*
public static class LoginHandler
{
    public static void LoginLocal()
    {
        Console.WriteLine("Realizando login local...");
        // Adicione aqui a lógica para o seu login local
    }
}
*/