using Core.Notification.Enum;
using Core.Scheduler.Domain;
using Core.Scheduler.ViewModel;
using System.Collections.Generic;

namespace Core.Scheduler
{
    public interface IEstimateService
    {
        JobEstimateEditModel Get(long estimateId);
        void Save(JobEstimateEditModel model);
        bool Delete(long estimateId);
        JobEstimateEditModel GetVacationInfo(long id);
        long SaveMeeting(JobEstimateEditModel model);
        long SaveMeeting(VacationRepeatEditModel model);
        void SaveVacation(JobEstimateEditModel model);
        bool DeleteVacation(long id);
        JobOccurenceListModel GetOccurenceInfo(long id);
        bool SaveSchedule(JobOccurenceListModel model);
        bool CheckDuplicateAssignment(JobOccurenceListModel model);
        void RepeatVacation(VacationRepeatEditModel model);
        List<long> GetUserIdsByMeeting(long meetingId);
        //void SaveMeetingForUser(JobEstimateEditModel model);
        void RepeatMeeting(VacationRepeatEditModel model);
        MeetingEditModel GetMessageInfo(long id);

        long GetParentMeetingId(long meetingId);

        bool DeleteMeeting(long id,long techId);
        bool EditMeeting(JobEstimateEditModel model);

        long SaveMeetingForUser(JobEstimateEditModel model);
        JobOccurenceListModel GetEstimateOccurenceInfo(long id);
        bool SaveEstimateSchedule(JobOccurenceListModel model);
        bool CheckCurrentEstimateDeletion(JobOccurenceListModel model);
        bool EditMeetingForEquipment(JobEstimateEditModel model);
        void SendingJobUpdationMails(JobEditModel model, long items);
        void SendingUpdationMails(JobEstimateEditModel model, long items);
        void SendMailToMember(JobEstimateEditModel model, NotificationTypes notificationTypes);
    }
}
