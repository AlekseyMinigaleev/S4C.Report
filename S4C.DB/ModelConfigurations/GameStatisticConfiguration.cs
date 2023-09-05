using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using C4S.DB.Models;

namespace C4S.DB.ModelConfigurations
{
    public class GameStatisticConfiguration : IEntityTypeConfiguration<GameStatisticModel>
    {
        public void Configure(EntityTypeBuilder<GameStatisticModel> builder)
        {
            builder.ToTable("GameStatistic")
                .HasKey(x => x.Id);
        }
    }
}
