using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using System.Threading.Tasks;

public static class DocumentationGenerator
{
  public static async Task ProcessFile(string filePath)
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

  private static SyntaxNode AddSummary(SyntaxNode node)
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

  private static ClassDeclarationSyntax AddClassSummary(ClassDeclarationSyntax cls)
  {
    var summary = GenerateSummary(cls).Result;
    return cls.WithLeadingTrivia(
        SyntaxFactory.ParseLeadingTrivia($"/// <summary>\n/// {summary}\n/// </summary>\n"));
  }

  private static MethodDeclarationSyntax AddMethodSummary(MethodDeclarationSyntax method)
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

  private static async Task<string> GenerateSummary(SyntaxNode node)
  {
    // Реализация генерации через API
    return node switch
    {
      ClassDeclarationSyntax cls => $"Tests for {cls.Identifier.Text.Replace("Tests", "")} functionality",
      MethodDeclarationSyntax method => $"Tests {NormalizeMethodName(method.Identifier.Text)}",
      _ => string.Empty
    };
  }

  private static string NormalizeMethodName(string methodName)
  {
    return string.Join(" ", methodName.Split('_', StringSplitOptions.RemoveEmptyEntries));
  }
}