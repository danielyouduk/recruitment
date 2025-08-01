using JobManagement.Domain.Entities;
using JobManagement.Domain.ValueObjects;

namespace JobManagement.Domain.Tests.ValueObjects;

public class JobDetailsTests
{
    #region Constructor Tests
    
    [Fact]
    public void Constructor_ShouldTrimWhitespace()
    {
        // Arrange
        var title = "  Software Engineer  ";
        var description = "  Build amazing software  ";
        
        // Act
        var jobDetails = new JobDetails(title, description);
        
        // Assert
        Assert.Equal("Software Engineer", jobDetails.Title);
        Assert.Equal("Build amazing software", jobDetails.Description);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_ShouldThrow_WhenTitleIsNullOrWhitespace(string invalidTitle)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new JobDetails(invalidTitle));
        Assert.Equal("Title cannot be null or empty (Parameter 'title')", exception.Message);
    }
    
    [Fact]
    public void Constructor_ShouldThrow_WhenTitleExceedsMaxLength()
    {
        // Arrange
        var longTitle = new string('A', JobDetails.MaxTitleLength + 1);
        
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new JobDetails(longTitle));
        Assert.Equal($"Title cannot exceed {JobDetails.MaxTitleLength} characters (Parameter 'title')", exception.Message);
    }
    
    [Fact]
    public void Constructor_ShouldThrow_WhenDescriptionExceedsMaxLength()
    {
        // Arrange
        var title = "Software Engineer";
        var longDescription = new string('A', JobDetails.MaxDescriptionLength + 1);
        
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new JobDetails(title, longDescription));
        Assert.Equal($"Description cannot exceed {JobDetails.MaxDescriptionLength} characters (Parameter 'description')", exception.Message);
    }

    [Fact]
    public void Constructor_ShouldAccept_MaxLengthTitle()
    {
        // Arrange
        var maxLengthTitle = new string('A', JobDetails.MaxTitleLength);
        
        // Act & Assert
        var jobDetails = new JobDetails(maxLengthTitle);
        Assert.Equal(maxLengthTitle, jobDetails.Title);
    }

    [Fact]
    public void Constructor_ShouldAccept_MaxLengthDescription()
    {
        // Arrange
        var title = "Software Engineer";
        var maxLengthDescription = new string('A', JobDetails.MaxDescriptionLength);
        
        // Act & Assert
        var jobDetails = new JobDetails(title, maxLengthDescription);
        Assert.Equal(maxLengthDescription, jobDetails.Description);
    }
    
    #endregion
    
    #region UpdateDetails Tests
    
    [Fact]
    public void UpdateDetails_ShouldThrow_WhenNewDetailsIsNull()
    {
        // Arrange
        var job = new Job(new JobDetails("Software Engineer"));

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => job.UpdateDetails(null));
        Assert.Equal("newDetails", exception.ParamName);
    }
    
    [Fact]
    public void UpdateDetails_ShouldThrow_WhenJobIsClosed()
    {
        // Arrange
        var job = new Job(new JobDetails("Software Engineer"));
        job.CreateDraft();
        job.Post();
        job.Close();

        var newDetails = new JobDetails("New Title");

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => job.UpdateDetails(newDetails));
        Assert.Equal("Cannot update details of a closed job", exception.Message);
    }
    
    [Fact]
    public void UpdateDetails_ShouldUpdateTitleAndDescription_WhenJobIsDraft()
    {
        // Arrange
        var initialDetails = new JobDetails("Software Engineer");
        var job = new Job(initialDetails);
    
        var newDetails = new JobDetails("Senior Software Engineer", "Build amazing software with our team");
    
        // Act
        job.UpdateDetails(newDetails);
    
        // Assert
        Assert.Equal("Senior Software Engineer", job.Details.Title);
        Assert.Equal("Build amazing software with our team", job.Details.Description);
    }

    [Fact]
    public void UpdateDetails_ShouldUpdateOnlyTitle_WhenDescriptionIsNull()
    {
        // Arrange
        var job = new Job(new JobDetails("Software Engineer", "Old description"));
        var newDetails = new JobDetails("Senior Software Engineer");
    
        // Act
        job.UpdateDetails(newDetails);
    
        // Assert
        Assert.Equal("Senior Software Engineer", job.Details.Title);
        Assert.Null(job.Details.Description);
    }
    
    [Fact]
    public void UpdateDetails_ShouldThrow_WhenJobIsActive()
    {
        // Arrange
        var job = new Job(new JobDetails("Software Engineer"));
        job.CreateDraft();
        job.Post();
    
        var newDetails = new JobDetails("New Title");
    
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => job.UpdateDetails(newDetails));
        Assert.Equal("Cannot update details of an active job", exception.Message);
    }

    [Fact]
    public void UpdateDetails_ShouldThrow_WhenJobIsDeleted()
    {
        // Arrange
        var job = new Job(new JobDetails("Software Engineer"));
        job.Delete();
    
        var newDetails = new JobDetails("New Title");
    
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => job.UpdateDetails(newDetails));
        Assert.Equal("Cannot update details of a deleted job", exception.Message);
    }
    
    #endregion
}