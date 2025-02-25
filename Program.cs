using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;


public static class MainClass
{
    // Настройки API
    const string API_URL = "https://api.deepseek.com/v1/chat/completions";
    const string API_KEY = "eyJjdHkiOiJqd3QiLCJlbmMiOiJBMjU2Q0JDLUhTNTEyIiwiYWxnIjoiUlNBLU9BRVAtMjU2In0.VSnREzokRiuGTlWipGy60sfaXQX4ufLE4Nd8sCxfQ1g365nIyvUjgzZM-uJ1tdRfI7zq0sQ_BJU3GfpLl1P4nPayYPa6lBqadI-fqeUG0GUpd4M5NvCZ0ZdauiwrTQhDPIIyS8ufb3rxseTxT7-70QSV2stA1TE3dAgluq62y0pRTzIdsa5hYebIDTk6JDkbP9aPnmDl4WsVCUGjP-TEJ2_vaR7CaOb5lM6EPsqbZIgteLiUpBeXeyaknn4ib7GrchXoSe2PQZzYsJ1bhaN9cZ0Y4TE_q7i6nkOLULhEJ3eoXqAEFZISIKGa50Vth-7MEkFN569lseL-2jpeu9L6PA.gIzIBwVay4pvwy2E9YIVDw.Paaypa_CKeK8ID-D101kAEm8o34vDYQ4GuBLwcAM7rKCQHttzJx4bp8dB4rgi9lvB9NZjT5K_bRN4otATlIwMeST5Hmbxy0LyFGakra6HU1P_ElcjdPmo8Yg8KUIh-NRQGTJ7glmbXKbppku3snoUnoab_7oSbqbmpt_PQ12ImnhWMYFvcTO9Mktfy_YgwQmLSeqkKj4K1Q6s4RoqMBCPrF0hPWt1j8kMQhEIvvPNgifRAoVu4R_suvzcJcI8C6leYvGKz6k6xAM7px37xunh5glHuJdnKRfrrzPW7TNLTPBmo85QRMH3ds9BPhQYTg1-zLbkix_xqEGb7lYhDnZAEHndLMxb6uPQMyJW9SlUpLQ6mOnYLC-wyMlHXU2I3uhncnq03jNzqM4PgyzDuLw8Pf2U6a15z-TcrTXLb9vy46rC7re6X0T6bA-XFwM9NNXVhhU5HnF9_yXN88iq9ublkwki60uD33oYnftFcOjuxe3Out_rMsczqG541X31nFzRn_aVbN9TyIJ4twVaJgUg8dnOnl27q92q1V0DRBD9LJTrgHXYi7IcDZskXmBcbp6ypsOgxKH8lnWBQ4QyZ1_OxKbB5ymnLlUbMbacNMd4gl_U7iqhpJVnjyTeHafBGnS-oARx4G3sCScjKMjP9hw74kDr87_Ra1utoWfRlGinlTpPZQp2FwzNONo0E8wRmxMOM4Bl7oTZ3hCjTvFpppc43-Mb9abhm-Hq1VKfIbl7kw._MLly9HePsZoJ9duDebpRDrZs7iasrfCvGpaBHAcs3Y";

    public static void Main()
    {
        // See https://aka.ms/new-console-template for more information
        Console.WriteLine("Hello, World!");

        // Обход всех .cs файлов в проекте
        var files = Directory.GetFiles("../../", "*.cs", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            var code = File.ReadAllText(file);
            var tree = CSharpSyntaxTree.ParseText(code);
            var root = tree.GetRoot();

            // Модифицируем синтаксическое дерево
            var newRoot = root.ReplaceNodes(
                root.DescendantNodes()
                    .Where(n => n.IsKind(SyntaxKind.ClassDeclaration) ||
                               n.IsKind(SyntaxKind.MethodDeclaration)),
                (original, rewritten) => AddSummary(original));

            // Сохраняем изменения
            if (newRoot != root)
            {
                File.WriteAllText(file, newRoot.ToFullString());
            }
        }

    }

    public static SyntaxNode AddSummary(SyntaxNode node)
    {
        if (node.HasSummaryComment()) return node; // Пропускаем если уже есть комментарий

        var comment = GenerateSummary(node).Result;
        return node.WithLeadingTrivia(
            SyntaxFactory.ParseLeadingTrivia($"/// <summary>\n/// {comment}\n/// </summary>\n"));
    }

    public static async Task<string> GenerateSummary(SyntaxNode node)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {API_KEY}");

        var prompt = SummaryGigachatHelper.CreatePrompt(node);
        var response = await httpClient.PostAsync(API_URL,
            new StringContent($"{{\"model\": \"deepseek\", \"messages\": [{{\"role\": \"user\", \"content\": \"{prompt}\"}}]}}",
            Encoding.UTF8,
            "application/json"));

        return await SummaryGigachatHelper.ProcessApiResponse(response);
    }
}

public static class SummaryGigachatHelper
{

    public static string CreatePrompt(SyntaxNode node)
    {
        return node switch
        {
            ClassDeclarationSyntax cls =>
                $"Generate brief XML summary for C# test class in 1 sentence. Class: {cls.Identifier}",
            MethodDeclarationSyntax method =>
                $"Generate concise XML summary for NUnit test method in 1 sentence. Method: {method.Identifier}",
            _ => throw new NotSupportedException()
        };
    }

    public static async Task<string> ProcessApiResponse(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        return Regex.Match(json, "\"content\":\"(.*?)\"").Groups[1].Value;
    }

    public static bool HasSummaryComment(this SyntaxNode node)
    {
        return node.GetLeadingTrivia()
            .Any(t => t.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia));
    }
}