using JobManagement.Domain.ValueObjects;
using Recruitment.Core.Common;

namespace JobManagement.Domain.Entities;

public class Job : AggregateRoot<Guid>
{
    public required string Title { get; init; }
    public JobStatus Status { get; private set; } = JobStatus.Draft;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public void Post()
    {
        if (Status == JobStatus.Active)
            throw new InvalidOperationException("Job is already posted");
        
        if (Status == JobStatus.Closed)
            throw new InvalidOperationException("Cannot post a closed job");

        Status = JobStatus.Active;
    }
    
    public void Close()
    {
        if (Status == JobStatus.Closed)
            throw new InvalidOperationException("Job is already closed");
        
        Status = JobStatus.Closed;
    }
}