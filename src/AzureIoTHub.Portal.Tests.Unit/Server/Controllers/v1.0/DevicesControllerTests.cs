// Copyright (c) CGI France. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AzureIoTHub.Portal.Tests.Unit.Server.Controllers.v1._0
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AzureIoTHub.Portal.Domain;
    using AzureIoTHub.Portal.Domain.Exceptions;
    using AzureIoTHub.Portal.Domain.Repositories;
    using AzureIoTHub.Portal.Server.Controllers.V10;
    using AzureIoTHub.Portal.Server.Mappers;
    using AzureIoTHub.Portal.Server.Services;
    using FluentAssertions;
    using Hellang.Middleware.ProblemDetails;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Controllers;
    using Microsoft.AspNetCore.Mvc.Routing;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Azure.Devices;
    using Microsoft.Azure.Devices.Shared;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Primitives;
    using Models.v10;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class DevicesControllerTests
    {
        private MockRepository mockRepository;

        private Mock<IUrlHelper> mockUrlHelper;
        private Mock<ILogger<DevicesController>> mockLogger;
        private Mock<IExternalDeviceService> mockDeviceService;
        private Mock<IDevicePropertyService> mockDevicePropertyService;
        private Mock<IDeviceTagService> mockDeviceTagService;
        private Mock<IDeviceTwinMapper<DeviceListItem, DeviceDetails>> mockDeviceTwinMapper;

        private Mock<IDeviceService<DeviceDetails>> mockGenericDeviceService;

        [SetUp]
        public void SetUp()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);

            this.mockLogger = this.mockRepository.Create<ILogger<DevicesController>>();
            this.mockDeviceService = this.mockRepository.Create<IExternalDeviceService>();
            this.mockDevicePropertyService = this.mockRepository.Create<IDevicePropertyService>();
            this.mockDeviceTagService = this.mockRepository.Create<IDeviceTagService>();
            this.mockDeviceTwinMapper = this.mockRepository.Create<IDeviceTwinMapper<DeviceListItem, DeviceDetails>>();
            this.mockUrlHelper = this.mockRepository.Create<IUrlHelper>();
            this.mockGenericDeviceService = this.mockRepository.Create<IDeviceService<DeviceDetails>>();
        }

        private DevicesController CreateDevicesController()
        {
            return new DevicesController(
                this.mockLogger.Object,
                this.mockDevicePropertyService.Object,
                this.mockGenericDeviceService.Object)
            {
                Url = this.mockUrlHelper.Object
            };
        }

        [Test]
        public async Task GetListStateUnderTestExpectedBehavior()
        {
            // Arrange
            var devicesController = CreateDevicesController();
            const int count = 100;
            var twinCollection = new TwinCollection();

            twinCollection["deviceType"] = "test";

            var mockTagSearch = new StringValues("test");

            var mockQueryCollection = new QueryCollection(new Dictionary<string, StringValues>()
            {
                { "tag.deviceType", mockTagSearch }
            });

            var mockHttpRequest = this.mockRepository.Create<HttpRequest>();
            var mockHttpContext = this.mockRepository.Create<HttpContext>();

            _ = mockHttpContext.SetupGet(c => c.Request)
                .Returns(mockHttpRequest.Object);

            _ = mockHttpRequest.Setup(c => c.Query)
                .Returns(mockQueryCollection);

            devicesController.ControllerContext = new ControllerContext(
                new ActionContext(mockHttpContext.Object, new RouteData(), new ControllerActionDescriptor()));

            _ = this.mockDeviceService.Setup(c => c.GetAllDevice(
                    It.Is<string>(x => x == "aaa"),
                    It.Is<string>(x => string.IsNullOrEmpty(x)),
                    It.Is<string>(x => x == "LoRa Concentrator"),
                    It.Is<string>(x => x == "bbb"),
                    It.Is<bool?>(x => x == true),
                    It.Is<bool?>(x => x == false),
                    It.Is<Dictionary<string, string>>(x => x["deviceType"] == "test"),
                    It.Is<int>(x => x == 2)))
                .ReturnsAsync(new PaginationResult<Twin>
                {
                    Items = Enumerable.Range(0, 100).Select(x => new Twin
                    {
                        DeviceId = FormattableString.Invariant($"{x}"),
                        Tags = twinCollection
                    }),
                    TotalItems = 1000,
                    NextPage = Guid.NewGuid().ToString()
                });

            _ = this.mockDeviceTwinMapper.Setup(c => c.CreateDeviceListItem(It.IsAny<Twin>()))
                .Returns<Twin>((x) => new DeviceListItem
                {
                    DeviceID = x.DeviceId
                });

            _ = this.mockDeviceTagService.Setup(c => c.GetAllSearchableTagsNames())
                .Returns(new string[] { "deviceType" });

            _ = this.mockUrlHelper.Setup(c => c.RouteUrl(It.IsAny<UrlRouteContext>()))
                .Returns(Guid.NewGuid().ToString());

            // Act

            var result = await devicesController.SearchItems(
                searchText: "bbb",
                searchStatus: true,
                searchState: false,
                pageSize: 2);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(count, result.Items.Count());
            Assert.AreEqual(1000, result.TotalItems);
            this.mockRepository.VerifyAll();
        }

        [Test]
        public async Task GetItemStateUnderTestExpectedBehavior()
        {
            // Arrange
            var devicesController = CreateDevicesController();
            const string deviceID = "aaa";

            _ = this.mockDeviceService.Setup(c => c.GetDeviceTwin(It.Is<string>(x => x == deviceID)))
                .Returns<string>(x => Task.FromResult(new Twin(x)));

            _ = this.mockDeviceTwinMapper.Setup(c => c.CreateDeviceDetails(It.IsAny<Twin>(), It.IsAny<IEnumerable<string>>()))
                .Returns<Twin, IEnumerable<string>>((x, _) => new DeviceDetails
                {
                    DeviceID = x.DeviceId
                });

            _ = this.mockDeviceTagService.Setup(c => c.GetAllTagsNames())
                .Returns(new List<string>());

            // Act
            var result = await devicesController.GetItem(deviceID);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(deviceID, result.DeviceID);
            this.mockRepository.VerifyAll();
        }

        [Test]
        public async Task CreateDeviceAsyncStateUnderTestExpectedBehavior()
        {
            // Arrange
            var devicesController = CreateDevicesController();
            var device = new DeviceDetails
            {
                DeviceID = "aaa",
            };

            Twin twin = null;

            _ = this.mockDeviceService.Setup(c => c.CreateNewTwinFromDeviceId(It.IsAny<string>()))
                .ReturnsAsync(new Twin { DeviceId = device.DeviceID });

            _ = this.mockDeviceTwinMapper.Setup(c => c.UpdateTwin(It.Is<Twin>(x => x.DeviceId == device.DeviceID), It.Is<DeviceDetails>(x => x == device)))
                .Callback<Twin, DeviceDetails>((t, _) => twin = t);

            _ = this.mockDeviceService.Setup(c => c.CreateDeviceWithTwin(It.Is<string>(x => x == device.DeviceID), It.Is<bool>(x => !x), It.Is<Twin>(x => x == twin), It.Is<DeviceStatus>(x => x == (device.IsEnabled ? DeviceStatus.Enabled : DeviceStatus.Disabled))))
                .ReturnsAsync(new BulkRegistryOperationResult { IsSuccessful = true });

            // Act
            var result = await devicesController.CreateDeviceAsync(device);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsAssignableFrom<OkObjectResult>(result);
            this.mockRepository.VerifyAll();
        }

        [Test]
        public async Task CreateDeviceAsyncModelNotValidShouldReturnBadRequest()
        {
            // Arrange
            var devicesController = CreateDevicesController();
            var device = new DeviceDetails
            {
                DeviceID = "aaa",
            };

            devicesController.ModelState.AddModelError("Key", "Device model is invalid");

            // Act
            var act = () => devicesController.CreateDeviceAsync(device);

            // Assert
            _ = await act.Should().ThrowAsync<ProblemDetailsException>();
            this.mockRepository.VerifyAll();
        }

        [Test]
        public async Task UpdateDeviceAsyncStateUnderTestExpectedBehavior()
        {
            // Arrange
            var devicesController = CreateDevicesController();
            var device = new DeviceDetails
            {
                DeviceID = "aaa"
            };
            var item = new Device(device.DeviceID);
            var twin = new Twin(device.DeviceID);

            _ = this.mockDeviceService.Setup(c => c.GetDevice(It.Is<string>(x => x == device.DeviceID)))
                .ReturnsAsync(item);
            _ = this.mockDeviceService.Setup(c => c.GetDeviceTwin(It.Is<string>(x => x == device.DeviceID)))
                .ReturnsAsync(twin);
            _ = this.mockDeviceService.Setup(c => c.UpdateDevice(It.Is<Device>(x => x.Id == item.Id)))
                .ReturnsAsync(item);
            _ = this.mockDeviceService.Setup(c => c.UpdateDeviceTwin(It.Is<Twin>(x => x.DeviceId == device.DeviceID)))
                .ReturnsAsync(twin);

            _ = this.mockDeviceTwinMapper.Setup(x => x.UpdateTwin(It.Is<Twin>(x => x == twin), It.Is<DeviceDetails>(x => x == device)));

            // Act
            var result = await devicesController.UpdateDeviceAsync(device);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsAssignableFrom<OkResult>(result);
            this.mockRepository.VerifyAll();
        }

        [Test]
        public async Task UpdateDeviceAsyncModelNotValidShouldReturnBadRequest()
        {
            // Arrange
            var devicesController = CreateDevicesController();
            var device = new DeviceDetails
            {
                DeviceID = "aaa"
            };

            devicesController.ModelState.AddModelError("Key", "Device model is invalid");

            // Act
            var act = () => devicesController.UpdateDeviceAsync(device);

            // Assert
            _ = await act.Should().ThrowAsync<ProblemDetailsException>();
            this.mockRepository.VerifyAll();
        }

        [Test]
        public async Task DeleteStateUnderTestExpectedBehavior()
        {
            // Arrange
            var devicesController = CreateDevicesController();
            const string deviceID = "aaa";

            _ = this.mockDeviceService.Setup(c => c.DeleteDevice(It.Is<string>(x => x == deviceID)))
                .Returns(Task.CompletedTask);

            // Act
            var result = await devicesController.Delete(deviceID);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsAssignableFrom<OkResult>(result);
            this.mockRepository.VerifyAll();
        }

        [Test]
        public async Task GetEnrollmentCredentialsShouldReturnEnrollmentCredentials()
        {
            // Arrange
            var devicesController = CreateDevicesController();


            var mockRegistrationCredentials = new EnrollmentCredentials
            {
                RegistrationID = "aaa",
                SymmetricKey = "dfhjkfdgh"
            };

            _ = this.mockDeviceService.Setup(c => c.GetEnrollmentCredentials("aaa"))
                .ReturnsAsync(mockRegistrationCredentials);

            // Act
            var response = await devicesController.GetCredentials("aaa");

            // Assert
            Assert.IsNotNull(response);
            Assert.IsAssignableFrom<OkObjectResult>(response.Result);

            var okObjectResult = (OkObjectResult)response.Result;
            Assert.IsNotNull(okObjectResult);
            Assert.AreEqual(mockRegistrationCredentials, okObjectResult.Value);

            this.mockRepository.VerifyAll();
        }

        [Test]
        public async Task CreateDeviceAsyncDuplicatedDeviceIdShouldThrowInternalServerErrorException()
        {
            // Arrange
            var devicesController = CreateDevicesController();

            _ = this.mockDeviceService.Setup(c => c.CreateNewTwinFromDeviceId(It.IsAny<string>()))
                .ThrowsAsync(new InternalServerErrorException("test"));

            // Act
            var result = async () => await devicesController.CreateDeviceAsync(new DeviceDetails());

            // Assert
            _ = await result.Should().ThrowAsync<InternalServerErrorException>();
            this.mockRepository.VerifyAll();
        }

        [Test]
        public async Task GetPropertiesShouldReturnDevicePropertyValues()
        {
            // Arrange
            var devicesController = CreateDevicesController();

            var deviceId = Guid.NewGuid().ToString();

            var expectedDevicePropertyValues = new List<DevicePropertyValue>
            {
                new()
                {
                    Name = Guid.NewGuid().ToString()
                }
            };

            _ = this.mockDevicePropertyService.Setup(c => c.GetProperties(deviceId))
                .ReturnsAsync(expectedDevicePropertyValues);

            // Act
            var result = await devicesController.GetProperties(deviceId);

            // Assert
            _ = result.Count().Should().Be(1);
            _ = result.Should().BeEquivalentTo(expectedDevicePropertyValues);

            this.mockRepository.VerifyAll();
        }

        [Test]
        public async Task SetPropertiesShouldSetDevicePropertyValues()
        {
            // Arrange
            var devicesController = CreateDevicesController();

            var deviceId = Guid.NewGuid().ToString();

            var devicePropertyValues = new List<DevicePropertyValue>
            {
                new()
                {
                    Name = Guid.NewGuid().ToString()
                }
            };

            _ = this.mockDevicePropertyService.Setup(c => c.SetProperties(deviceId, devicePropertyValues))
                .Returns(Task.CompletedTask);

            // Act
            var result = await devicesController.SetProperties(deviceId, devicePropertyValues);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsAssignableFrom<OkResult>(result.Result);

            var okResult = (OkResult)result.Result;
            Assert.IsNotNull(okResult);

            this.mockRepository.VerifyAll();
        }
    }
}
