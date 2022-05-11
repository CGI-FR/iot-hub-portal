// Copyright (c) CGI France. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AzureIoTHub.Portal.Models.v10.LoRaWAN
{
    using System.ComponentModel.DataAnnotations;
    using AzureIoTHub.Portal.Models.v10;

    /// <summary>
    /// LoRa Device base
    /// </summary>
    public class LoRaDeviceBase : DeviceDetails
    {
        /// <summary>
        /// The device identifier.
        /// </summary>
        [Required(ErrorMessage = "The device should have a unique identifier.")]
        [MaxLength(ErrorMessage = "The device identifier should be up to 128 characters long.")]
        [RegularExpression("^[A-Z0-9]{16}$", ErrorMessage = "The device identifier must contain 16 hexadecimal characters (numbers from 0 to 9 and/or letters from A to F)")]
        public override string DeviceID { get; set; }

        /// <summary>
        /// The LoRa device class type.
        /// </summary>
        public ClassType ClassType { get; set; }

        /// <summary>
        /// The status of OTAA setting.
        /// </summary>
        public bool UseOTAA { get; set; }

        /// <summary>
        /// The device OTAA Application eui.
        /// </summary>
        // [Required(ErrorMessage = "The OTAA App EUI is required.")]
        public string AppEUI { get; set; }

        /// <summary>
        /// The sensor decoder API Url.
        /// </summary>
        public string SensorDecoder { get; set; }

        /// <summary>
        /// The GatewayID of the device.
        /// </summary>
        public string GatewayID { get; set; }

        /// <summary>
        /// Allows disabling the downstream (cloud to device) for a device.
        /// By default downstream messages are enabled.
        /// </summary>
        public bool? Downlink { get; set; }

        /// <summary>
        /// Allows setting the device preferred receive window (RX1 or RX2).
        /// The default preferred receive window is 1.
        /// </summary>
        public int? PreferredWindow { get; set; }

        /// <summary>
        /// Allows controlling the handling of duplicate messages received by multiple gateways.
        /// The default is None.
        /// </summary>
        public DeduplicationMode Deduplication { get; set; }

        /// <summary>
        /// Allows setting an offset between received Datarate and retransmit datarate as specified in the LoRa Specifiations.
        /// Valid for OTAA devices.
        /// If an invalid value is provided the network server will use default value 0.
        /// </summary>
        public int? RX1DROffset { get; set; }

        /// <summary>
        /// Allows setting a custom Datarate for second receive windows.
        /// Valid for OTAA devices.
        /// If an invalid value is provided the network server will use default value 0 (DR0).
        /// </summary>
        public int? RX2DataRate { get; set; }

        /// <summary>
        /// Allows setting a custom wait time between receiving and transmission as specified in the specification.
        /// </summary>
        public int? RXDelay { get; set; }

        /// <summary>
        /// Allows to disable the relax mode when using ABP.
        /// By default relaxed mode is enabled.
        /// </summary>
        public bool? ABPRelaxMode { get; set; }

        /// <summary>
        /// Allows to explicitly specify a frame counter up start value.
        /// If the device joins, this value will be used to validate the first frame and initialize the server state for the device.
        /// </summary>
        [Range(0, 4294967295)]
        public int? FCntUpStart { get; set; }

        /// <summary>
        /// Allows to explicitly specify a frame counter down start value.
        /// </summary>
        [Range(0, 4294967295)]
        public int? FCntDownStart { get; set; }

        /// <summary>
        /// Allow the usage of 32bit counters on your device.
        /// </summary>
        public bool? Supports32BitFCnt { get; set; }

        /// <summary>
        /// Allows to reset the frame counters to the FCntUpStart/FCntDownStart values respectively.
        /// </summary>
        [Range(0, 4294967295)]
        public int? FCntResetCounter { get; set; }

        /// <summary>
        /// Allows defining a sliding expiration to the connection between the leaf device and IoT/Edge Hub.
        /// The default is none, which causes the connection to not be dropped.
        /// </summary>
        public int? KeepAliveTimeout { get; set; }

        /// <summary>
        /// Initializes a new instance of the LoRa Device Base class.
        /// </summary>
        public LoRaDeviceBase()
        {
            Downlink = true;
            PreferredWindow = 1;
            Deduplication = DeduplicationMode.None;
            ABPRelaxMode = true;
        }
    }
}
