using JobManagement.Domain.Entities;
using JobManagement.Domain.ValueObjects;

namespace JobManagement.Domain.Tests.Entities;

public class JobTests
{
    [Fact]
    public void Job_ShouldHave_RequiredProperties()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var title = "Software Engineer";
        
        // Act
        var job = new Job 
        { 
            Id = jobId, 
            Title = title 
        };
        
        // Assert
        Assert.Equal(jobId, job.Id);
        Assert.Equal(title, job.Title);
        Assert.Equal(JobStatus.Draft, job.Status);
        Assert.True(job.CreatedAt <= DateTime.UtcNow);
        Assert.True(job.CreatedAt >= DateTime.UtcNow.AddSeconds(-1));
    }
    
    [Fact]
    public void Job_ShouldInitialize_WithDraftStatus()
    {
        // Arrange & Act
        var job = new Job 
        { 
            Id = Guid.NewGuid(), 
            Title = "Test Job" 
        };
        
        // Assert
        Assert.Equal(JobStatus.Draft, job.Status);
    }
    
    [Fact]
    public void Post_ShouldChangeStatus_FromDraftToActive()
    {
        // Arrange
        var job = new Job { Id = Guid.NewGuid(), Title = "Software Engineer" };
    
        // Act
        job.Post();
    
        // Assert
        Assert.Equal(JobStatus.Active, job.Status);
    }
    
    [Fact]
    public void Post_ShouldThrow_WhenJobAlreadyActive()
    {
        // Arrange
        var job = new Job { Id = Guid.NewGuid(), Title = "Software Engineer" };
        job.Post();
        
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => job.Post());
        Assert.Equal("Job is already posted", exception.Message);
    }
    
    [Fact]
    public void Post_ShouldThrow_WhenJobIsClosed()
    {
        // Arrange
        var job = new Job { Id = Guid.NewGuid(), Title = "Software Engineer" };
        job.Post();
        job.Close();
    
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => job.Post());
        Assert.Equal("Cannot post a closed job", exception.Message);
    }
    
    [Fact]
    public void Close_ShouldThrow_WhenJobAlreadyClosed()
    {
        // Arrange
        var job = new Job { Id = Guid.NewGuid(), Title = "Software Engineer" };
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
        var job = new Job { Id = Guid.NewGuid(), Title = "Software Engineer" };
    
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => job.Close());
        Assert.Equal("Cannot close a draft job. Delete it instead.", exception.Message);
    }
    
    [Fact]
    public void Delete_ShouldThrow_WhenJobIsActive()
    {
        // Arrange
        var job = new Job { Id = Guid.NewGuid(), Title = "Software Engineer" };
        job.Post();
    
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => job.Delete());
        Assert.Equal("Cannot delete an active job. Close it first.", exception.Message);
    }

    [Fact]
    public void Delete_ShouldThrow_WhenJobIsClosed() 
    {
        // Arrange
        var job = new Job { Id = Guid.NewGuid(), Title = "Software Engineer" };
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
        var job = new Job { Id = Guid.NewGuid(), Title = "Software Engineer" };
    
        // Act & Assert
        job.Delete();
    }
    
    
    [Fact]
    public void Delete_ShouldMarkAsDeleted_WhenJobIsDraft()
    {
        // Arrange
        var job = new Job { Id = Guid.NewGuid(), Title = "Software Engineer" };
    
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
        var job = new Job { Id = Guid.NewGuid(), Title = "Software Engineer" };
        job.Delete();
    
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => job.Delete());
        Assert.Equal("Job is already deleted", exception.Message);
    }

    [Fact]
    public void Post_ShouldThrow_WhenJobIsDeleted()
    {
        // Arrange
        var job = new Job { Id = Guid.NewGuid(), Title = "Software Engineer" };
        job.Delete();
    
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => job.Post());
        Assert.Equal("Cannot post a deleted job", exception.Message);
    }

    [Fact]
    public void Close_ShouldThrow_WhenJobIsDeleted()
    {
        // Arrange
        var job = new Job { Id = Guid.NewGuid(), Title = "Software Engineer" };
        job.Delete();
    
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => job.Close());
        Assert.Equal("Cannot close a deleted job", exception.Message);
    }
    
    [Fact]
    public void Restore_ShouldUnmarkDeleted_WhenJobIsDeleted()
    {
        // Arrange
        var job = new Job { Id = Guid.NewGuid(), Title = "Software Engineer" };
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
        var job = new Job { Id = Guid.NewGuid(), Title = "Software Engineer" };
    
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => job.Restore());
        Assert.Equal("Job is not deleted", exception.Message);
    }

    [Fact]
    public void Post_ShouldSucceed_AfterRestore()
    {
        // Arrange
        var job = new Job { Id = Guid.NewGuid(), Title = "Software Engineer" };
        job.Delete();
        job.Restore();
    
        // Act & Assert
        job.Post();
        Assert.Equal(JobStatus.Active, job.Status);
    }
}