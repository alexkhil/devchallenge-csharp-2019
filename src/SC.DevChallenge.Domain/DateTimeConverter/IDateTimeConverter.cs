using System;

namespace SC.DevChallenge.Domain.DateTimeConverter
{
    public interface IDateTimeConverter
    {
        int DateTimeToTimeSlot(DateTime dateTime);

        DateTime GetTimeSlotStartDate(int timeslot);

        DateTime GetTimeSlotStartDate(DateTime dateTime);
    }
}