using System;

namespace SC.DevChallenge.Domain.Abstractions
{
    public interface IDateTimeConverter
    {
        int DateTimeToTimeSlot(DateTime dateTime);

        DateTime GetTimeSlotStartDate(int timeslot);

        DateTime GetTimeSlotStartDate(DateTime dateTime);
    }
}