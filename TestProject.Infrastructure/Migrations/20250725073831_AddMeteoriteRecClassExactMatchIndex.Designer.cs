﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TestProject.Infrastructure;

#nullable disable

namespace TestProject.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250725073831_AddMeteoriteRecClassExactMatchIndex")]
    partial class AddMeteoriteRecClassExactMatchIndex
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "postgis");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("TestProject.Core.Entities.Meteorite", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("Fall")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Point>("Geolocation")
                        .HasColumnType("geometry (point, 4326)");

                    b.Property<decimal?>("Mass")
                        .HasColumnType("numeric");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("NameType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RecClass")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("Reclat")
                        .HasColumnType("numeric(9,6)");

                    b.Property<decimal>("Reclong")
                        .HasColumnType("numeric(9,6)");

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<DateTime>("Year")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Meteorite");
                });
#pragma warning restore 612, 618
        }
    }
}
