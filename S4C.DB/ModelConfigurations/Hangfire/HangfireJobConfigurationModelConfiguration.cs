using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using C4S.DB.Models.Hangfire;

namespace C4S.DB.ModelConfigurations.Hangfire
{
    public class HangfireJobConfigurationModelConfiguration : IEntityTypeConfiguration<HangfireJobConfigurationModel>
    {
        public void Configure(EntityTypeBuilder<HangfireJobConfigurationModel> builder)
        {
            builder
                .ToTable("HangfireJobConfiguration")
                .HasKey(x => x.JopType);
        }
    }
}
