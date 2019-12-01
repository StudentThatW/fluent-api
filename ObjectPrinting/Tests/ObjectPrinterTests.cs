﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace ObjectPrinting.Tests
{
    [TestFixture]
    public class ObjectPrinterTests
    {

        private Person person;
        
        [SetUp]
        public void CreatePerson()
        {
            person = new Person { Name = "James", Age = 30, Height = 182.2, Family = new List<Person>()};
        }
        
        [Test]
        public void ExcludingType_ShouldExcludePropertiesOfSelectedType()
        {
            var printer = ObjectPrinter.For<Person>().Excluding<int>();
            printer.PrintToString(person).Should().NotContain("30").And.Contain("James");
        }

        [Test]
        public void ExcludingProperty_ShouldExcludeSelectedProperty()
        {
            var printer = ObjectPrinter.For<Person>().Excluding(p => p.Name);
            printer.PrintToString(person).Should().NotContain("James").And.NotContain("Name");
        }

        
        [TestCase("en-GB", "182.2")]
        [TestCase("de", "182,2")]
        public void Using_ShouldSerializeNumericPropertiesWithSelectedCulture(string culture, string expected)
        {
            var printer = ObjectPrinter.For<Person>()
                .Printing<double>().Using(CultureInfo.GetCultureInfo(culture));
            printer.PrintToString(person).Should().Contain(expected);            
        }
        

        [Test]
        public void Using_ShouldSerializeSelectedTypeProperties_WithAlternativeMethod()
        {
            var printer = ObjectPrinter.For<Person>()
                .Printing<int>().Using(i => i.ToString("X"));
            printer.PrintToString(person).Should().NotContain("30").And.Contain("1E");
        }

        [Test]
        public void Trim_ShouldReturnProperty_WithTrimmedLength()
        {
            var printer = ObjectPrinter.For<Person>()
                .Printing(p => p.Name).Trim(4);
            printer.PrintToString(person).Should().Contain("Jame").And.NotContain("James");
        }

        [Test]
        public void PrintToString_ShouldPrintPropertiesByConfig()
        {
            person.PrintToString(s => s.Excluding(p => p.Name))
                .Should().NotContain("James");
        }

        [Test]
        public void PrintToString_ShouldPrintElementsOfList()
        {
            var person1 = new Person { Name = "Alex", Age = 19, Height = 170.2 };
            var listOfPersons = new List<Person> { person, person1 };
            var printer = ObjectPrinter.For<List<Person>>();
            printer.PrintToString(listOfPersons).Should().Contain("0 : Person").And.Contain("1 : Person");
        }

        [Test]
        public void PrintToString_ShouldPrintElementsOfDictionary()
        {
            var person1 = new Person { Name = "Alex", Age = 19, Height = 170.2 };
            var dictOfPersons = new Dictionary<string, Person> { { "olderPerson", person}, {"youngerPerson", person1 } };
            var printer = ObjectPrinter.For<Dictionary<string, Person>>();
            printer.PrintToString(dictOfPersons).Should().Contain("olderPerson : Person").And.Contain("youngerPerson : Person");
        }
    }
}