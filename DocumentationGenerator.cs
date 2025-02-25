using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;
using System.Text.RegularExpressions;

public class DocumentationGenerator(string API_URL, string API_KEY)
{
  private readonly string _API_URL = API_URL;
  private readonly string _API_KEY = API_KEY;

  public string CreatePrompt(SyntaxNode node)
  {
    return node switch
    {
      ClassDeclarationSyntax cls =>
          $"Сгенерируй XML summary на русском для класса в 1 предложении. " +
          $"Атрибуты: {GetClassAttributes(cls)} ; " +
          $"Имя класса: {cls.Identifier.Text}",

      MethodDeclarationSyntax method =>
          $"Сгенерируй XML summary на русском для метода в 1 предложении. " +
          $"Атрибуты: {GetMethodAttributes(method)} ; " +
          $"Сигнатура: {GetMethodSignature(method)}",

      _ => string.Empty
    };
  }

  // Вспомогательные методы для извлечения информации
  private string GetClassAttributes(ClassDeclarationSyntax cls)
  {
    return cls.AttributeLists
        .SelectMany(al => al.Attributes)
        .Select(a => a.Name.ToString())
        .DefaultIfEmpty("нет атрибутов")
        .Aggregate((a, b) => $"{a}, {b}");
  }

  private string GetMethodAttributes(MethodDeclarationSyntax method)
  {
    return method.AttributeLists
        .SelectMany(al => al.Attributes)
        .Select(a => a.Name.ToString())
        .DefaultIfEmpty("нет атрибутов")
        .Aggregate((a, b) => $"{a}, {b}");
  }

  private string GetMethodSignature(MethodDeclarationSyntax method)
  {
    var parameters = method.ParameterList.Parameters
        .Select(p => $"{p.Type} {p.Identifier}")
        .DefaultIfEmpty("нет параметров")
        .Aggregate((a, b) => $"{a}, {b}");

    return $"{method.ReturnType} {method.Identifier}({parameters})";
  }

  public async Task<string> ProcessApiResponse(HttpResponseMessage response)
  {
    var json = await response.Content.ReadAsStringAsync();
    return Regex.Match(json, "\"content\":\"(.*?)\"").Groups[1].Value;
  }

  public async Task ProcessFile(string filePath)
  {
    try
    {
      var code = File.ReadAllText(filePath);
      var tree = CSharpSyntaxTree.ParseText(code);
      var root = await tree.GetRootAsync();

      var newRoot = root.ReplaceNodes(
          root.DescendantNodes()
              .Where(n => (n is ClassDeclarationSyntax ||
                          (n is MethodDeclarationSyntax method && IsPublicMethod(method)))),
          (original, rewritten) => AddSummary(original));

      if (newRoot != root)
      {
        File.WriteAllText(filePath, newRoot.ToFullString());
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error processing {filePath}: {ex.Message}");
    }
  }

  private SyntaxNode AddSummary(SyntaxNode node)
  {
    return node switch
    {
      ClassDeclarationSyntax cls when !HasSummary(cls) =>
          AddClassSummary(cls),
      MethodDeclarationSyntax method when IsPublicMethod(method) && !HasSummary(method) =>
          AddMethodSummary(method),
      _ => node
    };
  }

  private ClassDeclarationSyntax AddClassSummary(ClassDeclarationSyntax cls)
  {
    var summary = GenerateSummary(cls).Result;
    return cls.WithLeadingTrivia(
        SyntaxFactory.ParseLeadingTrivia($"/// <summary>\n/// {summary}\n/// </summary>\n"));
  }

  private MethodDeclarationSyntax AddMethodSummary(MethodDeclarationSyntax method)
  {
    var summary = GenerateSummary(method).Result;
    var trivia = SyntaxFactory.ParseLeadingTrivia($"/// <summary>\n/// {summary}\n/// </summary\n");

    return method.AttributeLists.Any()
        ? method.ReplaceNode(
            method.AttributeLists.First(),
            method.AttributeLists.First().WithLeadingTrivia(trivia))
        : method.WithLeadingTrivia(trivia);
  }

  private static bool IsPublicMethod(MethodDeclarationSyntax method)
  {
    return method.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword));
  }

  private static bool HasSummary(SyntaxNode node)
  {
    return node.GetLeadingTrivia()
        .Any(t => t.GetStructure() is DocumentationCommentTriviaSyntax);
  }

  private async Task<string> GenerateSummary(SyntaxNode node)
  {
    // Настройка HttpClientHandler для игнорирования проверки SSL
    var handler = new HttpClientHandler
    {
      ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
    };

    using var httpClient = new HttpClient(handler);
    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_API_KEY}");

    var prompt = CreatePrompt(node);
    var response = await httpClient.PostAsync(_API_URL,
        new StringContent($"{{\"model\": \"GigaChat\", \"messages\": [{{\"role\": \"user\", \"content\": \"{prompt}\"}}]}}",
        Encoding.UTF8,
        "application/json"));

    return await ProcessApiResponse(response);
  }
}