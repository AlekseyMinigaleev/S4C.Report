using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using S4C.DB.Models;

namespace S4C.DB.ModelConfigurations
{
    public class GameModelConfiguration : IEntityTypeConfiguration<GameModel>
    {
        public void Configure(EntityTypeBuilder<GameModel> builder)
        {
            builder.ToTable("Game")
                .HasKey(x => x.Id);

            builder.HasMany(x => x.GameStatistics)
                .WithOne(x => x.Game)
                .HasForeignKey(x => x.GameId);
        }
    }
}
