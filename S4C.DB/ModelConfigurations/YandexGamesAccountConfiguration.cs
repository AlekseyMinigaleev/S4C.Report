using C4S.DB.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace C4S.DB.ModelConfigurations
{
    public class YandexGamesAccountConfiguration : IEntityTypeConfiguration<YandexGamesAccountModel>
    {
        public void Configure(EntityTypeBuilder<YandexGamesAccountModel> builder)
        {
            builder.ToTable("YandexGamesAccount")
                .HasKey(x => x.Id);

            builder.HasMany(x => x.Games)
                .WithOne(x => x.YandexGamesAccount)
                .HasForeignKey(x => x.YandexGamesAccountId);
        }
    }
}