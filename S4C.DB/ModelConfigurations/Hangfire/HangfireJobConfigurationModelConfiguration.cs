using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using S4C.DB.Models.Hangfire;

namespace S4C.DB.ModelConfigurations.Hangfire
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
