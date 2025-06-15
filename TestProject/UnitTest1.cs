namespace TestProject;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        // Arrange
        var expected = 5;
        
        // Act
        var actual = 2 + 3; // Example operation
        
        // Assert
        Assert.Equal(expected, actual);
    }
}