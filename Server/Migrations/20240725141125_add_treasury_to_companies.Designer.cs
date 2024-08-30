﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Server.Persistence;

#nullable disable

namespace Server.Migrations
{
    [DbContext(typeof(WssDbContext))]
    [Migration("20240725141125_add_treasury_to_companies")]
    partial class add_treasury_to_companies
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Server.Models.Company", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int?>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<int>("PlayerId")
                        .HasColumnType("integer");

                    b.Property<int>("Treasury")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(1000000);

                    b.HasKey("Id");

                    b.HasIndex("PlayerId")
                        .IsUnique();

                    b.ToTable("companies", (string)null);
                });

            modelBuilder.Entity("Server.Models.Employee", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int?>("Id"));

                    b.Property<int>("CompanyId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.ToTable("employees", (string)null);
                });

            modelBuilder.Entity("Server.Models.Game", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int?>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<int>("Rounds")
                        .HasColumnType("integer");

                    b.Property<string>("Status")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(255)")
                        .HasDefaultValue("Waiting");

                    b.HasKey("Id");

                    b.ToTable("games", (string)null);
                });

            modelBuilder.Entity("Server.Models.Player", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int?>("Id"));

                    b.Property<int?>("CompanyId")
                        .HasColumnType("integer");

                    b.Property<int>("GameId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("players", (string)null);
                });

            modelBuilder.Entity("Server.Models.Skill", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.ToTable("skills", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "HTML"
                        },
                        new
                        {
                            Id = 2,
                            Name = "CSS"
                        },
                        new
                        {
                            Id = 3,
                            Name = "JavaScript"
                        },
                        new
                        {
                            Id = 4,
                            Name = "TypeScript"
                        },
                        new
                        {
                            Id = 5,
                            Name = "React"
                        },
                        new
                        {
                            Id = 6,
                            Name = "Angular"
                        },
                        new
                        {
                            Id = 7,
                            Name = "Vue.js"
                        },
                        new
                        {
                            Id = 8,
                            Name = "Node.js"
                        },
                        new
                        {
                            Id = 9,
                            Name = "Express.js"
                        },
                        new
                        {
                            Id = 10,
                            Name = "ASP.NET Core"
                        },
                        new
                        {
                            Id = 11,
                            Name = "Ruby on Rails"
                        },
                        new
                        {
                            Id = 12,
                            Name = "Django"
                        },
                        new
                        {
                            Id = 13,
                            Name = "Flask"
                        },
                        new
                        {
                            Id = 14,
                            Name = "PHP"
                        },
                        new
                        {
                            Id = 15,
                            Name = "Laravel"
                        },
                        new
                        {
                            Id = 16,
                            Name = "Spring Boot"
                        },
                        new
                        {
                            Id = 17,
                            Name = "SQL"
                        },
                        new
                        {
                            Id = 18,
                            Name = "NoSQL"
                        },
                        new
                        {
                            Id = 19,
                            Name = "GraphQL"
                        },
                        new
                        {
                            Id = 20,
                            Name = "REST APIs"
                        });
                });

            modelBuilder.Entity("Server.Models.Company", b =>
                {
                    b.HasOne("Server.Models.Player", "Player")
                        .WithOne("Company")
                        .HasForeignKey("Server.Models.Company", "PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player");
                });

            modelBuilder.Entity("Server.Models.Employee", b =>
                {
                    b.HasOne("Server.Models.Company", "Company")
                        .WithMany("Employees")
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsMany("Server.Models.LeveledSkill", "Skills", b1 =>
                        {
                            b1.Property<int>("EmployeeId")
                                .HasColumnType("integer");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("integer");

                            b1.Property<int>("Level")
                                .HasColumnType("integer");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("EmployeeId", "Id");

                            b1.ToTable("employees");

                            b1.ToJson("Skills");

                            b1.WithOwner()
                                .HasForeignKey("EmployeeId");
                        });

                    b.Navigation("Company");

                    b.Navigation("Skills");
                });

            modelBuilder.Entity("Server.Models.Player", b =>
                {
                    b.HasOne("Server.Models.Game", "Game")
                        .WithMany("Players")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");
                });

            modelBuilder.Entity("Server.Models.Company", b =>
                {
                    b.Navigation("Employees");
                });

            modelBuilder.Entity("Server.Models.Game", b =>
                {
                    b.Navigation("Players");
                });

            modelBuilder.Entity("Server.Models.Player", b =>
                {
                    b.Navigation("Company")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
