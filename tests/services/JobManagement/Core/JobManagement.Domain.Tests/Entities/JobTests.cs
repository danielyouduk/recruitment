using JobManagement.Domain.Entities;
using JobManagement.Domain.ValueObjects;

namespace JobManagement.Domain.Tests.Entities;

public class JobTests
{
    [Fact]
    public void Job_ShouldHave_RequiredProperties()
    {
        // Arrange
        var title = "Software Engineer";
        var details = new JobDetails(title);
        
        // Act
        var job = new Job(details);
        
        // Assert
        Assert.Equal(title, job.Details.Title);
        Assert.Equal(JobStatus.Draft, job.Status);
        Assert.True(job.CreatedAt <= DateTime.UtcNow);
        Assert.True(job.CreatedAt >= DateTime.UtcNow.AddSeconds(-1));
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