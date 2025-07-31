using JobManagement.Domain.ValueObjects;
using Recruitment.Core.Common;

namespace JobManagement.Domain.Entities;

public class Job : AggregateRoot<Guid>
{
    public required string Title { get; init; }
    public JobStatus Status { get; private set; } = JobStatus.Draft;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

}