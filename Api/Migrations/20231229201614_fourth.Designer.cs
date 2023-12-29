﻿// <auto-generated />
using Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Api.Migrations
{
    [DbContext(typeof(AppDBContext))]
    [Migration("20231229201614_fourth")]
    partial class fourth
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Api.Models.Teacher", b =>
                {
                    b.Property<string>("TeachersID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HashedPassword")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SingleProperty")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TeachersID");

                    b.ToTable("Teachers");
                });

            modelBuilder.Entity("Api.Models.TeachersRelation.TeachersProfile", b =>
                {
                    b.Property<string>("ProfileID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Bio")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TeachersName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TeachersProfileID")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TeachersTraining")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ProfileID");

                    b.ToTable("TeachersProfiles");
                });

            modelBuilder.Entity("Api.Models.User", b =>
                {
                    b.Property<string>("UsersID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HashedPassword")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UsersID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Api.Models.UsersRelation.UsersProfile", b =>
                {
                    b.Property<string>("ProfileID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Bio")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Classroom")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UsersProfileID")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ProfileID");

                    b.ToTable("UsersProfiles");
                });
#pragma warning restore 612, 618
        }
    }
}
