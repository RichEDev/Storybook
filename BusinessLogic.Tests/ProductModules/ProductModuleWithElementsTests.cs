namespace BusinessLogic.Tests.ProductModules
{
    using System;
    using System.Collections.Generic;

    using BusinessLogic.Elements;
    using BusinessLogic.Modules;
    using BusinessLogic.ProductModules;
    using BusinessLogic.ProductModules.Elements;

    using Xunit;

    public class ProductModuleWithElementsTests
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
            public void ValidCtor_InitializingProductModuleWithElements_SetsProperties(int id)
            {
                IProductModuleWithElements sut;

                switch ((Modules)id)
                {
                    case Modules.PurchaseOrders:
                        sut = new PurchaseOrderProductModuleWithElements(id, "ref", "refDesc", "BrandName", "BrandName", new List<IElement>());
                        break;
                    case Modules.Expenses:
                        sut = new ExpensesProductModuleWithElements(id, "ref", "refDesc", "BrandName", "BrandName", new List<IElement>());
                        break;
                    case Modules.Contracts:
                        sut = new FrameworkProductModuleWithElements(id, "ref", "refDesc", "BrandName", "BrandName", new List<IElement>());
                        break;
                    case Modules.SpendManagement:
                        sut = new SpendManagementProductModuleWithElements(id, "ref", "refDesc", "BrandName", "BrandName", new List<IElement>());
                        break;
                    case Modules.SmartDiligence:
                    case Modules.CorporateDiligence:
                        sut = new CorporateDilligenceProductModuleWithElements(id, "ref", "refDesc", "BrandName", "BrandName", new List<IElement>());
                        break;
                    case Modules.Greenlight:
                        sut = new GreenLightProductModuleWithElements(id, "ref", "refDesc", "BrandName", "BrandName", new List<IElement>());
                        break;
                    case Modules.GreenlightWorkforce:
                        sut = new GreenLightWorkForceProductModuleWithElements(id, "ref", "refDesc", "BrandName", "BrandName", new List<IElement>());
                        break;
                    case Modules.ESR:
                        sut = new EsrProductModuleWithElements(id, "ref", "refDesc", "BrandName", "BrandName", new List<IElement>());
                        break;
                    default:
                        sut = new NullProductModuleWithElements(id, "ref", "refDesc", "BrandName", "BrandName", new List<IElement>());
                        break;
                }

                Assert.Equal(id, sut.Id);
                Assert.Equal("ref", sut.Name);
                Assert.Equal("refDesc", sut.Description);
                Assert.Equal("BrandName", sut.BrandName);
                Assert.Equal("BrandName", sut.BrandNameHtml);
                Assert.Equal("~/home.aspx", sut.HomePage);
                Assert.Equal(new List<IElement>(), sut.Elements);
                Assert.Equal(0, sut.Elements.Count);
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
            public void InvalidId_InitializingProductModuleWithElements_ThrowsArgumentException(int id)
            {
                switch ((Modules)id)
                {
                    case Modules.PurchaseOrders:
                        Assert.Throws<ArgumentException>(() =>
                            new PurchaseOrderProductModuleWithElements(-1, "ref", "refDesc", "BrandName", "BrandName", new List<IElement>()));
                        break;
                    case Modules.Expenses:
                        Assert.Throws<ArgumentException>(() =>
                            new ExpensesProductModuleWithElements(-1, "ref", "refDesc", "BrandName", "BrandName", new List<IElement>()));
                        break;
                    case Modules.Contracts:
                        Assert.Throws<ArgumentException>(() =>
                            new FrameworkProductModuleWithElements(-1, "ref", "refDesc", "BrandName", "BrandName", new List<IElement>()));
                        break;
                    case Modules.SpendManagement:
                        Assert.Throws<ArgumentException>(() =>
                            new SpendManagementProductModuleWithElements(-1, "ref", "refDesc", "BrandName", "BrandName", new List<IElement>()));
                        break;
                    case Modules.SmartDiligence:
                    case Modules.CorporateDiligence:
                        Assert.Throws<ArgumentException>(() =>
                            new CorporateDilligenceProductModuleWithElements(-1, "ref", "refDesc", "BrandName", "BrandName", new List<IElement>()));
                        break;
                    case Modules.Greenlight:
                        Assert.Throws<ArgumentException>(() =>
                            new GreenLightProductModuleWithElements(-1, "ref", "refDesc", "BrandName", "BrandName", new List<IElement>()));
                        break;
                    case Modules.GreenlightWorkforce:
                        Assert.Throws<ArgumentException>(() =>
                            new GreenLightWorkForceProductModuleWithElements(-1, "ref", "refDesc", "BrandName", "BrandName", new List<IElement>()));
                        break;
                    case Modules.ESR:
                        Assert.Throws<ArgumentException>(() =>
                            new EsrProductModuleWithElements(-1, "ref", "refDesc", "BrandName", "BrandName", new List<IElement>()));
                        break;
                    default:
                        Assert.Throws<ArgumentException>(() =>
                            new NullProductModuleWithElements(-1, "ref", "refDesc", "BrandName", "BrandName", new List<IElement>()));
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
            public void InvalidDescription_InitializingProductModuleWithElements_ThrowsArgumentNullException(int id)
            {
                switch ((Modules)id)
                {
                    case Modules.PurchaseOrders:
                        Assert.Throws<ArgumentNullException>(() =>
                            new PurchaseOrderProductModuleWithElements(id, "ref", null, "BrandName", "BrandName", new List<IElement>()));
                        break;
                    case Modules.Expenses:
                        Assert.Throws<ArgumentNullException>(() =>
                            new ExpensesProductModuleWithElements(id, "ref", null, "BrandName", "BrandName", new List<IElement>()));
                        break;
                    case Modules.Contracts:
                        Assert.Throws<ArgumentNullException>(() =>
                            new FrameworkProductModuleWithElements(id, "ref", null, "BrandName", "BrandName", new List<IElement>()));
                        break;
                    case Modules.SpendManagement:
                        Assert.Throws<ArgumentNullException>(() =>
                            new SpendManagementProductModuleWithElements(id, "ref", null, "BrandName", "BrandName", new List<IElement>()));
                        break;
                    case Modules.SmartDiligence:
                    case Modules.CorporateDiligence:
                        Assert.Throws<ArgumentNullException>(() =>
                            new CorporateDilligenceProductModuleWithElements(id, "ref", null, "BrandName", "BrandName", new List<IElement>()));
                        break;
                    case Modules.Greenlight:
                        Assert.Throws<ArgumentNullException>(() =>
                            new GreenLightProductModuleWithElements(id, "ref", null, "BrandName", "BrandName", new List<IElement>()));
                        break;
                    case Modules.GreenlightWorkforce:
                        Assert.Throws<ArgumentNullException>(() =>
                            new GreenLightWorkForceProductModuleWithElements(id, "ref", null, "BrandName", "BrandName", new List<IElement>()));
                        break;
                    case Modules.ESR:
                        Assert.Throws<ArgumentNullException>(() =>
                            new EsrProductModuleWithElements(id, "ref", null, "BrandName", "BrandName", new List<IElement>()));
                        break;
                    default:
                        Assert.Throws<ArgumentNullException>(() =>
                            new NullProductModuleWithElements(id, "ref", null, "BrandName", "BrandName", new List<IElement>()));
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
            public void InvalidName_InitializingProductModuleWithElements_ThrowsArgumentNullException(int id)
            {
                switch ((Modules)id)
                {
                    case Modules.PurchaseOrders:
                        Assert.Throws<ArgumentNullException>(() =>
                            new PurchaseOrderProductModuleWithElements(id, null, "refDesc", "BrandName", "BrandName", new List<IElement>()));
                        break;
                    case Modules.Expenses:
                        Assert.Throws<ArgumentNullException>(() =>
                            new ExpensesProductModuleWithElements(id, null, "refDesc", "BrandName", "BrandName", new List<IElement>()));
                        break;
                    case Modules.Contracts:
                        Assert.Throws<ArgumentNullException>(() =>
                            new FrameworkProductModuleWithElements(id, null, "refDesc", "BrandName", "BrandName", new List<IElement>()));
                        break;
                    case Modules.SpendManagement:
                        Assert.Throws<ArgumentNullException>(() =>
                            new SpendManagementProductModuleWithElements(id, null, "refDesc", "BrandName", "BrandName", new List<IElement>()));
                        break;
                    case Modules.SmartDiligence:
                    case Modules.CorporateDiligence:
                        Assert.Throws<ArgumentNullException>(() =>
                            new CorporateDilligenceProductModuleWithElements(id, null, "refDesc", "BrandName", "BrandName", new List<IElement>()));
                        break;
                    case Modules.Greenlight:
                        Assert.Throws<ArgumentNullException>(() =>
                            new GreenLightProductModuleWithElements(id, null, "refDesc", "BrandName", "BrandName", new List<IElement>()));
                        break;
                    case Modules.GreenlightWorkforce:
                        Assert.Throws<ArgumentNullException>(() =>
                            new GreenLightWorkForceProductModuleWithElements(id, null, "refDesc", "BrandName", "BrandName", new List<IElement>()));
                        break;
                    case Modules.ESR:
                        Assert.Throws<ArgumentNullException>(() =>
                            new EsrProductModuleWithElements(id, null, "refDesc", "BrandName", "BrandName", new List<IElement>()));
                        break;
                    default:
                        Assert.Throws<ArgumentNullException>(() =>
                            new NullProductModuleWithElements(id, null, "refDesc", "BrandName", "BrandName", new List<IElement>()));
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
            public void InvalidElements_InitializingProductModuleWithElements_ThrowsArgumentNullException(int id)
            {
                switch ((Modules)id)
                {
                    case Modules.PurchaseOrders:
                        Assert.Throws<ArgumentNullException>(() =>
                            new PurchaseOrderProductModuleWithElements(id, "ref", "refDesc", "BrandName", "BrandName", null));
                        break;
                    case Modules.Expenses:
                        Assert.Throws<ArgumentNullException>(() =>
                            new ExpensesProductModuleWithElements(id, "ref", "refDesc", "BrandName", "BrandName", null));
                        break;
                    case Modules.Contracts:
                        Assert.Throws<ArgumentNullException>(() =>
                            new FrameworkProductModuleWithElements(id, "ref", "refDesc", "BrandName", "BrandName", null));
                        break;
                    case Modules.SpendManagement:
                        Assert.Throws<ArgumentNullException>(() =>
                            new SpendManagementProductModuleWithElements(id, "ref", "refDesc", "BrandName", "BrandName", null));
                        break;
                    case Modules.SmartDiligence:
                    case Modules.CorporateDiligence:
                        Assert.Throws<ArgumentNullException>(() =>
                            new CorporateDilligenceProductModuleWithElements(id, "ref", "refDesc", "BrandName", "BrandName", null));
                        break;
                    case Modules.Greenlight:
                        Assert.Throws<ArgumentNullException>(() =>
                            new GreenLightProductModuleWithElements(id, "ref", "refDesc", "BrandName", "BrandName", null));
                        break;
                    case Modules.GreenlightWorkforce:
                        Assert.Throws<ArgumentNullException>(() =>
                            new GreenLightWorkForceProductModuleWithElements(id, "ref", "refDesc", "BrandName", "BrandName", null));
                        break;
                    case Modules.ESR:
                        Assert.Throws<ArgumentNullException>(() =>
                            new EsrProductModuleWithElements(id, "ref", "refDesc", "BrandName", "BrandName", null));
                        break;
                    default:
                        Assert.Throws<ArgumentNullException>(() =>
                            new NullProductModuleWithElements(id, "ref", "refDesc", "BrandName", "BrandName", null));
                        break;
                }
            }
        }
    }
}
