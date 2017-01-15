namespace AvgIdentity.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AvgIdentity.Managers;
    using AvgIdentity.Models;
    using AvgIdentity.Test.Mocks;

    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;


    [TestClass]
    public class UserRoleManagerTests
    {

        private List<IdentityRole> roles = new List<IdentityRole>();

        [TestMethod]
        public void TestMethod1()
        {

            var a = new TestInMemoryIdentityDbContext();

            var rolemanager = new UserRoleManager<AvgIdentityUser, TestInMemoryIdentityDbContext>(null, null, a);

            var res = rolemanager.AddRoleAsync("aaa").Result;

            Assert.IsTrue(res);

            res = rolemanager.AddRoleAsync("aaa").Result;

            Assert.IsFalse(res);

            res = rolemanager.AddRoleAsync(new[] { "aaa", "bbb" }).Result;

            Assert.IsFalse(res);

            res = rolemanager.AddRoleAsync(new[] { "ccc", "bbb" }).Result;

            Assert.IsTrue(res);
        }
    }
}