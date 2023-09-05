using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using C4S.DB.Models;

namespace C4S.DB.ModelConfigurations
{
    public class GameConfiguration : IEntityTypeConfiguration<GameModel>
    {
        public void Configure(EntityTypeBuilder<GameModel> builder)
        {
            builder.ToTable("Game")
                .HasKey(x => x.Id);
            
            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.HasMany(x => x.GameStatistics)
                .WithOne(x => x.Game)
                .HasForeignKey(x => x.GameId);
        }
    }
}
