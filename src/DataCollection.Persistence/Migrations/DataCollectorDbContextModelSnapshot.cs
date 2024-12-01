﻿// <auto-generated />
using System;
using DataCollection.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DataCollection.Persistence.Migrations
{
    [DbContext(typeof(DataCollectorDbContext))]
    partial class DataCollectorDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0");

            modelBuilder.Entity("DataCollection.Persistence.WindowsDataEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("ProcessFileName")
                        .HasMaxLength(1000)
                        .HasColumnType("TEXT");

                    b.Property<string>("ProcessFriendlyName")
                        .HasMaxLength(1000)
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("StartTime")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("StopTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("WindowTitle")
                        .HasMaxLength(1000)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("WindowsData");
                });
#pragma warning restore 612, 618
        }
    }
}