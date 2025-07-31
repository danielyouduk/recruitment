using JobManagement.Domain.Entities;
using JobManagement.Domain.ValueObjects;

namespace JobManagement.Domain.Tests.Entities;

public class JobTests
{
    [Fact]
    public void Job_ShouldInitializeWithCorrectDefaults()
    {
        // Arrange
        var details = new JobDetails("Software Engineer");
    
        // Act
        var job = new Job(details);
    
        // Assert
        Assert.Equal(details, job.Details);
        Assert.Equal(JobStatus.Draft, job.Status);
        Assert.False(job.IsDeleted);
        Assert.True(job.CreatedAt <= DateTime.UtcNow);
        Assert.True(job.CreatedAt >= DateTime.UtcNow.AddSeconds(-1));
    }
    
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
    
    [Fact]
    public void Job_ShouldInitialize_WithDraftStatus()
    {
        // Arrange & Act
        var details = new JobDetails("Test Job");
        var job = new Job(details);
        
        // Assert
        Assert.Equal(JobStatus.Draft, job.Status);
    }
    
    [Fact]
    public void Post_ShouldChangeStatus_FromDraftToActive()
    {
        // Arrange
        var details = new JobDetails("Software Engineer");
        var job = new Job(details);
    
        // Act
        job.Post();
    
        // Assert
        Assert.Equal(JobStatus.Active, job.Status);
    }

    [Fact]
    public void Post_ShouldThrow_WhenJobAlreadyActive()
    {
        // Arrange
        var details = new JobDetails("Software Engineer");
        var job = new Job(details);
        job.Post();
        
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => job.Post());
        Assert.Equal("Job is already posted", exception.Message);
    }
    
    [Fact]
    public void Post_ShouldThrow_WhenJobIsClosed()
    {
        // Arrange
        var details = new JobDetails("Software Engineer");
        var job = new Job(details);
        job.Post();
        job.Close();
    
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => job.Post());
        Assert.Equal("Cannot post a closed job", exception.Message);
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
    
    [Fact]
    public void Close_ShouldThrow_WhenJobAlreadyClosed()
    {
        // Arrange
        var details = new JobDetails("Software Engineer");
        var job = new Job(details);
        job.Post();
        job.Close();
    
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => job.Close());
        Assert.Equal("Job is already closed", exception.Message);
    }
    
    [Fact]
    public void Close_ShouldThrow_WhenJobIsDraft()
    {
        // Arrange
        var details = new JobDetails("Software Engineer");
        var job = new Job(details);
    
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => job.Close());
        Assert.Equal("Cannot close a draft job. Delete it instead.", exception.Message);
    }
    
    [Fact]
    public void Delete_ShouldThrow_WhenJobIsActive()
    {
        // Arrange
        var details = new JobDetails("Software Engineer");
        var job = new Job(details);
        job.Post();
    
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => job.Delete());
        Assert.Equal("Cannot delete an active job. Close it first.", exception.Message);
    }

    [Fact]
    public void Delete_ShouldThrow_WhenJobIsClosed() 
    {
        // Arrange
        var details = new JobDetails("Software Engineer");
        var job = new Job(details);
        job.Post();
        job.Close();
    
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => job.Delete());
        Assert.Equal("Cannot delete a closed job", exception.Message);
    }
    
    [Fact]
    public void Delete_ShouldSucceed_WhenJobIsDraft()
    {
        // Arrange
        var details = new JobDetails("Software Engineer");
        var job = new Job(details);
    
        // Act & Assert
        job.Delete();
    }
    
    [Fact]
    public void Delete_ShouldMarkAsDeleted_WhenJobIsDraft()
    {
        // Arrange
        var details = new JobDetails("Software Engineer");
        var job = new Job(details);
    
        // Act
        job.Delete();
    
        // Assert
        Assert.True(job.IsDeleted);
        Assert.Equal(JobStatus.Draft, job.Status);
    }

    [Fact]
    public void Delete_ShouldThrow_WhenJobIsAlreadyDeleted()
    {
        // Arrange
        var details = new JobDetails("Software Engineer");
        var job = new Job(details);
        job.Delete();
    
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => job.Delete());
        Assert.Equal("Job is already deleted", exception.Message);
    }

    [Fact]
    public void Post_ShouldThrow_WhenJobIsDeleted()
    {
        // Arrange
        var details = new JobDetails("Software Engineer");
        var job = new Job(details);
        job.Delete();
    
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => job.Post());
        Assert.Equal("Cannot post a deleted job", exception.Message);
    }

    [Fact]
    public void Close_ShouldThrow_WhenJobIsDeleted()
    {
        // Arrange
        var details = new JobDetails("Software Engineer");
        var job = new Job(details);
        job.Delete();
    
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => job.Close());
        Assert.Equal("Cannot close a deleted job", exception.Message);
    }
    
    [Fact]
    public void Restore_ShouldUnmarkDeleted_WhenJobIsDeleted()
    {
        // Arrange
        var details = new JobDetails("Software Engineer");
        var job = new Job(details);
        job.Delete();
    
        // Act
        job.Restore();
    
        // Assert
        Assert.False(job.IsDeleted);
        Assert.Equal(JobStatus.Draft, job.Status);
    }

    [Fact]
    public void Restore_ShouldThrow_WhenJobIsNotDeleted()
    {
        // Arrange
        var details = new JobDetails("Software Engineer");
        var job = new Job(details);
    
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => job.Restore());
        Assert.Equal("Job is not deleted", exception.Message);
    }

    [Fact]
    public void Post_ShouldSucceed_AfterRestore()
    {
        // Arrange
        var details = new JobDetails("Software Engineer");
        var job = new Job(details);
        job.Delete();
        job.Restore();
    
        // Act & Assert
        job.Post();
        Assert.Equal(JobStatus.Active, job.Status);
    }
}