using CommonLogic.BusinessLogic.Interfaces;
using CommonLogic.Core.Models;
using CommonLogic.Services.Implementations;
using CommonLogic.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Stand4.Services
{
    public class TestExecutionService : ITestExecutionService
    {
        private readonly IModbusService modbusService;
        private readonly IDataManager _dataManager;
        private readonly IModbusPollingService modbusPolling;
        Device deviceToTest;
        private double innerTestStatus = 0;
        private double stepNumber = 0;

        public event Action<SensorReading> TestStatusUpdated;
        private enum TestState
        {
            Idle,
            Test1,
            Test2,
            Test3,
            TestFull,
        }
        private TestState currentState = TestState.Idle;
        public TestExecutionService(IModbusService modbusService, IDataManager dataManager, IModbusPollingService mbPolling)
        {
            this.modbusService = modbusService;
            _dataManager = dataManager;
            this.modbusPolling = mbPolling;
            this.modbusPolling.DataReceived += OnDataReceived;
        }
        private void OnDataReceived(Dictionary<string, SensorReading> readings)
        {
            if (readings == null) return;
            innerTestStatus = readings.ContainsKey("InnerTestStatus") ? readings["InnerTestStatus"].Value : innerTestStatus;
            stepNumber = readings.ContainsKey("StepNumber") ? readings["StepNumber"].Value : stepNumber;
            if (readings.TryGetValue("InnerTestStatus", out var statusReading))
            {
                CheckInnerStatus(statusReading);
            }
        }
        private void CheckInnerStatus(SensorReading statusReading)
        {

            if (statusReading.Value == 0)
            {
                    currentState = TestState.Idle;
            }
            TestStatusUpdated?.Invoke(statusReading);
        }

        private async Task RunFirstTestLogic(Device device)
        {
            if (currentState != TestState.Idle) return;
            SensorReading reading = new SensorReading();
            reading.Value = 1000;
            try
            {
                
                bool writeResult = await modbusService.WriteRegisterAsync(device, 0, 110);
                
                if (writeResult)
                {
                   
                    reading.Value =1101;                    
                    currentState = TestState.Test1;
                }
                else
                {
                    reading.Value = 1100;
                    currentState = TestState.Idle;
                }
                TestStatusUpdated?.Invoke(reading);
            }
            catch (Exception ex)
            {
                TestStatusUpdated?.Invoke(reading);
            }
        }
        private async Task RunSecondTestLogic(Device device)
        {
            if (currentState != TestState.Idle) return;
            SensorReading reading = new SensorReading();
            reading.Value = 1000;
            try
            {                
                bool writeResult = await modbusService.WriteRegisterAsync(device, 0, 120);

                if (writeResult)
                {
                    reading.Value = 1201;
                    currentState = TestState.Test2;
                }
                else
                {
                    reading.Value =1200;
                    currentState = TestState.Idle;
                }
                TestStatusUpdated?.Invoke(reading);
            }
            catch (Exception ex)
            {
                TestStatusUpdated?.Invoke(reading);
            }
        }
        private async Task RunThirdTestLogic(Device device)
        {
            if (currentState != TestState.Idle) return;
            SensorReading reading = new SensorReading();
            reading.Value = 1000;
            try
            {
                bool writeResult = await modbusService.WriteRegisterAsync(device, 0, 130);

                if (writeResult)
                {
                    reading.Value = 1301;
                    currentState = TestState.Test3;
                }
                else
                {
                    reading.Value = 1300;
                    currentState = TestState.Idle;
                }
                TestStatusUpdated?.Invoke(reading);
            }
            catch (Exception ex)
            {
                TestStatusUpdated?.Invoke(reading);
            }
        }
        private async Task RunFullTestLogic(Device device)
        {
            if (currentState != TestState.Idle) return;
            SensorReading reading = new SensorReading();
            reading.Value = 1000;
            try
            {
                bool writeResult = await modbusService.WriteRegisterAsync(device, 0, 110);
                if (writeResult)
                {
                    writeResult = await modbusService.WriteCoilAsync(device, 13, true);
                }
                else
                {
                    currentState = TestState.Idle;
                    return;
                }

                if (writeResult)
                {
                    currentState = TestState.Test1;
                }
                else
                {
                    currentState = TestState.Idle;
                }
                TestStatusUpdated?.Invoke(reading);
            }
            catch (Exception ex)
            {
                TestStatusUpdated?.Invoke(reading);
            }
        }
        public void StartTest(TestMode mode, Device deviceToTest)
        {
            this.deviceToTest = deviceToTest;
           
            switch (mode)
            {
                case TestMode.FirstTest:
                    Task.Run(() => RunFirstTestLogic(deviceToTest));
                    break;
                case TestMode.SecondTest:
                    Task.Run(() => RunSecondTestLogic(deviceToTest));
                    break;
                case TestMode.ThirdTest:
                    Task.Run(() => RunThirdTestLogic(deviceToTest));
                    break;
                case TestMode.FullMode:
                    Task.Run(() => RunFullTestLogic(deviceToTest));
                    break;

                default:
                    return;
            }
        }
    }

}
