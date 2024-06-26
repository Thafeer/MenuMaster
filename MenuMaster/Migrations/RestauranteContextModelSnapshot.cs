﻿// <auto-generated />
using System;
using MenuMaster.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MenuMaster.Migrations
{
    [DbContext(typeof(RestauranteContext))]
    partial class RestauranteContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MenuMaster.Models.Cliente", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                b.Property<string>("Nome")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("Telefone")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.HasKey("Id");

                b.ToTable("Clientes");
            });

            modelBuilder.Entity("MenuMaster.Models.MenuItem", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                b.Property<bool>("Disponivel")
                    .HasColumnType("bit");

                b.Property<string>("Nome")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<decimal>("Preco")
                    .HasColumnType("decimal(18,2)");

                b.Property<string>("Tipo")
                    .HasColumnType("nvarchar(max)");

                b.HasKey("Id");

                b.ToTable("MenuItens");
            });

            modelBuilder.Entity("MenuMaster.Models.Mesa", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                b.Property<int>("ClienteId")
                    .HasColumnType("int");

                b.Property<int>("Numero")
                    .HasColumnType("int");

                b.Property<bool>("Ocupada")
                    .HasColumnType("bit");

                b.Property<bool>("Disponivel")
                    .HasColumnType("bit");

                b.HasKey("Id");

                b.ToTable("Mesas");
            });

            modelBuilder.Entity("MenuMaster.Models.Pedido", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                b.Property<DateTime>("DataHora")
                    .HasColumnType("datetime2");

                b.Property<int>("MesaId")
                    .HasColumnType("int");

                b.Property<int>("MenuItemId")
                   .HasColumnType("int");

                b.Property<decimal>("Total")
                    .HasColumnType("decimal(18,2)");

                b.Property<int>("Quantidade")
                    .HasColumnType("int");

                b.Property<decimal>("Preco")
                   .HasColumnType("decimal(18,2)");

                b.Property<string>("Status")
                    .HasColumnType("nvarchar(max)");

                b.HasKey("Id");

                b.ToTable("Pedidos");
            });
#pragma warning restore 612, 618
        }
    }
}