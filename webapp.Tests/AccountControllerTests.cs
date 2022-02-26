using System;
using Xunit;
using Controllers;
using Models;
using Views;

namespace webapp.Tests
{
    public class AccountControllerTests
    {
        private AccountController controller;
        
        [Fact]
        public void ForgotPasswordWithValidUser()
        {
            var model new ForgotPasswordModel();
            model.Input.EmailAddress = "njohnson@nicholasrjohnson.com"
            controller = new AccountController();

            var result = controller.ForgotPassword(model) as ViewResult;

            Assert.AreEqual("")
        }

        [[Fact]
        public void ForgotPasswordWithInvaliduser()
        {
            var model new ForgotPasswordModel();
            model.Input.EmailAddress = "example@nicholasrjohnson.com"
            controller = new AccountController();

            var result = controller.ForgotPassword(model) as ViewResult;
        }]
    }
}
