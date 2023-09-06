using C4S.DB.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace C4S.DB.ModelConfigurations
{
    public class GameGameStatusConfiguration : IEntityTypeConfiguration<GameGameStatusModel>
    {
        public void Configure(EntityTypeBuilder<GameGameStatusModel> builder)
        {
            builder.ToTable("GameGameStatus");
            builder.HasKey(x => new { x.GameStatisticId, x.GameStatusId });

            builder.HasOne(x => x.GameStatus)
                .WithMany()
                .HasForeignKey(x => x.GameStatusId);
        }
    }
}