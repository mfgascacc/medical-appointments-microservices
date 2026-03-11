using System;
using Prescriptions.Domain.Entities;
using Prescriptions.Domain.Enums;

namespace Prescriptions.Domain.Tests;

[TestClass]
public sealed class PrescriptionTests
{
    [TestMethod]
    public void Constructor_WithValidData_InitialStatusIsActive()
    {
        var prescription = new Prescription(Guid.NewGuid(), "RX-100", Guid.NewGuid(), DateTime.UtcNow);

        Assert.AreEqual(PrescriptionStatus.Active, prescription.Status);
    }

    [TestMethod]
    public void MarkDelivered_FromActive_ChangesStatusToDelivered()
    {
        var prescription = new Prescription(Guid.NewGuid(), "RX-100", Guid.NewGuid(), DateTime.UtcNow);

        prescription.MarkDelivered();

        Assert.AreEqual(PrescriptionStatus.Delivered, prescription.Status);
    }

    [TestMethod]
    public void MarkExpired_FromActive_ChangesStatusToExpired()
    {
        var prescription = new Prescription(Guid.NewGuid(), "RX-100", Guid.NewGuid(), DateTime.UtcNow);

        prescription.MarkExpired();

        Assert.AreEqual(PrescriptionStatus.Expired, prescription.Status);
    }

    [TestMethod]
    public void MarkDelivered_WhenExpired_ThrowsInvalidOperationException()
    {
        var prescription = new Prescription(Guid.NewGuid(), "RX-100", Guid.NewGuid(), DateTime.UtcNow);
        prescription.MarkExpired();

        Assert.ThrowsException<InvalidOperationException>(() => prescription.MarkDelivered());
    }

    [TestMethod]
    public void Constructor_WithWhitespaceCode_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Prescription(Guid.NewGuid(), " ", Guid.NewGuid(), DateTime.UtcNow));
    }
}
