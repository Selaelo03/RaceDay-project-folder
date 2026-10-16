
using RaceDay.API.Models;
using Xunit;

namespace RaceDay.API.Tests
{
    public class EventTests
    {
        [Fact]
        public void Event_ShouldStoreCorrectDetails()
        {
            // Arrange
            var raceEvent = new RaceDay.API.Models.Event
            {
                EventID = 1,
                OrganiserID = 1,
                EventName = "RaceDay Johannesburg 10K",
                Description = "Running event",
                EventDate = new DateTime(2026, 11, 15),
                Location = "Johannesburg",
                Distance = 10,
                EventType = "Run"
            };

            // Assert
            Assert.Equal(
                "RaceDay Johannesburg 10K",
                raceEvent.EventName);

            Assert.Equal(10m, raceEvent.Distance);
            Assert.Equal("Run", raceEvent.EventType);
            Assert.Equal("Johannesburg", raceEvent.Location);
        }


        [Fact]
        public void Event_ShouldAllowUpdatingDetails()
        {
            var raceEvent = new RaceDay.API.Models.Event
            {
                EventName = "Original Race",
                Location = "Pretoria",
                Distance = 5,
                EventType = "Run"
            };

            raceEvent.EventName = "Updated Race";
            raceEvent.Distance = 10;

            Assert.Equal("Updated Race", raceEvent.EventName);
            Assert.Equal(10m, raceEvent.Distance);
        }

        [Fact]
        public void Category_ShouldStoreAgeLimits()
        {
            var category = new RaceDay.API.Models.Category
            {
                CategoryName = "Adults",
                CategoryType = "Age",
                MinimumAge = 18,
                MaximumAge = 40
            };

            Assert.Equal(18, category.MinimumAge);
            Assert.Equal(40, category.MaximumAge);
        }

        [Fact]
        public void Enrollment_ShouldStoreReferences()
        {
            var enrollment = new RaceDay.API.Models.Enrollment
            {
                ParticipantID = 2,
                EventID = 5,
                CategoryID = 3
            };

            Assert.Equal(2, enrollment.ParticipantID);
            Assert.Equal(5, enrollment.EventID);
            Assert.Equal(3, enrollment.CategoryID);
        }

        [Fact]
        public void Result_ShouldStoreFinishingPosition()
        {
            var result = new RaceDay.API.Models.Result
            {
                EnrollmentID = 1,
                FinishTime = TimeSpan.FromMinutes(45),
                FinishingPosition = 3
            };

            Assert.Equal(3, result.FinishingPosition);
            Assert.Equal(
                TimeSpan.FromMinutes(45),
                result.FinishTime);
        }

    }
}
