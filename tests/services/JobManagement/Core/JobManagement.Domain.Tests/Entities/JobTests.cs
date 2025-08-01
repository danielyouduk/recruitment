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
        Assert.Equal(JobStatus.New, job.Status);
        Assert.False(job.IsDeleted);
        Assert.True(job.CreatedAt <= DateTime.UtcNow);
        Assert.True(job.CreatedAt >= DateTime.UtcNow.AddSeconds(-1));
    }
    
    [Fact]
    public void Post_ShouldChangeStatus_FromDraftToActive()
    {
        // Arrange
        var details = new JobDetails("Software Engineer");
        var job = new Job(details);
        job.CreateDraft();
    
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
        job.CreateDraft();
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
        job.CreateDraft();
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
        var details = new JobDetails("Software Engineer");
        var job = new Job(details);
        job.CreateDraft();
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
        job.CreateDraft();
        
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => job.Close());
        Assert.Equal("Cannot close a draft job. Delete it instead.", exception.Message);
    }
    
    [Fact]
    public void Close_ShouldThrow_WhenJobIsNew()
    {
        // Arrange
        var details = new JobDetails("Software Engineer");
        var job = new Job(details);
    
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => job.Close());
        Assert.Equal("Cannot close an unsaved job.", exception.Message);
    }
    
    [Fact]
    public void Delete_ShouldThrow_WhenJobIsActive()
    {
        // Arrange
        var details = new JobDetails("Software Engineer");
        var job = new Job(details);
        job.CreateDraft();
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
        job.CreateDraft();
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
        job.CreateDraft();
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
        job.CreateDraft();
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
        job.CreateDraft();
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
        job.CreateDraft();
        job.Delete();
        job.Restore();
    
        // Act & Assert
        job.Post();
        Assert.Equal(JobStatus.Active, job.Status);
    }
    
    [Fact]
    public void Post_ShouldThrow_WhenJobIsNew()
    {
        // Arrange
        var details = new JobDetails("Software Engineer");
        var job = new Job(details);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => job.Post());
        Assert.Equal("Can only post saved draft jobs", exception.Message);
    }
    
    [Fact]
    public void CreateDraft_ShouldThrow_WhenJobIsDeleted()
    {
        // Arrange
        var details = new JobDetails("Software Engineer");
        var job = new Job(details);
        job.Delete();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => job.CreateDraft());
        Assert.Equal("Cannot create draft from a deleted job", exception.Message);
    }

    [Fact]
    public void CreateDraft_ShouldThrow_WhenJobIsNotNew()
    {
        // Arrange
        var details = new JobDetails("Software Engineer");
        var job = new Job(details);
        job.CreateDraft();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => job.CreateDraft());
        Assert.Equal("Can only create draft from new job", exception.Message);
    }
    
    [Fact]
    public void Job_ShouldInitializeWithNullPostedAt()
    {
        // Arrange
        var details = new JobDetails("Software Engineer");

        // Act
        var job = new Job(details);

        // Assert
        Assert.Null(job.PostedAt);
    }

    [Fact]
    public void Post_ShouldSetPostedAt_WhenJobIsPosted()
    {
        // Arrange
        var details = new JobDetails("Software Engineer");
        var job = new Job(details);
        job.CreateDraft();
        var beforePost = DateTime.UtcNow;

        // Act
        job.Post();

        // Assert
        Assert.NotNull(job.PostedAt);
        Assert.True(job.PostedAt >= beforePost);
        Assert.True(job.PostedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Post_ShouldChangeStatusAndSetPostedAt()
    {
        // Arrange
        var details = new JobDetails("Software Engineer");
        var job = new Job(details);
        job.CreateDraft();

        // Act
        job.Post();

        // Assert
        Assert.Equal(JobStatus.Active, job.Status);
        Assert.NotNull(job.PostedAt);
    }
}