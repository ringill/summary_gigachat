using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

public static class MainClass
{
    // Настройки API
    const string API_URL = "https://gigachat.devices.sberbank.ru/api/v1/chat/completions";
    const string API_KEY = "eyJjdHkiOiJqd3QiLCJlbmMiOiJBMjU2Q0JDLUhTNTEyIiwiYWxnIjoiUlNBLU9BRVAtMjU2In0.AAti29ksq350P_iIHrpXECd2Za_GX7S_UkPmL_w3XVxNYXG68GQxj43ZQCmTHUrlzyXnPm0keAD9WwN0GKDTQSmjWtyJBeofKOHk3tAdKs2pqioVtFjYZ-BcJCGLlV6wJ0nf4-xyELsuKQDXt9j1DpdZeFEYXMl75q5vpaJkU0WObup6mGr6Zn__Vi9tBEU68WoIBMJBVphighwf2fm8-iITVkL1BeAV9h1JvYOjndlQ7TqzEhUDLeaOTWmro7BC5U_G4QO-qcmHOTLoyMSAjbOGnA5Xs5pwaR8kp98iuOwrFzuQBxRYeLzOKyY8yIWoYFNyITp6oC1JtgS9OjRQzw.vBF6aftAeAbWPqj6ahYZbg.OYlfhxayG913wQVAyMDmqRiFZd3J4sqKejsMpNH7TAoTOixVPG5gaYiuTiWQcOw0u3fBjOXwf7A4sGghcXZ6MkL1Z5ZmjKr3fKY61gI2WKrr_sOc5sLlWXsf-WJ2KdHY6QZSDnboUqx4dR6hALzaFlDEQU7W6la0Et0DM4zbL1K1F85lOF1XdfHAP8E4BZxRglddqFBv87K0hedrLksQFgl8EcEjBxSAhwogPBJBRs8kTTKIjBhHBCCDUSgaRfwf2Nh3BZxfqhBDc_WFNIJqR3lIUylheGQWUKaaYN5k7wojLQSv4qetnz6HvyS-W6MpTuWPiq55dKzyi9X5tzKxknFvqK-zxIN3hC4A6Nfpodmt9BOB6LD6bBMdbtIJuMJZNEusuEVZ7WH9yW3WfEVhk2qdQKBDgQ1XYTG5xgGAkh1Bi5dlETJxu3XtNA4AIII7H6unS2CnYdmN99iaSRI1wzK8jmNy_SB3DEr6frdQZUcyzh3bVAe9ma5IKGlnp76ZorGzhsn0xmKl1v_VK2u0lf_GErn-TmVdOkfMDLYS21FnfD4U6FSCxwqx6b1huHnoErO2fY5ssyhDdQ7AiGUWLgCnhZyVgZSMEHttPDZDWgnMzenx5leqaoqeuUcQFWHdc924nUMdrj9wQpfUNqpVCAYVTIrq3epmdmrW4XyVjaRJqlpTezOaZ_W3uWoHZUcXqCxhZ96DZ9dBQeeGwHHCfVjRCJBz5OdeNMnV_RN06-E.kLPLaLdFk_GfQTPOIQ9l6b9GHdBQYyiZ6VaUIZxaJVg";

    public static void Main()
    {
        var documentationGenerator = new DocumentationGenerator(API_URL, API_KEY);
        // Обработка всех файлов в проекте
        var files = Directory.GetFiles("../../../../", "*Tests.cs", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            documentationGenerator.ProcessFile(file).GetAwaiter().GetResult();
        }
    }
}
