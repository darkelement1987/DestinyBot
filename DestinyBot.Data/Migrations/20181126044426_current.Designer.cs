﻿// <auto-generated />
using DestinyBot.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DestinyBot.Data.Migrations
{
    [DbContext(typeof(DestinyBotContext))]
    [Migration("20181126044426_current")]
    partial class current
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024");

            modelBuilder.Entity("DestinyBot.Data.Entities.GuildOwner", b =>
                {
                    b.Property<string>("GuildId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ChannelHub");

                    b.Property<string>("Snowflake");

                    b.Property<long>("TwitchId");

                    b.Property<long>("TwitterId");

                    b.Property<string>("YoutubeId");

                    b.HasKey("GuildId");

                    b.ToTable("GuildOwners");
                });

            modelBuilder.Entity("DestinyBot.Data.Entities.YoutubeSubscription", b =>
                {
                    b.Property<string>("ChannelId");

                    b.Property<string>("DiscordChannelId");

                    b.Property<string>("ChannelName");

                    b.Property<long>("LatestVideoDate");

                    b.HasKey("ChannelId", "DiscordChannelId");

                    b.ToTable("YoutubeSubscriptions");
                });
#pragma warning restore 612, 618
        }
    }
}
