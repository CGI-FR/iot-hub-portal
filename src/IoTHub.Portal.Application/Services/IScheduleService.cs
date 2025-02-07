// Copyright (c) CGI France. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace IoTHub.Portal.Application.Services
{
    public interface IScheduleService
    {
        Task<ScheduleDto> CreateSchedule(ScheduleDto schedule);
        Task UpdateSchedule(ScheduleDto schedule);
        Task DeleteSchedule(string scheduleId);
        Task<Schedule> GetSchedule(string scheduleId);
        Task<IEnumerable<ScheduleDto>> GetSchedules();
    }
}
