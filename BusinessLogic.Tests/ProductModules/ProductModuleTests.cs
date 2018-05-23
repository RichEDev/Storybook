namespace BusinessLogic.Tests.ProductModules
{
    using System;

    using BusinessLogic.Modules;
    using BusinessLogic.ProductModules;

    using Xunit;

    public class ProductModuleTests
    {
        public class Ctor
        {
            [Theory]
            [InlineData(1)]
            [InlineData(2)]
            [InlineData(3)]
            [InlineData(4)]
            [InlineData(5)]
            [InlineData(6)]
            [InlineData(7)]
            [InlineData(9)]
            [InlineData(12)]
            [InlineData(10000)]
            public void ValidCtor_InitializingProductModule_SetsProperties(int id)
            {
                IProductModule sut;

                switch ((Modules)id)
                {
                    case Modules.PurchaseOrders:
                        sut = new PurchaseOrderProductModule(id, "ref", "refDesc", "BrandName", "BrandName");
                        break;
                    case Modules.Expenses:
                        sut = new ExpensesProductModule(id, "ref", "refDesc", "BrandName", "BrandName");
                        break;
                    case Modules.Contracts:
                        sut = new FrameworkProductModule(id, "ref", "refDesc", "BrandName", "BrandName");
                        break;
                    case Modules.SpendManagement:
                        sut = new SpendManagementProductModule(id, "ref", "refDesc", "BrandName", "BrandName");
                        break;
                    case Modules.SmartDiligence:
                    case Modules.CorporateDiligence:
                        sut = new CorporateDilligenceProductModule(id, "ref", "refDesc", "BrandName", "BrandName");
                        break;
                    case Modules.Greenlight:
                        sut = new GreenLightProductModule(id, "ref", "refDesc", "BrandName", "BrandName");
                        break;
                    case Modules.GreenlightWorkforce:
                        sut = new GreenLightWorkForceProductModule(id, "ref", "refDesc", "BrandName", "BrandName");
                        break;
                    case Modules.ESR:
                        sut = new EsrProductModule(id, "ref", "refDesc", "BrandName", "BrandName");
                        break;
                    default:
                        sut = new NullProductModule(id, "ref", "refDesc", "BrandName", "BrandName");
                        break;
                }

                Assert.Equal(id, sut.Id);
                Assert.Equal("ref", sut.Name);
                Assert.Equal("refDesc", sut.Description);
                Assert.Equal("BrandName", sut.BrandName);
                Assert.Equal("BrandName", sut.BrandNameHtml);
                Assert.Equal("~/home.aspx", sut.HomePage);
            }

            [Theory]
            [InlineData(1)]
            [InlineData(2)]
            [InlineData(3)]
            [InlineData(4)]
            [InlineData(5)]
            [InlineData(6)]
            [InlineData(7)]
            [InlineData(9)]
            [InlineData(12)]
            [InlineData(10000)]
            public void InvalidId_InitializingProductModule_ThrowsArgumentException(int id)
            {
                switch ((Modules)id)
                {
                    case Modules.PurchaseOrders:
                        Assert.Throws<ArgumentException>(() =>
                            new PurchaseOrderProductModule(-1, "ref", "refDesc", "BrandName", "BrandName"));
                        break;
                    case Modules.Expenses:
                        Assert.Throws<ArgumentException>(() =>
                            new ExpensesProductModule(-1, "ref", "refDesc", "BrandName", "BrandName"));
                        break;
                    case Modules.Contracts:
                        Assert.Throws<ArgumentException>(() =>
                            new FrameworkProductModule(-1, "ref", "refDesc", "BrandName", "BrandName"));
                        break;
                    case Modules.SpendManagement:
                        Assert.Throws<ArgumentException>(() =>
                            new SpendManagementProductModule(-1, "ref", "refDesc", "BrandName", "BrandName"));
                        break;
                    case Modules.SmartDiligence:
                    case Modules.CorporateDiligence:
                        Assert.Throws<ArgumentException>(() =>
                            new CorporateDilligenceProductModule(-1, "ref", "refDesc", "BrandName", "BrandName"));
                        break;
                    case Modules.Greenlight:
                        Assert.Throws<ArgumentException>(() =>
                            new GreenLightProductModule(-1, "ref", "refDesc", "BrandName", "BrandName"));
                        break;
                    case Modules.GreenlightWorkforce:
                        Assert.Throws<ArgumentException>(() =>
                            new GreenLightWorkForceProductModule(-1, "ref", "refDesc", "BrandName", "BrandName"));
                        break;
                    case Modules.ESR:
                        Assert.Throws<ArgumentException>(() =>
                            new EsrProductModule(-1, "ref", "refDesc", "BrandName", "BrandName"));
                        break;
                    default:
                        Assert.Throws<ArgumentException>(() =>
                            new NullProductModule(-1, "ref", "refDesc", "BrandName", "BrandName"));
                        break;
                }
            }

            [Theory]
            [InlineData(1)]
            [InlineData(2)]
            [InlineData(3)]
            [InlineData(4)]
            [InlineData(5)]
            [InlineData(6)]
            [InlineData(7)]
            [InlineData(9)]
            [InlineData(12)]
            [InlineData(10000)]
            public void InvalidDescription_InitializingProductModule_ThrowsArgumentNullException(int id)
            {
                switch ((Modules)id)
                {
                    case Modules.PurchaseOrders:
                        Assert.Throws<ArgumentNullException>(() =>
                            new PurchaseOrderProductModule(id, "ref", null, "BrandName", "BrandName"));
                        break;
                    case Modules.Expenses:
                        Assert.Throws<ArgumentNullException>(() =>
                            new ExpensesProductModule(id, "ref", null, "BrandName", "BrandName"));
                        break;
                    case Modules.Contracts:
                        Assert.Throws<ArgumentNullException>(() =>
                            new FrameworkProductModule(id, "ref", null, "BrandName", "BrandName"));
                        break;
                    case Modules.SpendManagement:
                        Assert.Throws<ArgumentNullException>(() =>
                            new SpendManagementProductModule(id, "ref", null, "BrandName", "BrandName"));
                        break;
                    case Modules.SmartDiligence:
                    case Modules.CorporateDiligence:
                        Assert.Throws<ArgumentNullException>(() =>
                            new CorporateDilligenceProductModule(id, "ref", null, "BrandName", "BrandName"));
                        break;
                    case Modules.Greenlight:
                        Assert.Throws<ArgumentNullException>(() =>
                            new GreenLightProductModule(id, "ref", null, "BrandName", "BrandName"));
                        break;
                    case Modules.GreenlightWorkforce:
                        Assert.Throws<ArgumentNullException>(() =>
                            new GreenLightWorkForceProductModule(id, "ref", null, "BrandName", "BrandName"));
                        break;
                    case Modules.ESR:
                        Assert.Throws<ArgumentNullException>(() =>
                            new EsrProductModule(id, "ref", null, "BrandName", "BrandName"));
                        break;
                    default:
                        Assert.Throws<ArgumentNullException>(() =>
                            new NullProductModule(id, "ref", null, "BrandName", "BrandName"));
                        break;
                }
            }

            [Theory]
            [InlineData(1)]
            [InlineData(2)]
            [InlineData(3)]
            [InlineData(4)]
            [InlineData(5)]
            [InlineData(6)]
            [InlineData(7)]
            [InlineData(9)]
            [InlineData(12)]
            [InlineData(10000)]
            public void InvalidName_InitializingProductModule_ThrowsArgumentNullException(int id)
            {
                switch ((Modules)id)
                {
                    case Modules.PurchaseOrders:
                        Assert.Throws<ArgumentNullException>(() =>
                            new PurchaseOrderProductModule(id, null, "refDesc", "BrandName", "BrandName"));
                        break;
                    case Modules.Expenses:
                        Assert.Throws<ArgumentNullException>(() =>
                            new ExpensesProductModule(id, null, "refDesc", "BrandName", "BrandName"));
                        break;
                    case Modules.Contracts:
                        Assert.Throws<ArgumentNullException>(() =>
                            new FrameworkProductModule(id, null, "refDesc", "BrandName", "BrandName"));
                        break;
                    case Modules.SpendManagement:
                        Assert.Throws<ArgumentNullException>(() =>
                            new SpendManagementProductModule(id, null, "refDesc", "BrandName", "BrandName"));
                        break;
                    case Modules.SmartDiligence:
                    case Modules.CorporateDiligence:
                        Assert.Throws<ArgumentNullException>(() =>
                            new CorporateDilligenceProductModule(id, null, "refDesc", "BrandName", "BrandName"));
                        break;
                    case Modules.Greenlight:
                        Assert.Throws<ArgumentNullException>(() =>
                            new GreenLightProductModule(id, null, "refDesc", "BrandName", "BrandName"));
                        break;
                    case Modules.GreenlightWorkforce:
                        Assert.Throws<ArgumentNullException>(() =>
                            new GreenLightWorkForceProductModule(id, null, "refDesc", "BrandName", "BrandName"));
                        break;
                    case Modules.ESR:
                        Assert.Throws<ArgumentNullException>(() =>
                            new EsrProductModule(id, null, "refDesc", "BrandName", "BrandName"));
                        break;
                    default:
                        Assert.Throws<ArgumentNullException>(() =>
                            new NullProductModule(id, null, "refDesc", "BrandName", "BrandName"));
                        break;
                }
            }
        }
    }
}
