using System;
using Ploeh.AutoFixture;
using Xunit;

namespace ConventionMapper.Tests
{
    public class MapperTests
    {
        [Fact]
        public void MappingObject_WithSamePropertyTypeAndNames_ShouldMapTheValuesCorrectly_ByHavingTheResultObjectPropertyValues_EqualTo_TheSourcePropertyValues()
        {
            var customer = new Fixture().CreateAnonymous<Customer>();

            CustomerDTO customerDTO = Mapper<Customer, CustomerDTO>.MapByPropertyConvention(customer);

            Assert.Equal(customerDTO.Name, customer.Name);
            Assert.Equal(customerDTO.Id, customer.Id);
            Assert.Equal(customerDTO.LoginDate, customer.LoginDate);
        }

        [Fact]
        public void MappingObject_WithSamePropertyTypeAndNames_ShouldFillTheResultPropertyValues_WithOnesFromSource_AndKeepTheExistingSetOnesIntact()
        {
            const string actual = "YO";

            var customerDTO = new CustomerDTO { Id = actual };

            var customer = new Fixture().CreateAnonymous<Customer>();

            Mapper<CustomerDTO, Customer>.MapByPropertyConventionModifiyingExisting(customerDTO, customer);

            Assert.Equal(customer.Name, customer.Name);
            Assert.Equal(customer.Id, actual);
            Assert.Equal(customer.LoginDate, customer.LoginDate);
        }

        public class Customer
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public DateTime LoginDate { get; set; }
            public bool IsActive { get; set; }
        }

        public class CustomerDTO
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public DateTime LoginDate { get; set; }
        }
    }
}
