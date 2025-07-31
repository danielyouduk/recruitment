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
}