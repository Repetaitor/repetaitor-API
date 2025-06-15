namespace TestProject;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        // Arrange
        var expected = 5+6;
        
        // Act
        var actual = 11; // Example operation
        
        // Assert
        Assert.Equal(expected, actual);
    }
}