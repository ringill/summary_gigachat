using NUnit.Framework;

public class CalculatorTests
{
  [Test]
  public void Add_TwoNumbers_ReturnsSum()
  {
    // Test logic
  }

  [Test]
  [Category("Math")]
  [Description("Проверка сложения чисел")]
  public void Add_TwoNumbers_ReturnsSum(int a, int b)
  {
    // ...
  }
}

public class CalculatorTests1
{

  [Test]
  public void Add_TwoNumbers_ReturnsSum1()
  {
    // Test logic
  }
}