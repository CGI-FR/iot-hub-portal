// Copyright (c) CGI France. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AzureIoTHub.Portal.Server.Tests.Unit.Pages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using AzureIoTHub.Portal.Client.Pages.DeviceModels;
    using AzureIoTHub.Portal.Client.Shared;
    using AzureIoTHub.Portal.Models;
    using AzureIoTHub.Portal.Models.v10;
    using AzureIoTHub.Portal.Server.Tests.Unit.Helpers;
    using Bunit;
    using Bunit.TestDoubles;
    using Microsoft.AspNetCore.Components;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;
    using MudBlazor;
    using MudBlazor.Interop;
    using MudBlazor.Services;
    using NUnit.Framework;
    using RichardSzalay.MockHttp;
    using AzureIoTHub.Portal.Client.Exceptions;
    using AzureIoTHub.Portal.Client.Models;
    using AzureIoTHub.Portal.Models.v10.LoRaWAN;

    [TestFixture]
    public class CreateDeviceModelPageTests : IDisposable
    {
        private Bunit.TestContext testContext;
        private MockHttpMessageHandler mockHttpClient;

        private MockRepository mockRepository;
        private Mock<IDialogService> mockDialogService;
        private Mock<ISnackbar> mockSnackbarService;

        private static string ApiBaseUrl => "/api/models";
        private static string LorawanApiUrl => "/api/lorawan/models";

        [SetUp]
        public void SetUp()
        {
            this.testContext = new Bunit.TestContext();

            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.mockHttpClient = this.testContext.Services
                                            .AddMockHttpClient();

            this.mockDialogService = this.mockRepository.Create<IDialogService>();
            _ = this.testContext.Services.AddSingleton(this.mockDialogService.Object);

            this.mockSnackbarService = this.mockRepository.Create<ISnackbar>();
            _ = this.testContext.Services.AddSingleton(this.mockSnackbarService.Object);

            _ = this.testContext.Services.AddMudServices();

            _ = this.testContext.JSInterop.SetupVoid("mudKeyInterceptor.connect", _ => true);
            _ = this.testContext.JSInterop.SetupVoid("mudPopover.connect", _ => true);
            _ = this.testContext.JSInterop.SetupVoid("Blazor._internal.InputFile.init", _ => true);
            _ = this.testContext.JSInterop.Setup<BoundingClientRect>("mudElementRef.getBoundingClientRect", _ => true);
            _ = this.testContext.JSInterop.Setup<IEnumerable<BoundingClientRect>>("mudResizeObserver.connect", _ => true);

            this.mockHttpClient.AutoFlush = true;
        }

        private IRenderedComponent<TComponent> RenderComponent<TComponent>(params ComponentParameter[] parameters)
         where TComponent : IComponent
        {
            return this.testContext.RenderComponent<TComponent>(parameters);
        }

        [Test]
        public void ClickOnSaveShouldPostNonLoRaDeviceModelData()
        {
            // Arrange
            var modelName = Guid.NewGuid().ToString();
            var description = Guid.NewGuid().ToString();

            _ = this.testContext.Services.AddSingleton(new PortalSettings { IsLoRaSupported = false });

            _ = this.mockHttpClient.When(HttpMethod.Post, $"{ApiBaseUrl}")
                .With(m =>
                {
                    Assert.IsAssignableFrom<ObjectContent<DeviceModel>>(m.Content);
                    var objectContent = m.Content as ObjectContent<DeviceModel>;
                    Assert.IsNotNull(objectContent);

                    Assert.IsAssignableFrom<DeviceModel>(objectContent.Value);
                    var deviceModel = objectContent.Value as DeviceModel;
                    Assert.IsNotNull(deviceModel);

                    Assert.IsNotNull(deviceModel.ModelId);
                    Assert.AreEqual(deviceModel.Name, modelName);
                    Assert.AreEqual(deviceModel.Description, description);
                    Assert.AreEqual(deviceModel.SupportLoRaFeatures, false);

                    return true;
                })
                .RespondText(string.Empty);

            _ = this.mockHttpClient.When(HttpMethod.Post, $"{ApiBaseUrl}/*/properties")
                .RespondText(string.Empty);

            var mockDialogReference = new DialogReference(Guid.NewGuid(), this.mockDialogService.Object);
            _ = this.mockDialogService.Setup(c => c.Show<ProcessingDialog>("Processing", It.IsAny<DialogParameters>()))
                .Returns(mockDialogReference);
            _ = this.mockDialogService.Setup(c => c.Close(It.Is<DialogReference>(x => x == mockDialogReference)));

            _ = this.mockSnackbarService.Setup(c => c.Add(It.IsAny<string>(), Severity.Success, It.IsAny<Action<SnackbarOptions>>())).Returns((Snackbar)null);

            // Act
            var cut = RenderComponent<CreateDeviceModelPage>();
            var saveButton = cut.WaitForElement("#SaveButton");

            cut.WaitForElement($"#{nameof(DeviceModel.Name)}").Change(modelName);
            cut.WaitForElement($"#{nameof(DeviceModel.Description)}").Change(description);

            saveButton.Click();
            cut.WaitForState(() => this.testContext.Services.GetRequiredService<FakeNavigationManager>().Uri.EndsWith("/device-models", StringComparison.OrdinalIgnoreCase));

            // Assert
            cut.WaitForAssertion(() => this.mockHttpClient.VerifyNoOutstandingExpectation());
            cut.WaitForAssertion(() => this.mockRepository.VerifyAll());
        }

        [Test]
        public void ClickOnSaveShouldProcessProblemDetailsExceptionWhenIssueOccursOnCreatingDeviceModel()
        {
            // Arrange
            var modelName = Guid.NewGuid().ToString();
            var description = Guid.NewGuid().ToString();

            _ = this.testContext.Services.AddSingleton(new PortalSettings { IsLoRaSupported = false });

            _ = this.mockHttpClient.When(HttpMethod.Post, $"{ApiBaseUrl}")
                .Throw(new ProblemDetailsException(new ProblemDetailsWithExceptionDetails()));

            _ = this.mockHttpClient.When(HttpMethod.Post, $"{ApiBaseUrl}/*/properties")
                .RespondText(string.Empty);

            var mockDialogReference = new DialogReference(Guid.NewGuid(), this.mockDialogService.Object);
            _ = this.mockDialogService.Setup(c => c.Show<ProcessingDialog>("Processing", It.IsAny<DialogParameters>()))
                .Returns(mockDialogReference);
            _ = this.mockDialogService.Setup(c => c.Close(It.Is<DialogReference>(x => x == mockDialogReference)));

            // Act
            var cut = RenderComponent<CreateDeviceModelPage>();
            var saveButton = cut.WaitForElement("#SaveButton");

            cut.WaitForElement($"#{nameof(DeviceModel.Name)}").Change(modelName);
            cut.WaitForElement($"#{nameof(DeviceModel.Description)}").Change(description);

            saveButton.Click();
            cut.WaitForState(() => !this.testContext.Services.GetRequiredService<FakeNavigationManager>().Uri.EndsWith("/device-models", StringComparison.OrdinalIgnoreCase));

            // Assert
            cut.WaitForAssertion(() => this.mockHttpClient.VerifyNoOutstandingExpectation());
            cut.WaitForAssertion(() => this.mockRepository.VerifyAll());
        }

        [Test]
        public void ClickOnAddPropertyShouldAddNewProperty()
        {
            // Arrange
            var propertyName = Guid.NewGuid().ToString();
            var displayName = Guid.NewGuid().ToString();

            _ = this.mockHttpClient.When(HttpMethod.Post, $"{ApiBaseUrl}")
                .RespondText(string.Empty);

            _ = this.testContext.Services.AddSingleton(new PortalSettings { IsLoRaSupported = false });

            _ = this.mockHttpClient.When(HttpMethod.Post, $"{ApiBaseUrl}/*/properties")
                .With(m =>
                {
                    Assert.IsAssignableFrom<ObjectContent<List<DeviceProperty>>>(m.Content);
                    var objectContent = m.Content as ObjectContent<List<DeviceProperty>>;
                    Assert.IsNotNull(objectContent);

                    Assert.IsAssignableFrom<List<DeviceProperty>>(objectContent.Value);
                    var properties = objectContent.Value as IEnumerable<DeviceProperty>;
                    Assert.IsNotNull(properties);

                    Assert.AreEqual(1, properties?.Count());

                    var property = properties.Single(x => x.Name == propertyName);

                    Assert.AreEqual(propertyName, property.Name);
                    Assert.AreEqual(displayName, property.DisplayName);
                    Assert.AreEqual(DevicePropertyType.Boolean, property.PropertyType);
                    Assert.IsTrue(property.IsWritable);

                    return true;
                })
                .RespondText(string.Empty);

            var mockDialogReference = new DialogReference(Guid.NewGuid(), this.mockDialogService.Object);
            _ = this.mockDialogService.Setup(c => c.Show<ProcessingDialog>("Processing", It.IsAny<DialogParameters>()))
                .Returns(mockDialogReference);
            _ = this.mockDialogService.Setup(c => c.Close(It.Is<DialogReference>(x => x == mockDialogReference)));

            _ = this.mockSnackbarService.Setup(c => c.Add(It.IsAny<string>(), Severity.Success, It.IsAny<Action<SnackbarOptions>>())).Returns((Snackbar)null);

            // Act
            var cut = RenderComponent<CreateDeviceModelPage>();
            var saveButton = cut.WaitForElement("#SaveButton");
            var addPropertyButton = cut.WaitForElement("#addPropertyButton");

            cut.WaitForElement($"#{nameof(DeviceModel.Name)}").Change(Guid.NewGuid().ToString());
            cut.WaitForElement($"#{nameof(DeviceModel.Description)}").Change(Guid.NewGuid().ToString());

            addPropertyButton.Click();

            cut.WaitForElement($"#property- #{nameof(DeviceProperty.Name)}").Change(propertyName);

            var propertyCssSelector = $"#property-{propertyName}";

            cut.WaitForElement($"{propertyCssSelector} #{nameof(DeviceProperty.DisplayName)}").Change(displayName);
            cut.WaitForElement($"{propertyCssSelector} #{nameof(DeviceProperty.PropertyType)}").Change(nameof(DevicePropertyType.Boolean));
            cut.WaitForElement($"{propertyCssSelector} #{nameof(DeviceProperty.IsWritable)}").Change(true);

            cut.WaitForAssertion(() => Assert.AreEqual(1, cut.FindAll("#deletePropertyButton").Count));
            //Assert.AreEqual(1, cut.FindAll("#deletePropertyButton").Count);

            saveButton.Click();
            cut.WaitForState(() => this.testContext.Services.GetRequiredService<FakeNavigationManager>().Uri.EndsWith("/device-models", StringComparison.OrdinalIgnoreCase));

            // Assert
            cut.WaitForAssertion(() => this.mockHttpClient.VerifyNoOutstandingExpectation());
            cut.WaitForAssertion(() => this.mockRepository.VerifyAll());
        }

        [Test]
        public void ClickOnRemovePropertyShouldRemoveTheProperty()
        {
            // Arrange
            _ = this.testContext.Services.AddSingleton(new PortalSettings { IsLoRaSupported = false });

            _ = this.mockHttpClient.When(HttpMethod.Post, $"{ApiBaseUrl}")
                .RespondText(string.Empty);

            _ = this.mockHttpClient.When(HttpMethod.Post, $"{ApiBaseUrl}/*/properties")
                .With(m =>
                {
                    Assert.IsAssignableFrom<ObjectContent<List<DeviceProperty>>>(m.Content);
                    var objectContent = m.Content as ObjectContent<List<DeviceProperty>>;
                    Assert.IsNotNull(objectContent);

                    Assert.IsAssignableFrom<List<DeviceProperty>>(objectContent.Value);
                    var properties = objectContent.Value as IEnumerable<DeviceProperty>;
                    Assert.IsNotNull(properties);

                    Assert.AreEqual(0, properties?.Count());

                    return true;
                })
                .RespondText(string.Empty);

            var mockDialogReference = new DialogReference(Guid.NewGuid(), this.mockDialogService.Object);
            _ = this.mockDialogService.Setup(c => c.Show<ProcessingDialog>("Processing", It.IsAny<DialogParameters>()))
                .Returns(mockDialogReference);
            _ = this.mockDialogService.Setup(c => c.Close(It.Is<DialogReference>(x => x == mockDialogReference)));

            _ = this.mockSnackbarService.Setup(c => c.Add(It.IsAny<string>(), Severity.Success, It.IsAny<Action<SnackbarOptions>>())).Returns((Snackbar)null);

            // Act
            var cut = RenderComponent<CreateDeviceModelPage>();
            var saveButton = cut.WaitForElement("#SaveButton");

            cut.WaitForElement($"#{nameof(DeviceModel.Name)}").Change(Guid.NewGuid().ToString());
            cut.WaitForElement($"#{nameof(DeviceModel.Description)}").Change(Guid.NewGuid().ToString());

            var addPropertyButton = cut.WaitForElement("#addPropertyButton");
            addPropertyButton.Click();

            cut.WaitForAssertion(()=> Assert.AreEqual(1, cut.FindAll("#deletePropertyButton").Count));

            var removePropertyButton = cut.WaitForElement("#deletePropertyButton");
            removePropertyButton.Click();

            cut.WaitForAssertion(() => Assert.AreEqual(0, cut.FindAll("#deletePropertyButton").Count));

            saveButton.Click();
            cut.WaitForState(() => this.testContext.Services.GetRequiredService<FakeNavigationManager>().Uri.EndsWith("/device-models", StringComparison.OrdinalIgnoreCase));

            // Assert
            cut.WaitForAssertion(() => this.mockHttpClient.VerifyNoOutstandingExpectation());
            cut.WaitForAssertion(() => this.mockRepository.VerifyAll());
        }

        [Test]
        public void WhenLoraFeatureIsDisabledModelDetailsShouldNotDisplayLoRaWANSwitch()
        {
            // Arrange
            _ = this.testContext.Services.AddSingleton(new PortalSettings { IsLoRaSupported = false });

            // Act
            var cut = RenderComponent<CreateDeviceModelPage>();

            // Assert
            cut.WaitForAssertion(() => cut.Find("#form"));
            cut.WaitForAssertion(() => Assert.AreEqual(0, cut.FindAll("#SupportLoRaFeatures").Count));
            cut.WaitForAssertion(() => this.mockHttpClient.VerifyNoOutstandingExpectation());
        }

        [Test]
        public void WhenLoraFeatureIsEnabledModelDetailsShouldDisplayLoRaWANSwitch()
        {
            // Arrange
            _ = this.testContext.Services.AddSingleton(new PortalSettings { IsLoRaSupported = true });

            // Act
            var cut = RenderComponent<CreateDeviceModelPage>();
            //_ = cut.WaitForElement("#form");
            //_ = cut.WaitForElement("#SupportLoRaFeatures");

            // Assert
            cut.WaitForAssertion(() => cut.Find("#form"));
            cut.WaitForAssertion(() => cut.Find("#SupportLoRaFeatures"));
            cut.WaitForAssertion(() => this.mockHttpClient.VerifyNoOutstandingExpectation());
        }

        [Test]
        public void WhenLoraFeatureIsEnabledModelDetailsShouldDisplayLoRaWANTab()
        {
            // Arrange
            _ = this.testContext.Services.AddSingleton(new PortalSettings { IsLoRaSupported = true });

            // Act
            var cut = RenderComponent<CreateDeviceModelPage>();

            // Assert
            cut.WaitForAssertion(() => cut.Find("#form"));
            cut.WaitForElement("#SupportLoRaFeatures").Change(true);

            cut.WaitForState(() => cut.FindAll(".mud-tabs .mud-tab").Count == 2);

            var tabs = cut.WaitForElements(".mud-tabs .mud-tab");

            Assert.AreEqual(2, tabs.Count);
            Assert.AreEqual("General", tabs[0].TextContent);
            Assert.AreEqual("LoRaWAN", tabs[1].TextContent);

            cut.WaitForAssertion(() => this.mockHttpClient.VerifyNoOutstandingExpectation());
        }

        [Test]
        public void ClickOnSaveShouldPostLoRaDeviceModelData()
        {
            // Arrange
            var modelName = Guid.NewGuid().ToString();
            var description = Guid.NewGuid().ToString();

            _ = this.testContext.Services.AddSingleton(new PortalSettings { IsLoRaSupported = true });

            _ = this.mockHttpClient.When(HttpMethod.Post, $"{LorawanApiUrl}")
                .With(m =>
                {
                    Assert.IsAssignableFrom<ObjectContent<LoRaDeviceModel>>(m.Content);
                    var objectContent = m.Content as ObjectContent<LoRaDeviceModel>;
                    Assert.IsNotNull(objectContent);

                    Assert.IsAssignableFrom<LoRaDeviceModel>(objectContent.Value);
                    var deviceModel = objectContent.Value as LoRaDeviceModel;
                    Assert.IsNotNull(deviceModel.ModelId);

                    Assert.AreEqual(deviceModel.Name, modelName);
                    Assert.AreEqual(deviceModel.Description, description);
                    Assert.AreEqual(deviceModel.SupportLoRaFeatures, true);

                    return true;
                })
                .RespondText(string.Empty);

            _ = this.mockHttpClient.When(HttpMethod.Post, $"{LorawanApiUrl}/*/commands")
                .RespondText(string.Empty);

            var mockDialogReference = new DialogReference(Guid.NewGuid(), this.mockDialogService.Object);
            _ = this.mockDialogService.Setup(c => c.Show<ProcessingDialog>("Processing", It.IsAny<DialogParameters>()))
                .Returns(mockDialogReference);
            _ = this.mockDialogService.Setup(c => c.Close(It.Is<DialogReference>(x => x == mockDialogReference)));

            _ = this.mockSnackbarService.Setup(c => c.Add(It.IsAny<string>(), Severity.Success, It.IsAny<Action<SnackbarOptions>>())).Returns((Snackbar)null);

            // Act
            var cut = RenderComponent<CreateDeviceModelPage>();
            var saveButton = cut.WaitForElement("#SaveButton");

            cut.WaitForElement("#SupportLoRaFeatures").Change(true);
            cut.WaitForState(() => cut.FindAll(".mud-tabs .mud-tab").Count == 2);

            cut.WaitForElement($"#{nameof(DeviceModel.Name)}").Change(modelName);
            cut.WaitForElement($"#{nameof(DeviceModel.Description)}").Change(description);
            (cut.Instance.Model as LoRaDeviceModel).AppEUI = "AppEUI";

            saveButton.Click();
            cut.WaitForState(() => this.testContext.Services.GetRequiredService<FakeNavigationManager>().Uri.EndsWith("/device-models", StringComparison.OrdinalIgnoreCase));

            // Assert            
            cut.WaitForAssertion(() => this.mockHttpClient.VerifyNoOutstandingExpectation());
            cut.WaitForAssertion(() => this.mockRepository.VerifyAll());
        }

        //[Test]
        //public void WhenLoraDeviceModelDetailsShouldCallLoRaAPIs()
        //{
        //    // Arrange
        //    _ = this.testContext.Services.AddSingleton(new PortalSettings { IsLoRaSupported = true });

        //    var cut = RenderComponent<CreateDeviceModelPage>();
        //    _ = cut.WaitForElement("#form");

        //    cut.Find($"#{nameof(DeviceModel.Name)}").Change(Guid.NewGuid().ToString());
        //    cut.Find($"#{nameof(DeviceModel.Description)}").Change(Guid.NewGuid().ToString());

        //    cut.WaitForElement("#SupportLoRaFeatures")
        //        .Change(true);

        //    var tabs = cut.FindAll(".mud-tabs .mud-tab");
        //    var loraTab = tabs[1];
        //    loraTab.Click();
        //    var actualCreateLoraDeviceModel = cut.FindComponent<CreateLoraDeviceModel>();

        //    // cut.Render();
        //    // cut.FindComponent<CreateLoraDeviceModel>()
        //    //    .Find($"#{nameof(LoRaDeviceModel.AppEUI)}")
        //    //    .Change(Guid.NewGuid().ToString());

        //    _ = this.mockHttpClient.When(HttpMethod.Post, $"{LorawanApiUrl}")
        //        .RespondText(string.Empty);

        //    _ = this.mockHttpClient.When(HttpMethod.Post, $"{ LorawanApiUrl }/*/commands")
        //        .RespondText(string.Empty);

        //    _ = this.mockHttpClient.When(HttpMethod.Post, $"{ LorawanApiUrl }/*/avatar")
        //        .RespondText(string.Empty);

        //    var saveButton = cut.WaitForElement("#SaveButton");

        //    // Act
        //    saveButton.Click();
        //    cut.WaitForState(() => this.testContext.Services.GetRequiredService<FakeNavigationManager>().Uri.EndsWith("/device-models", StringComparison.OrdinalIgnoreCase));

        //    // Assert
        //    this.mockHttpClient.VerifyNoOutstandingExpectation();
        //}

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
