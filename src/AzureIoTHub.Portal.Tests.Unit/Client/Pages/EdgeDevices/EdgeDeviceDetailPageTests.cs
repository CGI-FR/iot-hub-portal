// Copyright (c) CGI France. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AzureIoTHub.Portal.Tests.Unit.Client.Pages.EdgeDevices
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AzureIoTHub.Portal.Client.Exceptions;
    using AzureIoTHub.Portal.Client.Models;
    using AzureIoTHub.Portal.Client.Pages.EdgeDevices;
    using AzureIoTHub.Portal.Client.Services;
    using Models.v10;
    using UnitTests.Bases;
    using Bunit;
    using Bunit.TestDoubles;
    using FluentAssertions;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;
    using MudBlazor;
    using MudBlazor.Services;
    using NUnit.Framework;
    using UnitTests.Mocks;

    [TestFixture]
    public class EdgeDeviceDetailPageTests : BlazorUnitTest
    {
        private Mock<IDialogService> mockDialogService;
        private Mock<ISnackbar> mockSnackbarService;
        private Mock<IEdgeDeviceClientService> mockEdgeDeviceClientService;
        private Mock<IEdgeModelClientService> mockIEdgeModelClientService;
        private Mock<IDeviceTagSettingsClientService> mockDeviceTagSettingsClientService;

        private readonly string mockdeviceId = Guid.NewGuid().ToString();

        private FakeNavigationManager mockNavigationManager;

        public override void Setup()
        {
            base.Setup();

            this.mockDialogService = MockRepository.Create<IDialogService>();
            this.mockSnackbarService = MockRepository.Create<ISnackbar>();
            this.mockEdgeDeviceClientService = MockRepository.Create<IEdgeDeviceClientService>();
            this.mockIEdgeModelClientService = MockRepository.Create<IEdgeModelClientService>();
            this.mockDeviceTagSettingsClientService = MockRepository.Create<IDeviceTagSettingsClientService>();

            _ = Services.AddSingleton(this.mockEdgeDeviceClientService.Object);
            _ = Services.AddSingleton(this.mockIEdgeModelClientService.Object);
            _ = Services.AddSingleton(this.mockDeviceTagSettingsClientService.Object);
            _ = Services.AddSingleton(this.mockDialogService.Object);
            _ = Services.AddSingleton(this.mockSnackbarService.Object);

            _ = Services.AddSingleton(new PortalSettings { IsLoRaSupported = false });

            Services.Add(new ServiceDescriptor(typeof(IResizeObserver), new MockResizeObserver()));

            this.mockNavigationManager = Services.GetRequiredService<FakeNavigationManager>();
        }

        [Test]
        public void ReturnButtonMustNavigateToPreviousPage()
        {
            // Arrange
            _ = SetupOnInitialisation();

            var cut = RenderComponent<EdgeDeviceDetailPage>(ComponentParameter.CreateParameter("deviceId", this.mockdeviceId));
            _ = cut.WaitForElement("#saveButton");

            // Act
            cut.WaitForElement("#returnButton").Click();

            // Assert
            cut.WaitForAssertion(() => this.mockNavigationManager.Uri.Should().EndWith("/edge/devices"));
            cut.WaitForAssertion(() => MockRepository.VerifyAll());
        }

        [Test]
        public void ClickOnSaveShouldPutEdgeDeviceDetails()
        {
            var mockIoTEdgeDevice = SetupOnInitialisation();

            _ = this.mockEdgeDeviceClientService.Setup(service =>
                    service.UpdateDevice(It.Is<IoTEdgeDevice>(device =>
                        mockIoTEdgeDevice.DeviceId.Equals(device.DeviceId, StringComparison.Ordinal))))
                .Returns(Task.CompletedTask);

            _ = this.mockSnackbarService.Setup(c => c.Add($"Device {this.mockdeviceId} has been successfully updated!\r\nPlease note that changes might take some minutes to be visible in the list...", Severity.Success, null)).Returns((Snackbar)null);

            var cut = RenderComponent<EdgeDeviceDetailPage>(ComponentParameter.CreateParameter("deviceId", this.mockdeviceId));

            // Act
            cut.WaitForElement("#saveButton").Click();

            // Assert
            cut.WaitForAssertion(() => MockRepository.VerifyAll());
        }

        [Test]
        public void EdgeDeviceDetailPageShouldProcessProblemDetailsExceptionWhenIssueOccursOnLoadDevice()
        {
            // Arrange
            _ = this.mockEdgeDeviceClientService.Setup(service => service.GetDevice(this.mockdeviceId))
                .ThrowsAsync(new ProblemDetailsException(new ProblemDetailsWithExceptionDetails()));

            // Act
            var cut = RenderComponent<EdgeDeviceDetailPage>(ComponentParameter.CreateParameter("deviceId", this.mockdeviceId));
            _ = cut.WaitForElement("form");

            // Assert
            cut.WaitForAssertion(() => MockRepository.VerifyAll());
        }

        [Test]
        public void EdgeDeviceDetailPageShouldProcessProblemDetailsExceptionWhenIssueOccursOnUpdateDevice()
        {
            // Arrange
            var mockIoTEdgeDevice = SetupOnInitialisation();

            _ = this.mockEdgeDeviceClientService.Setup(service =>
                    service.UpdateDevice(It.Is<IoTEdgeDevice>(device =>
                        mockIoTEdgeDevice.DeviceId.Equals(device.DeviceId, StringComparison.Ordinal))))
                .ThrowsAsync(new ProblemDetailsException(new ProblemDetailsWithExceptionDetails()));

            var cut = RenderComponent<EdgeDeviceDetailPage>(
                ComponentParameter.CreateParameter("deviceId", this.mockdeviceId));
            cut.WaitForAssertion(() => cut.Find("form").Should().NotBeNull());

            // Act
            cut.Find("#saveButton").Click();

            // Assert
            cut.WaitForAssertion(() => MockRepository.VerifyAll());
        }

        [Test]
        public void ClickOnRebootShouldRebootModule()
        {
            var mockIoTEdgeModule = new IoTEdgeModule()
            {
                ModuleName = Guid.NewGuid().ToString()
            };

            var mockIoTEdgeDevice = new IoTEdgeDevice()
            {
                DeviceId = this.mockdeviceId,
                ConnectionState = "Connected",
                Modules= new List<IoTEdgeModule>(){mockIoTEdgeModule},
                ModelId = Guid.NewGuid().ToString()
            };


            _ = this.mockEdgeDeviceClientService.Setup(service => service.GetDevice(this.mockdeviceId))
                .ReturnsAsync(mockIoTEdgeDevice);

            _ = this.mockIEdgeModelClientService
                .Setup(service => service.GetIoTEdgeModel(It.Is<string>(x => x.Equals(mockIoTEdgeDevice.ModelId, StringComparison.Ordinal))))
                .ReturnsAsync(new IoTEdgeModel());

            _ = this.mockDeviceTagSettingsClientService.Setup(service => service.GetDeviceTags()).ReturnsAsync(new List<DeviceTag>());

            _ = this.mockEdgeDeviceClientService.Setup(service => service.ExecuteModuleMethod(this.mockdeviceId, It.Is<IoTEdgeModule>(module => mockIoTEdgeModule.ModuleName.Equals(module.ModuleName, StringComparison.Ordinal)), "RestartModule"))
                .ReturnsAsync(new C2Dresult()
                {
                    Payload = "ABC",
                    Status = 200
                });

            _ = this.mockSnackbarService.Setup(c => c.Add("Command successfully executed.", Severity.Success, null)).Returns((Snackbar)null);

            var cut = RenderComponent<EdgeDeviceDetailPage>(ComponentParameter.CreateParameter("deviceId", this.mockdeviceId));

            var rebootButton = cut.WaitForElement("#rebootModule");

            // Act
            rebootButton.Click();

            // Assert
            cut.WaitForAssertion(() => MockRepository.VerifyAll());
        }

        [Test]
        public void ClickOnRebootShouldDisplaySnackbarIfError()
        {
            var mockIoTEdgeModule = new IoTEdgeModule()
            {
                ModuleName = Guid.NewGuid().ToString()
            };

            var mockIoTEdgeDevice = new IoTEdgeDevice()
            {
                DeviceId = this.mockdeviceId,
                ConnectionState = "Connected",
                Modules= new List<IoTEdgeModule>(){mockIoTEdgeModule},
                ModelId = Guid.NewGuid().ToString()
            };

            _ = this.mockEdgeDeviceClientService.Setup(service => service.GetDevice(this.mockdeviceId))
                .ReturnsAsync(mockIoTEdgeDevice);

            _ = this.mockIEdgeModelClientService
                .Setup(service => service.GetIoTEdgeModel(It.Is<string>(x => x.Equals(mockIoTEdgeDevice.ModelId, StringComparison.Ordinal))))
                .ReturnsAsync(new IoTEdgeModel());

            _ = this.mockDeviceTagSettingsClientService.Setup(service => service.GetDeviceTags()).ReturnsAsync(new List<DeviceTag>());

            _ = this.mockEdgeDeviceClientService.Setup(service => service.ExecuteModuleMethod(this.mockdeviceId, It.Is<IoTEdgeModule>(module => mockIoTEdgeModule.ModuleName.Equals(module.ModuleName, StringComparison.Ordinal)), "RestartModule"))
                .ReturnsAsync(new C2Dresult()
                {
                    Payload = "ABC",
                    Status = 500
                });

            _ = this.mockSnackbarService.Setup(c => c.Add(It.IsAny<string>(), Severity.Error, It.IsAny<Action<SnackbarOptions>>())).Returns((Snackbar)null);

            var cut = RenderComponent<EdgeDeviceDetailPage>(ComponentParameter.CreateParameter("deviceId", this.mockdeviceId));

            var rebootButton = cut.WaitForElement("#rebootModule");

            // Act
            rebootButton.Click();

            // Assert
            cut.WaitForAssertion(() => MockRepository.VerifyAll());
        }

        [Test]
        public void EdgeDeviceDetailPageShouldProcessProblemDetailsExceptionWhenIssueOccursOnClickOnReboot()
        {
            // Arrange
            var mockIoTEdgeModule = new IoTEdgeModule()
            {
                ModuleName = Guid.NewGuid().ToString()
            };

            var mockIoTEdgeDevice = new IoTEdgeDevice()
            {
                DeviceId = this.mockdeviceId,
                ConnectionState = "Connected",
                Modules= new List<IoTEdgeModule>(){mockIoTEdgeModule},
                ModelId = Guid.NewGuid().ToString(),
            };

            _ = this.mockEdgeDeviceClientService.Setup(service => service.GetDevice(this.mockdeviceId))
                .ReturnsAsync(mockIoTEdgeDevice);

            _ = this.mockIEdgeModelClientService
                .Setup(service => service.GetIoTEdgeModel(It.Is<string>(x => x.Equals(mockIoTEdgeDevice.ModelId, StringComparison.Ordinal))))
                .ReturnsAsync(new IoTEdgeModel());

            _ = this.mockDeviceTagSettingsClientService.Setup(service => service.GetDeviceTags()).ReturnsAsync(new List<DeviceTag>());

            _ = this.mockEdgeDeviceClientService.Setup(service => service.ExecuteModuleMethod(this.mockdeviceId, It.Is<IoTEdgeModule>(module => mockIoTEdgeModule.ModuleName.Equals(module.ModuleName, StringComparison.Ordinal)), "RestartModule"))
                .ThrowsAsync(new ProblemDetailsException(new ProblemDetailsWithExceptionDetails()));

            var cut = RenderComponent<EdgeDeviceDetailPage>(ComponentParameter.CreateParameter("deviceId", this.mockdeviceId));

            // Act
            cut.WaitForElement("#rebootModule").Click();

            // Assert
            cut.WaitForAssertion(() => MockRepository.VerifyAll());
        }

        [Test]
        public void ClickOnLogsShouldDisplayLogs()
        {
            var mockIoTEdgeModule = new IoTEdgeModule()
            {
                ModuleName = Guid.NewGuid().ToString()
            };

            var mockIoTEdgeDevice = new IoTEdgeDevice()
            {
                DeviceId = this.mockdeviceId,
                ConnectionState = "Connected",
                Modules= new List<IoTEdgeModule>(){mockIoTEdgeModule},
                ModelId = Guid.NewGuid().ToString()
            };

            _ = this.mockEdgeDeviceClientService.Setup(service => service.GetDevice(this.mockdeviceId))
                .ReturnsAsync(mockIoTEdgeDevice);

            _ = this.mockIEdgeModelClientService
                .Setup(service => service.GetIoTEdgeModel(It.Is<string>(x => x.Equals(mockIoTEdgeDevice.ModelId, StringComparison.Ordinal))))
                .ReturnsAsync(new IoTEdgeModel());

            _ = this.mockDeviceTagSettingsClientService.Setup(service => service.GetDeviceTags()).ReturnsAsync(new List<DeviceTag>());

            var mockDialogReference = new DialogReference(Guid.NewGuid(), this.mockDialogService.Object);

            _ = this.mockDialogService.Setup(c => c.Show<ModuleLogsDialog>(It.IsAny<string>(), It.IsAny<DialogParameters>()))
                .Returns(mockDialogReference);

            var cut = RenderComponent<EdgeDeviceDetailPage>(ComponentParameter.CreateParameter("deviceId", this.mockdeviceId));

            var logsButton = cut.WaitForElement("#showLogs");

            // Act
            logsButton.Click();

            // Assert
            cut.WaitForAssertion(() => MockRepository.VerifyAll());
        }

        [Test]
        public void ClickOnConnectShouldDisplayDeviceCredentials()
        {
            var mockIoTEdgeDevice = SetupOnInitialisation();

            var cut = RenderComponent<EdgeDeviceDetailPage>(ComponentParameter.CreateParameter("deviceId", this.mockdeviceId));
            cut.WaitForAssertion(() => cut.Find("#connectButton"));

            var connectButton = cut.Find("#connectButton");

            var mockDialogReference = new DialogReference(Guid.NewGuid(), this.mockDialogService.Object);

            _ = this.mockDialogService.Setup(c => c.Show<ConnectionStringDialog>(It.IsAny<string>(), It.IsAny<DialogParameters>()))
                .Returns(mockDialogReference);

            // Act
            connectButton.Click();

            // Assert
            cut.WaitForAssertion(() => MockRepository.VerifyAll());
        }

        [Test]
        public void ClickOnDeleteShouldDisplayConfirmationDialogAndReturnIfAborted()
        {
            var mockIoTEdgeDevice = SetupOnInitialisation();

            var mockDialogReference = MockRepository.Create<IDialogReference>();
            _ = mockDialogReference.Setup(c => c.Result).ReturnsAsync(DialogResult.Cancel());

            _ = this.mockDialogService.Setup(c => c.Show<EdgeDeviceDeleteConfirmationDialog>(It.IsAny<string>(), It.IsAny<DialogParameters>()))
                .Returns(mockDialogReference.Object);

            var cut = RenderComponent<EdgeDeviceDetailPage>(ComponentParameter.CreateParameter("deviceId", this.mockdeviceId));

            var deleteButton = cut.WaitForElement("#deleteButton");

            // Act
            deleteButton.Click();

            // Assert
            cut.WaitForAssertion(() => MockRepository.VerifyAll());
        }

        [Test]
        public void ClickOnDeleteShouldDisplayConfirmationDialogAndRedirectIfConfirmed()
        {
            var mockIoTEdgeDevice = SetupOnInitialisation();

            var mockDialogReference = MockRepository.Create<IDialogReference>();
            _ = mockDialogReference.Setup(c => c.Result).ReturnsAsync(DialogResult.Ok("Ok"));

            _ = this.mockDialogService.Setup(c => c.Show<EdgeDeviceDeleteConfirmationDialog>(It.IsAny<string>(), It.IsAny<DialogParameters>()))
                .Returns(mockDialogReference.Object);

            var cut = RenderComponent<EdgeDeviceDetailPage>(ComponentParameter.CreateParameter("deviceId", this.mockdeviceId));

            var deleteButton = cut.WaitForElement("#deleteButton");

            // Act
            deleteButton.Click();

            // Assert
            cut.WaitForAssertion(() => this.mockNavigationManager.Uri.Should().EndWith("/edge/devices"));
            cut.WaitForAssertion(() => MockRepository.VerifyAll());
        }

        private IoTEdgeDevice SetupOnInitialisation()
        {
            var tags = new Dictionary<string, string>()
            {
                {"test01", "test" }
            };

            var mockIoTEdgeDevice = new IoTEdgeDevice()
            {
                DeviceId = this.mockdeviceId,
                ConnectionState = "Connected",
                ModelId = Guid.NewGuid().ToString(),
                Tags = tags
            };

            _ = this.mockEdgeDeviceClientService.Setup(service => service.GetDevice(this.mockdeviceId))
                .ReturnsAsync(mockIoTEdgeDevice);

            _ = this.mockIEdgeModelClientService
                .Setup(service => service.GetIoTEdgeModel(It.Is<string>(x => x.Equals(mockIoTEdgeDevice.ModelId, StringComparison.Ordinal))))
                .ReturnsAsync(new IoTEdgeModel());

            _ = this.mockDeviceTagSettingsClientService.Setup(service => service.GetDeviceTags()).ReturnsAsync(new List<DeviceTag>());

            return mockIoTEdgeDevice;
        }
    }
}
