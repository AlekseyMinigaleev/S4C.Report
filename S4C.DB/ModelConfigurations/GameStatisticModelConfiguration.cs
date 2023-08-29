using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using C4S.DB.Models;

namespace C4S.DB.ModelConfigurations
{
    public class GameStatisticModelConfiguration : IEntityTypeConfiguration<GamesStatisticModel>
    {
        public void Configure(EntityTypeBuilder<GamesStatisticModel> builder)
        {
            builder.ToTable("GameStatistic")
                .HasKey(x => x.Id);
        }
    }
}
