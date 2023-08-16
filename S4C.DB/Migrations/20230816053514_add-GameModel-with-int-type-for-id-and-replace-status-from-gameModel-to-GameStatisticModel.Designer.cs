﻿// <auto-generated />
using System;
using C4S.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace C4S.DB.Migrations
{
    [DbContext(typeof(ReportDbContext))]
    [Migration("20230816053514_add-GameModel-with-int-type-for-id-and-replace-status-from-gameModel-to-GameStatisticModel")]
    partial class addGameModelwithinttypeforidandreplacestatusfromgameModeltoGameStatisticModel
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("C4S.DB.Models.GameModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("PublicationDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Game", (string)null);
                });

            modelBuilder.Entity("C4S.DB.Models.GamesStatisticModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<double?>("Evaluation")
                        .HasColumnType("float");

                    b.Property<int>("GameId")
                        .HasColumnType("int");

                    b.Property<DateTime>("LastSynchroDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("PlayersCount")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("GameStatistic", (string)null);
                });

            modelBuilder.Entity("C4S.DB.Models.Hangfire.HangfireJobConfigurationModel", b =>
                {
                    b.Property<int>("JobType")
                        .HasColumnType("int");

                    b.Property<string>("CronExpression")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsEnable")
                        .HasColumnType("bit");

                    b.HasKey("JobType");

                    b.ToTable("HangfireJobConfiguration", (string)null);
                });

            modelBuilder.Entity("C4S.DB.Models.GamesStatisticModel", b =>
                {
                    b.HasOne("C4S.DB.Models.GameModel", "Game")
                        .WithMany("GameStatistics")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");
                });

            modelBuilder.Entity("C4S.DB.Models.GameModel", b =>
                {
                    b.Navigation("GameStatistics");
                });
#pragma warning restore 612, 618
        }
    }
}
