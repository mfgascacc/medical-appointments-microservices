using System;
using People.Domain.Entities;
using People.Domain.Enums;

namespace People.Domain.Tests;

[TestClass]
public sealed class PersonTests
{
    [TestMethod]
    public void Constructor_WithValidData_CreatesPerson()
    {
        var id = Guid.NewGuid();

        var person = new Person(id, " Ana ", " Ruiz ", " CC-123 ", PersonType.Patient);

        Assert.AreEqual(id, person.Id);
        Assert.AreEqual("Ana", person.FirstName);
        Assert.AreEqual("Ruiz", person.LastName);
        Assert.AreEqual("CC-123", person.DocumentNumber);
        Assert.AreEqual(PersonType.Patient, person.Type);
    }

    [TestMethod]
    public void Constructor_WithEmptyId_ThrowsArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            new Person(Guid.Empty, "Ana", "Ruiz", "CC-123", PersonType.Patient));
    }

    [TestMethod]
    public void Constructor_WithEmptyFirstName_ThrowsArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            new Person(Guid.NewGuid(), "", "Ruiz", "CC-123", PersonType.Patient));
    }

    [TestMethod]
    public void Constructor_WithWhitespaceLastName_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Person(Guid.NewGuid(), "Ana", " ", "CC-123", PersonType.Patient));
    }

    [TestMethod]
    public void Constructor_WithWhitespaceDocumentNumber_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Person(Guid.NewGuid(), "Ana", "Ruiz", " ", PersonType.Patient));
    }
}
