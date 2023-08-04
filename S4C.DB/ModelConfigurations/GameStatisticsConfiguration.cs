using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using S4C.DB.Models;

namespace S4C.DB.ModelConfigurations
{
    public class GameStatisticsConfiguration : IEntityTypeConfiguration<GamesStatisticModel>
    {
        public void Configure(EntityTypeBuilder<GamesStatisticModel> builder)
        {
            builder.ToTable("GameStatistic")
                .HasKey(x => x.Id);
        }
    }
}
