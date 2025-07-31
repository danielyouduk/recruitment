using JobManagement.Domain.ValueObjects;
using Recruitment.Core.Common;

namespace JobManagement.Domain.Entities;

public class Job : AggregateRoot<Guid>
{
    public required string Title { get; init; }
    public JobStatus Status { get; private set; } = JobStatus.Draft;
    public bool IsDeleted { get; private set; } = false;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public void Post()
    {
        if (IsDeleted)
            throw new InvalidOperationException("Cannot post a deleted job");
        
        if (Status == JobStatus.Active)
            throw new InvalidOperationException("Job is already posted");
        
        if (Status == JobStatus.Closed)
            throw new InvalidOperationException("Cannot post a closed job");

        Status = JobStatus.Active;
    }
    
    public void Close()
    {
        if (IsDeleted)
            throw new InvalidOperationException("Cannot close a deleted job");

        if (Status == JobStatus.Draft)
            throw new InvalidOperationException("Cannot close a draft job. Delete it instead.");

        if (Status == JobStatus.Closed)
            throw new InvalidOperationException("Job is already closed");
        
        Status = JobStatus.Closed;
    }
    
    public void Delete()
    {
        if (IsDeleted)
            throw new InvalidOperationException("Job is already deleted");

        if (Status == JobStatus.Active)
            throw new InvalidOperationException("Cannot delete an active job. Close it first.");
            
        if (Status == JobStatus.Closed)
            throw new InvalidOperationException("Cannot delete a closed job");
        
        IsDeleted = true;
    }
    
    public void Restore()
    {
        if (!IsDeleted)
            throw new InvalidOperationException("Job is not deleted");
        
        IsDeleted = false;
    }
}