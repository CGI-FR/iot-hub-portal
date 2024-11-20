﻿// <auto-generated />
using System;
using IoTHub.Portal.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace IoTHub.Portal.Postgres.Migrations
{
    [DbContext(typeof(PortalDbContext))]
    [Migration("20241120123802_RoleBasedAccessControl-ScopeGesture")]
    partial class RoleBasedAccessControlScopeGesture
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("GroupUser", b =>
                {
                    b.Property<string>("GroupsId")
                        .HasColumnType("text");

                    b.Property<string>("MembersId")
                        .HasColumnType("text");

                    b.HasKey("GroupsId", "MembersId");

                    b.HasIndex("MembersId");

                    b.ToTable("GroupUser");
                });

            modelBuilder.Entity("IoTHub.Portal.Domain.Entities.AccessControl", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("PrincipalId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ScopeId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("PrincipalId");

                    b.HasIndex("RoleId");

                    b.HasIndex("ScopeId");

                    b.ToTable("AccessControls");
                });

            modelBuilder.Entity("IoTHub.Portal.Domain.Entities.Action", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("Actions");
                });

            modelBuilder.Entity("IoTHub.Portal.Domain.Entities.Concentrator", b =>
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

            modelBuilder.Entity("IoTHub.Portal.Domain.Entities.Device", b =>
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

            modelBuilder.Entity("IoTHub.Portal.Domain.Entities.DeviceModel", b =>
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

            modelBuilder.Entity("IoTHub.Portal.Domain.Entities.DeviceModelCommand", b =>
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

            modelBuilder.Entity("IoTHub.Portal.Domain.Entities.DeviceModelProperty", b =>
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

            modelBuilder.Entity("IoTHub.Portal.Domain.Entities.DeviceTag", b =>
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

            modelBuilder.Entity("IoTHub.Portal.Domain.Entities.DeviceTagValue", b =>
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

            modelBuilder.Entity("IoTHub.Portal.Domain.Entities.EdgeDevice", b =>
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

            modelBuilder.Entity("IoTHub.Portal.Domain.Entities.EdgeDeviceModel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("ExternalIdentifier")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("EdgeDeviceModels");
                });

            modelBuilder.Entity("IoTHub.Portal.Domain.Entities.EdgeDeviceModelCommand", b =>
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

            modelBuilder.Entity("IoTHub.Portal.Domain.Entities.Group", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PrincipalId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("PrincipalId")
                        .IsUnique();

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("IoTHub.Portal.Domain.Entities.Label", b =>
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

            modelBuilder.Entity("IoTHub.Portal.Domain.Entities.LoRaDeviceTelemetry", b =>
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

            modelBuilder.Entity("IoTHub.Portal.Domain.Entities.Principal", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Principals");
                });

            modelBuilder.Entity("IoTHub.Portal.Domain.Entities.Role", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("IoTHub.Portal.Domain.Entities.Scope", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ParentId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("Scopes");
                });

            modelBuilder.Entity("IoTHub.Portal.Domain.Entities.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Avatar")
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FamilyName")
                        .HasColumnType("text");

                    b.Property<string>("GivenName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("PrincipalId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("GivenName")
                        .IsUnique();

                    b.HasIndex("PrincipalId")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.DataProtection.EntityFrameworkCore.DataProtectionKey", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("FriendlyName")
                        .HasColumnType("text");

                    b.Property<string>("Xml")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("DataProtectionKeys");
                });

            modelBuilder.Entity("IoTHub.Portal.Domain.Entities.LorawanDevice", b =>
                {
                    b.HasBaseType("IoTHub.Portal.Domain.Entities.Device");

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

            modelBuilder.Entity("GroupUser", b =>
                {
                    b.HasOne("IoTHub.Portal.Domain.Entities.Group", null)
                        .WithMany()
                        .HasForeignKey("GroupsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("IoTHub.Portal.Domain.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("MembersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("IoTHub.Portal.Domain.Entities.AccessControl", b =>
                {
                    b.HasOne("IoTHub.Portal.Domain.Entities.Principal", "Principal")
                        .WithMany("AccessControls")
                        .HasForeignKey("PrincipalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("IoTHub.Portal.Domain.Entities.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("IoTHub.Portal.Domain.Entities.Scope", "Scope")
                        .WithMany()
                        .HasForeignKey("ScopeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Principal");

                    b.Navigation("Role");

                    b.Navigation("Scope");
                });

            modelBuilder.Entity("IoTHub.Portal.Domain.Entities.Action", b =>
                {
                    b.HasOne("IoTHub.Portal.Domain.Entities.Role", null)
                        .WithMany("Actions")
                        .HasForeignKey("RoleId");
                });

            modelBuilder.Entity("IoTHub.Portal.Domain.Entities.Device", b =>
                {
                    b.HasOne("IoTHub.Portal.Domain.Entities.DeviceModel", "DeviceModel")
                        .WithMany()
                        .HasForeignKey("DeviceModelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DeviceModel");
                });

            modelBuilder.Entity("IoTHub.Portal.Domain.Entities.DeviceTagValue", b =>
                {
                    b.HasOne("IoTHub.Portal.Domain.Entities.Device", null)
                        .WithMany("Tags")
                        .HasForeignKey("DeviceId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("IoTHub.Portal.Domain.Entities.EdgeDevice", null)
                        .WithMany("Tags")
                        .HasForeignKey("EdgeDeviceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("IoTHub.Portal.Domain.Entities.EdgeDevice", b =>
                {
                    b.HasOne("IoTHub.Portal.Domain.Entities.EdgeDeviceModel", "DeviceModel")
                        .WithMany()
                        .HasForeignKey("DeviceModelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DeviceModel");
                });

            modelBuilder.Entity("IoTHub.Portal.Domain.Entities.Group", b =>
                {
                    b.HasOne("IoTHub.Portal.Domain.Entities.Principal", "Principal")
                        .WithOne("Group")
                        .HasForeignKey("IoTHub.Portal.Domain.Entities.Group", "PrincipalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Principal");
                });

            modelBuilder.Entity("IoTHub.Portal.Domain.Entities.Label", b =>
                {
                    b.HasOne("IoTHub.Portal.Domain.Entities.Device", null)
                        .WithMany("Labels")
                        .HasForeignKey("DeviceId");

                    b.HasOne("IoTHub.Portal.Domain.Entities.DeviceModel", null)
                        .WithMany("Labels")
                        .HasForeignKey("DeviceModelId");

                    b.HasOne("IoTHub.Portal.Domain.Entities.EdgeDevice", null)
                        .WithMany("Labels")
                        .HasForeignKey("EdgeDeviceId");

                    b.HasOne("IoTHub.Portal.Domain.Entities.EdgeDeviceModel", null)
                        .WithMany("Labels")
                        .HasForeignKey("EdgeDeviceModelId");
                });

            modelBuilder.Entity("IoTHub.Portal.Domain.Entities.LoRaDeviceTelemetry", b =>
                {
                    b.HasOne("IoTHub.Portal.Domain.Entities.LorawanDevice", null)
                        .WithMany("Telemetry")
                        .HasForeignKey("LorawanDeviceId");
                });

            modelBuilder.Entity("IoTHub.Portal.Domain.Entities.Scope", b =>
                {
                    b.HasOne("IoTHub.Portal.Domain.Entities.Scope", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("IoTHub.Portal.Domain.Entities.User", b =>
                {
                    b.HasOne("IoTHub.Portal.Domain.Entities.Principal", "Principal")
                        .WithOne("User")
                        .HasForeignKey("IoTHub.Portal.Domain.Entities.User", "PrincipalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Principal");
                });

            modelBuilder.Entity("IoTHub.Portal.Domain.Entities.LorawanDevice", b =>
                {
                    b.HasOne("IoTHub.Portal.Domain.Entities.Device", null)
                        .WithOne()
                        .HasForeignKey("IoTHub.Portal.Domain.Entities.LorawanDevice", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("IoTHub.Portal.Domain.Entities.Device", b =>
                {
                    b.Navigation("Labels");

                    b.Navigation("Tags");
                });

            modelBuilder.Entity("IoTHub.Portal.Domain.Entities.DeviceModel", b =>
                {
                    b.Navigation("Labels");
                });

            modelBuilder.Entity("IoTHub.Portal.Domain.Entities.EdgeDevice", b =>
                {
                    b.Navigation("Labels");

                    b.Navigation("Tags");
                });

            modelBuilder.Entity("IoTHub.Portal.Domain.Entities.EdgeDeviceModel", b =>
                {
                    b.Navigation("Labels");
                });

            modelBuilder.Entity("IoTHub.Portal.Domain.Entities.Principal", b =>
                {
                    b.Navigation("AccessControls");

                    b.Navigation("Group");

                    b.Navigation("User");
                });

            modelBuilder.Entity("IoTHub.Portal.Domain.Entities.Role", b =>
                {
                    b.Navigation("Actions");
                });

            modelBuilder.Entity("IoTHub.Portal.Domain.Entities.Scope", b =>
                {
                    b.Navigation("Children");
                });

            modelBuilder.Entity("IoTHub.Portal.Domain.Entities.LorawanDevice", b =>
                {
                    b.Navigation("Telemetry");
                });
#pragma warning restore 612, 618
        }
    }
}
