﻿// <auto-generated />
using System;
using BugTracker.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BugTracker.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.1");

            modelBuilder.Entity("BugTracker.Models.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("UserCreatedId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("UserCreatedId");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("BugTracker.Models.Ticket", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("ProjectId")
                        .IsRequired()
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserAssignedId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserCreatedId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.HasIndex("UserAssignedId");

                    b.HasIndex("UserCreatedId");

                    b.ToTable("Tickets");
                });

            modelBuilder.Entity("BugTracker.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .UseCollation("NOCASE");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("BugTracker.Models.Project", b =>
                {
                    b.HasOne("BugTracker.Models.User", "UserCreated")
                        .WithMany("ProjectsCreated")
                        .HasForeignKey("UserCreatedId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UserCreated");
                });

            modelBuilder.Entity("BugTracker.Models.Ticket", b =>
                {
                    b.HasOne("BugTracker.Models.Project", "Project")
                        .WithMany("Tickets")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .IsRequired();

                    b.HasOne("BugTracker.Models.User", "UserAssigned")
                        .WithMany("TicketsAssigned")
                        .HasForeignKey("UserAssignedId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .IsRequired();

                    b.HasOne("BugTracker.Models.User", "UserCreated")
                        .WithMany("TicketsCreated")
                        .HasForeignKey("UserCreatedId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .IsRequired();

                    b.Navigation("Project");

                    b.Navigation("UserAssigned");

                    b.Navigation("UserCreated");
                });

            modelBuilder.Entity("BugTracker.Models.Project", b =>
                {
                    b.Navigation("Tickets");
                });

            modelBuilder.Entity("BugTracker.Models.User", b =>
                {
                    b.Navigation("ProjectsCreated");

                    b.Navigation("TicketsAssigned");

                    b.Navigation("TicketsCreated");
                });
#pragma warning restore 612, 618
        }
    }
}
