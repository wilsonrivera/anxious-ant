namespace AnxiousAnt.IO;

public class FileUtilsTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public void WriteText_ShouldThrowWhenFilenameIsNullEmptyOrWhiteSpace(string? input)
    {
        // Arrange
        Action act = () => FileUtils.WriteText(input!, "test");

        // Assert
        act.ShouldThrow<ArgumentException>();
    }

    [Fact]
    public void WriteText_ShouldThrowWhenContentIsNull()
    {
        // Arrange
        Action act = () => FileUtils.WriteText("test", null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void WriteText_ShouldThrowWehnEncodingIsNull()
    {
        // Arrange
        Action act = () => FileUtils.WriteText("test", "test", null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void WriteText_ShouldWriteTextToFile()
    {
        // Arrange
        var filename = Path.GetTempFileName();

        // Act
        FileUtils.WriteText(filename, "test", Encoding.UTF8);

        // Assert
        File.Exists(filename).ShouldBeTrue();
        File.ReadAllText(filename).ShouldBe("test");
        File.Delete(filename);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public void SafeWrite_ShouldThrowWhenFilenameIsNullEmptyOrWhiteSpace(string? input)
    {
        // Arrange
        Action act = () => FileUtils.SafeWrite(input!, "test");

        // Assert
        act.ShouldThrow<ArgumentException>();
    }

    [Fact]
    public void SafeWrite_ShouldThrowWhenContentIsNull()
    {
        // Arrange
        Action act = () => FileUtils.SafeWrite("test", null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void SafeWrite_ShouldCreateFileWhenItDoesNotExists()
    {
        // Arrange
        var filename = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        // Act
        File.Exists(filename).ShouldBeFalse();
        FileUtils.SafeWrite(filename, "test");

        // Assert
        File.Exists(filename).ShouldBeTrue();
        File.ReadAllText(filename).ShouldBe("test");
        File.Delete(filename);
    }

    [Fact]
    public void SafeWrite_ShouldReplaceExistingFile()
    {
        // Arrange
        var filename = Path.GetTempFileName();

        // Act
        FileUtils.WriteText(filename, "test");
        FileUtils.SafeWrite(filename, "test2");

        // Assert
        File.Exists(filename).ShouldBeTrue();
        File.ReadAllText(filename).ShouldBe("test2");
        File.Delete(filename);
    }

    [Fact]
    public void SafeWrite_Should()
    {
        // Arrange
        var filename = Path.GetTempFileName();

        // Act
        FileUtils.WriteText(filename, "test");
        using var stream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None);

        FileUtils.SafeWrite(filename, "test2");

        // Assert
        File.Exists(filename).ShouldBeTrue();
        File.ReadAllText(filename).ShouldBe("test2");
        File.Delete(filename);
    }
}