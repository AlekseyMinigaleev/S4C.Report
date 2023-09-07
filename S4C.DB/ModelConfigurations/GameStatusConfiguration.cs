using C4S.DB.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace C4S.DB.ModelConfigurations
{
    public class GameStatusConfiguration : IEntityTypeConfiguration<GameStatusModel>
    {
        public void Configure(EntityTypeBuilder<GameStatusModel> builder)
        {
            builder.ToTable("GameStatus")
                .HasKey(x => x.Id);
        }
    }
}