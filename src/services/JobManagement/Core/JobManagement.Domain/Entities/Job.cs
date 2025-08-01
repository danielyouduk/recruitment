using JobManagement.Domain.ValueObjects;
using Recruitment.Core.Common;

namespace JobManagement.Domain.Entities;

public class Job : AggregateRoot<Guid>
{
    public Job(JobDetails details) : base(Guid.NewGuid())
    {
        Id = Guid.NewGuid();
        Details = details ?? throw new ArgumentNullException(nameof(details));
        Status = JobStatus.New;
    }

    public JobDetails Details { get; private set; }
    public JobStatus Status { get; private set; } = JobStatus.Draft;
    public bool IsDeleted { get; private set; } = false;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public void CreateDraft()
    {
        if (IsDeleted)
            throw new InvalidOperationException("Cannot create draft from a deleted job");
            
        if (Status != JobStatus.New)
            throw new InvalidOperationException("Can only create draft from new job");
            
        Status = JobStatus.Draft;
    }
    
    public void Post()
    {
        if (Status == JobStatus.New)
            throw new InvalidOperationException("Can only post saved draft jobs");

        if (IsDeleted)
            throw new InvalidOperationException("Cannot post a deleted job");
        
        if (Status == JobStatus.Active)
            throw new InvalidOperationException("Job is already posted");
        
        if (Status == JobStatus.Closed)
            throw new InvalidOperationException("Cannot post a closed job");

        Status = JobStatus.Active;
    }
    
    public void UpdateDetails(JobDetails newDetails)
    {
        if (newDetails == null)
            throw new ArgumentNullException(nameof(newDetails));
            
        if (IsDeleted)
            throw new InvalidOperationException("Cannot update details of a deleted job");
            
        if (Status == JobStatus.Active)
            throw new InvalidOperationException("Cannot update details of an active job");
            
        if (Status == JobStatus.Closed)
            throw new InvalidOperationException("Cannot update details of a closed job");
            
        Details = newDetails;
    }
    
    public void Close()
    {
        if (IsDeleted)
            throw new InvalidOperationException("Cannot close a deleted job");

        if (Status == JobStatus.New)
            throw new InvalidOperationException("Cannot close an unsaved job.");
        
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