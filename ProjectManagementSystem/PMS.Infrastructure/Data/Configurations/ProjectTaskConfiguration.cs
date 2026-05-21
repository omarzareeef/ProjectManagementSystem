using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PMS.Domain.Entities;

namespace PMS.Infrastructure.Data.Configurations;

public class ProjectTaskConfiguration : IEntityTypeConfiguration<ProjectTask>
{
    public void Configure(EntityTypeBuilder<ProjectTask> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Description)
            .HasMaxLength(2000);

        builder.Property(r => r.Status)
            .HasConversion<int>();

        builder.Property(r => r.Priority)
            .HasConversion<int>();

        builder.Property(r => r.DueDate)
            .IsRequired();

        builder.Property(r => r.ProjectId)
            .IsRequired();

        builder.Property(r => r.CreatedById)
            .IsRequired();

        builder.HasOne(r => r.Project)
            .WithMany(n => n.ProjectTasks)
            .HasForeignKey(n => n.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.CreatedBy)
            .WithMany(u => u.ProjectTasks)
            .HasForeignKey(r => r.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(pt => new { pt.Id, pt.CreatedById });

        builder.HasIndex(pt => new { pt.CreatedById, pt.ProjectId });
    }
}
