using System.Collections.Generic;
using AttendanceSyncApp.Models.DTOs.GpsSystem;

namespace AttendanceSyncApp.Services.Interfaces
{
    public interface IGpsSystemService
    {
        List<GpsTrackerUserDto> GetTrackerUsers();
        bool SaveTrackerUser(GpsTrackerUserDto dto);

        GpsTrackerUserDto GetTrackerUserByMobileNo(string mobileNo);
        bool SaveFieldVisit(GpsFieldVisitSaveDto dto, string deviceInfo, string ipAddress);
        List<GpsVisitHistoryDto> GetFieldVisitHistory();
        List<GpsLiveLocationDto> GetCurrentLiveLocations();
        bool SaveLiveLocation(GpsLiveLocationSaveDto dto);
      
    }
}