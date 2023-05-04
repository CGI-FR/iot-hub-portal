﻿// <auto-generated />
using System;
using AzureIoTHub.Portal.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AzureIoTHub.Portal.Postgres.Migrations
{
    [DbContext(typeof(PortalDbContext))]
    [Migration("20230502094509_AWS ThingType initial create")]
    partial class AWSThingTypeinitialcreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("AzureIoTHub.Portal.Domain.Entities.AWS.ThingType", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("ThingTypes");
                });

            modelBuilder.Entity("AzureIoTHub.Portal.Domain.Entities.AWS.ThingTypeSearchableAtt", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ThingTypeId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ThingTypeId");

                    b.ToTable("ThingTypeSearchableAttributes");
                });

            modelBuilder.Entity("AzureIoTHub.Portal.Domain.Entities.AWS.ThingTypeTag", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ThingTypeId")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ThingTypeId");

                    b.ToTable("ThingTypeTags");
                });

            modelBuilder.Entity("AzureIoTHub.Portal.Domain.Entities.Concentrator", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ClientThumbprint")
                        .HasColumnType("text");

                    b.Property<string>("DeviceType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsConnected")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("LoraRegion")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Version")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Concentrators");
                });

            modelBuilder.Entity("AzureIoTHub.Portal.Domain.Entities.Device", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("DeviceModelId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsConnected")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("StatusUpdatedTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Version")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("DeviceModelId");

                    b.ToTable("Devices", (string)null);

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("AzureIoTHub.Portal.Domain.Entities.DeviceModel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<bool?>("ABPRelaxMode")
                        .HasColumnType("boolean");

                    b.Property<string>("AppEUI")
                        .HasColumnType("text");

                    b.Property<int>("ClassType")
                        .HasColumnType("integer");

                    b.Property<int?>("Deduplication")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<bool?>("Downlink")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsBuiltin")
                        .HasColumnType("boolean");

                    b.Property<int?>("KeepAliveTimeout")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("PreferredWindow")
                        .HasColumnType("integer");

                    b.Property<int?>("RXDelay")
                        .HasColumnType("integer");

                    b.Property<string>("SensorDecoder")
                        .HasColumnType("text");

                    b.Property<bool>("SupportLoRaFeatures")
                        .HasColumnType("boolean");

                    b.Property<bool?>("UseOTAA")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("DeviceModels");
                });

            modelBuilder.Entity("AzureIoTHub.Portal.Domain.Entities.DeviceModelCommand", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<bool>("Confirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("DeviceModelId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Frame")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsBuiltin")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Port")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("DeviceModelCommands");
                });

            modelBuilder.Entity("AzureIoTHub.Portal.Domain.Entities.DeviceModelProperty", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsWritable")
                        .HasColumnType("boolean");

                    b.Property<string>("ModelId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Order")
                        .HasColumnType("integer");

                    b.Property<int>("PropertyType")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("DeviceModelProperties");
                });

            modelBuilder.Entity("AzureIoTHub.Portal.Domain.Entities.DeviceTag", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Label")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("Required")
                        .HasColumnType("boolean");

                    b.Property<bool>("Searchable")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("DeviceTags");
                });

            modelBuilder.Entity("AzureIoTHub.Portal.Domain.Entities.DeviceTagValue", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("DeviceId")
                        .HasColumnType("text");

                    b.Property<string>("EdgeDeviceId")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("DeviceId");

                    b.HasIndex("EdgeDeviceId");

                    b.ToTable("DeviceTagValues");
                });

            modelBuilder.Entity("AzureIoTHub.Portal.Domain.Entities.EdgeDevice", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ConnectionState")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("DeviceModelId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("NbDevices")
                        .HasColumnType("integer");

                    b.Property<int>("NbModules")
                        .HasColumnType("integer");

                    b.Property<string>("Scope")
                        .HasColumnType("text");

                    b.Property<int>("Version")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("DeviceModelId");

                    b.ToTable("EdgeDevices");
                });

            modelBuilder.Entity("AzureIoTHub.Portal.Domain.Entities.EdgeDeviceModel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("EdgeDeviceModels");
                });

            modelBuilder.Entity("AzureIoTHub.Portal.Domain.Entities.EdgeDeviceModelCommand", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("EdgeDeviceModelId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ModuleName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("EdgeDeviceModelCommands");
                });

            modelBuilder.Entity("AzureIoTHub.Portal.Domain.Entities.Label", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("DeviceId")
                        .HasColumnType("text");

                    b.Property<string>("DeviceModelId")
                        .HasColumnType("text");

                    b.Property<string>("EdgeDeviceId")
                        .HasColumnType("text");

                    b.Property<string>("EdgeDeviceModelId")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("DeviceId");

                    b.HasIndex("DeviceModelId");

                    b.HasIndex("EdgeDeviceId");

                    b.HasIndex("EdgeDeviceModelId");

                    b.ToTable("Labels");
                });

            modelBuilder.Entity("AzureIoTHub.Portal.Domain.Entities.LoRaDeviceTelemetry", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<DateTime>("EnqueuedTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("LorawanDeviceId")
                        .HasColumnType("text");

                    b.Property<string>("Telemetry")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("LorawanDeviceId");

                    b.ToTable("LoRaDeviceTelemetry");
                });

            modelBuilder.Entity("AzureIoTHub.Portal.Domain.Entities.LorawanDevice", b =>
                {
                    b.HasBaseType("AzureIoTHub.Portal.Domain.Entities.Device");

                    b.Property<bool?>("ABPRelaxMode")
                        .HasColumnType("boolean");

                    b.Property<bool>("AlreadyLoggedInOnce")
                        .HasColumnType("boolean");

                    b.Property<string>("AppEUI")
                        .HasColumnType("text");

                    b.Property<string>("AppKey")
                        .HasColumnType("text");

                    b.Property<string>("AppSKey")
                        .HasColumnType("text");

                    b.Property<int>("ClassType")
                        .HasColumnType("integer");

                    b.Property<string>("DataRate")
                        .HasColumnType("text");

                    b.Property<int>("Deduplication")
                        .HasColumnType("integer");

                    b.Property<string>("DevAddr")
                        .HasColumnType("text");

                    b.Property<bool?>("Downlink")
                        .HasColumnType("boolean");

                    b.Property<int?>("FCntDownStart")
                        .HasColumnType("integer");

                    b.Property<int?>("FCntResetCounter")
                        .HasColumnType("integer");

                    b.Property<int?>("FCntUpStart")
                        .HasColumnType("integer");

                    b.Property<string>("GatewayID")
                        .HasColumnType("text");

                    b.Property<int?>("KeepAliveTimeout")
                        .HasColumnType("integer");

                    b.Property<string>("NbRep")
                        .HasColumnType("text");

                    b.Property<string>("NwkSKey")
                        .HasColumnType("text");

                    b.Property<int>("PreferredWindow")
                        .HasColumnType("integer");

                    b.Property<int?>("RX1DROffset")
                        .HasColumnType("integer");

                    b.Property<int?>("RX2DataRate")
                        .HasColumnType("integer");

                    b.Property<int?>("RXDelay")
                        .HasColumnType("integer");

                    b.Property<string>("ReportedRX1DROffset")
                        .HasColumnType("text");

                    b.Property<string>("ReportedRX2DataRate")
                        .HasColumnType("text");

                    b.Property<string>("ReportedRXDelay")
                        .HasColumnType("text");

                    b.Property<string>("SensorDecoder")
                        .HasColumnType("text");

                    b.Property<bool?>("Supports32BitFCnt")
                        .HasColumnType("boolean");

                    b.Property<string>("TxPower")
                        .HasColumnType("text");

                    b.Property<bool>("UseOTAA")
                        .HasColumnType("boolean");

                    b.ToTable("LorawanDevices", (string)null);
                });

            modelBuilder.Entity("AzureIoTHub.Portal.Domain.Entities.AWS.ThingTypeSearchableAtt", b =>
                {
                    b.HasOne("AzureIoTHub.Portal.Domain.Entities.AWS.ThingType", null)
                        .WithMany("ThingTypeSearchableAttributes")
                        .HasForeignKey("ThingTypeId");
                });

            modelBuilder.Entity("AzureIoTHub.Portal.Domain.Entities.AWS.ThingTypeTag", b =>
                {
                    b.HasOne("AzureIoTHub.Portal.Domain.Entities.AWS.ThingType", null)
                        .WithMany("Tags")
                        .HasForeignKey("ThingTypeId");
                });

            modelBuilder.Entity("AzureIoTHub.Portal.Domain.Entities.Device", b =>
                {
                    b.HasOne("AzureIoTHub.Portal.Domain.Entities.DeviceModel", "DeviceModel")
                        .WithMany()
                        .HasForeignKey("DeviceModelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DeviceModel");
                });

            modelBuilder.Entity("AzureIoTHub.Portal.Domain.Entities.DeviceTagValue", b =>
                {
                    b.HasOne("AzureIoTHub.Portal.Domain.Entities.Device", null)
                        .WithMany("Tags")
                        .HasForeignKey("DeviceId");

                    b.HasOne("AzureIoTHub.Portal.Domain.Entities.EdgeDevice", null)
                        .WithMany("Tags")
                        .HasForeignKey("EdgeDeviceId");
                });

            modelBuilder.Entity("AzureIoTHub.Portal.Domain.Entities.EdgeDevice", b =>
                {
                    b.HasOne("AzureIoTHub.Portal.Domain.Entities.EdgeDeviceModel", "DeviceModel")
                        .WithMany()
                        .HasForeignKey("DeviceModelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DeviceModel");
                });

            modelBuilder.Entity("AzureIoTHub.Portal.Domain.Entities.Label", b =>
                {
                    b.HasOne("AzureIoTHub.Portal.Domain.Entities.Device", null)
                        .WithMany("Labels")
                        .HasForeignKey("DeviceId");

                    b.HasOne("AzureIoTHub.Portal.Domain.Entities.DeviceModel", null)
                        .WithMany("Labels")
                        .HasForeignKey("DeviceModelId");

                    b.HasOne("AzureIoTHub.Portal.Domain.Entities.EdgeDevice", null)
                        .WithMany("Labels")
                        .HasForeignKey("EdgeDeviceId");

                    b.HasOne("AzureIoTHub.Portal.Domain.Entities.EdgeDeviceModel", null)
                        .WithMany("Labels")
                        .HasForeignKey("EdgeDeviceModelId");
                });

            modelBuilder.Entity("AzureIoTHub.Portal.Domain.Entities.LoRaDeviceTelemetry", b =>
                {
                    b.HasOne("AzureIoTHub.Portal.Domain.Entities.LorawanDevice", null)
                        .WithMany("Telemetry")
                        .HasForeignKey("LorawanDeviceId");
                });

            modelBuilder.Entity("AzureIoTHub.Portal.Domain.Entities.LorawanDevice", b =>
                {
                    b.HasOne("AzureIoTHub.Portal.Domain.Entities.Device", null)
                        .WithOne()
                        .HasForeignKey("AzureIoTHub.Portal.Domain.Entities.LorawanDevice", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AzureIoTHub.Portal.Domain.Entities.AWS.ThingType", b =>
                {
                    b.Navigation("Tags");

                    b.Navigation("ThingTypeSearchableAttributes");
                });

            modelBuilder.Entity("AzureIoTHub.Portal.Domain.Entities.Device", b =>
                {
                    b.Navigation("Labels");

                    b.Navigation("Tags");
                });

            modelBuilder.Entity("AzureIoTHub.Portal.Domain.Entities.DeviceModel", b =>
                {
                    b.Navigation("Labels");
                });

            modelBuilder.Entity("AzureIoTHub.Portal.Domain.Entities.EdgeDevice", b =>
                {
                    b.Navigation("Labels");

                    b.Navigation("Tags");
                });

            modelBuilder.Entity("AzureIoTHub.Portal.Domain.Entities.EdgeDeviceModel", b =>
                {
                    b.Navigation("Labels");
                });

            modelBuilder.Entity("AzureIoTHub.Portal.Domain.Entities.LorawanDevice", b =>
                {
                    b.Navigation("Telemetry");
                });
#pragma warning restore 612, 618
        }
    }
}
