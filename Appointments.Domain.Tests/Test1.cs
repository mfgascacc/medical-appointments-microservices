using System;
using Appointments.Domain.Entities;
using Appointments.Domain.Enums;

namespace Appointments.Domain.Tests;

[TestClass]
public sealed class AppointmentTests
{
    [TestMethod]
    public void Constructor_WithValidData_InitialStatusIsPending()
    {
        var appointment = new Appointment(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow);

        Assert.AreEqual(AppointmentStatus.Pending, appointment.Status);
    }

    [TestMethod]
    public void Start_FromPending_ChangesStatusToInProgress()
    {
        var appointment = new Appointment(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow);

        appointment.Start();

        Assert.AreEqual(AppointmentStatus.InProgress, appointment.Status);
    }

    [TestMethod]
    public void Finish_FromInProgress_ChangesStatusToFinished()
    {
        var appointment = new Appointment(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow);
        appointment.Start();

        appointment.Finish();

        Assert.AreEqual(AppointmentStatus.Finished, appointment.Status);
    }

    [TestMethod]
    public void Finish_FromPending_ThrowsInvalidOperationException()
    {
        var appointment = new Appointment(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow);

        Assert.ThrowsException<InvalidOperationException>(() => appointment.Finish());
    }

    [TestMethod]
    public void Start_WhenAlreadyInProgress_ThrowsInvalidOperationException()
    {
        var appointment = new Appointment(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow);
        appointment.Start();

        Assert.ThrowsException<InvalidOperationException>(() => appointment.Start());
    }
}
